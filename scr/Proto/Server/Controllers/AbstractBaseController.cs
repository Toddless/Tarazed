namespace Server.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    [Authorize]
    [Route("[controller]")]
    public abstract class AbstractBaseController<TU> : Controller
        where TU : class, IEntity
    {
        private readonly IDatabaseContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _manager;

        public AbstractBaseController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(manager);
            _manager = manager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async virtual Task<IEnumerable<TU>?> GetAsync(IEnumerable<long>? primaryIds)
        {
            try
            {
                var context = _context.CheckContext();
                var currentUser = await _manager.GetUserAsync(User);
                var userID = currentUser?.Id;

                if (primaryIds?.Any() ?? false)
                {
                    return await context.Set<TU>()
                         .Where(o => primaryIds.Contains(o.PrimaryId) && o.CustomerId == userID)
                         .ToListAsync();
                }

                return await context.Set<TU>()
                    .Where(o => o.CustomerId == userID)
                    .ToListAsync();
            }
            catch (InternalServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }

        [HttpPut]
        public async virtual Task<TU?> CreateAsync(TU item)
        {
            try
            {
                var currentUser = await _manager.GetUserAsync(User);
                if (item.PrimaryId != 0)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                bool isValid = item.ValidateObject(out List<ValidationResult> result);
                if (!isValid)
                {
                    result.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                var context = _context.CheckContext();

                item.CustomerId = currentUser!.Id;

                context.Set<TU>().Add(item);
                await TrackChanges(context);

                return item;
            }
            catch (InternalServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }

        [HttpPost]
        public async virtual Task<TU?> UpdateAsync(TU item)
        {
            try
            {
                if (item == null || item.PrimaryId == 0)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                bool isValid = item.ValidateObject(out List<ValidationResult> results);
                if (!isValid)
                {
                    results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                var context = _context.CheckContext();
                var currentUser = await _manager.GetUserAsync(User);
                var set = context.Set<TU>();
                var itemExists = await set.AsNoTracking()
                    .Where(o => o.PrimaryId == item.PrimaryId && o.CustomerId == currentUser!.Id)
                    .FirstOrDefaultAsync();

                if (itemExists == null)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                item.CustomerId = currentUser.Id;
                set.Update(item);
                await TrackChanges(context);
                return item;
            }
            catch (InternalServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }

        [HttpDelete]
        public virtual async Task<bool> DeleteAsync(long? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                var context = _context.CheckContext();
                var currentUser = await _manager.GetUserAsync(User);
                var user = currentUser.Id;

                var set = context.Set<TU>();
                var itemExists = await set.FirstOrDefaultAsync(x => x.PrimaryId == id && x.CustomerId == user);
                if (itemExists == null)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                set.Remove(itemExists);
                await TrackChanges(context);

                return true;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (InternalServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }

        private static async Task TrackChanges(IDatabaseContext context)
        {
            var changedCount = await context.SaveChangesAsync();
            if (changedCount != 1)
            {
                throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(TU).Name));
            }
        }
    }
}
