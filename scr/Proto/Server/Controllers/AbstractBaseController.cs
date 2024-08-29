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
        protected readonly IDatabaseContext _context;
        protected readonly ILogger _logger;
        protected readonly UserManager<ApplicationUser> _manager;

        public AbstractBaseController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            _manager = manager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async virtual Task<IEnumerable<TU>?> GetAsync(IEnumerable<long>? ids, bool useNavigationProperties = false)
        {
            try
            {
                var context = _context.CheckContext();
                var currentUser = await _manager.GetUserAsync(User).ConfigureAwait(false);

                if (currentUser == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest);
                }

                // sucht nach gegebene Ids, die currentUser gehören,
                // falls null eingegeben wurde, gibt alle items currentsUsers zurück
                IQueryable<TU> query = context.Set<TU>().Where(o => o.CustomerId == currentUser.Id);

                if (ids?.Any() ?? false)
                {
                    query = query.Where(o => ids.Contains(o.Id));
                }

                if (useNavigationProperties)
                {
                    query = AddIncludes(query);
                }

                var result = await query.AsNoTracking().ToListAsync().ConfigureAwait(false);
                return result;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(GetAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }
        }

        [HttpPut]
        public async virtual Task<TU?> CreateAsync(TU item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentNullException(DataModel.Resources.Errors.NotFound);
                }

                if (item.Id != 0)
                {
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest_PrimaryKeySet);
                }

                bool isValid = item.ValidateObject(out List<ValidationResult> result);
                if (!isValid)
                {
                    result.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest);
                }

                var currentUser = await _manager.GetUserAsync(User).ConfigureAwait(false);
                var context = _context.CheckContext();

                item.CustomerId = currentUser!.Id;

                context.Set<TU>().Add(item);
                await SaveChangesAsync(context).ConfigureAwait(false);

                return item;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(CreateAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }
        }

        [HttpPost]
        public async virtual Task<TU?> UpdateAsync(TU item)
        {
            try
            {
                if (item == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.NullObject);
                }

                if (item.Id == 0)
                {
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest_PrimaryKeyNotSet);
                }

                bool isValid = item.ValidateObject(out List<ValidationResult> results);
                if (!isValid)
                {
                    results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest);
                }

                var currentUser = await _manager.GetUserAsync(User).ConfigureAwait(false);
                var context = _context.CheckContext();
                var set = context.Set<TU>();
                var itemExists = await set.AsNoTracking()
                    .Where(o => o.CustomerId == currentUser!.Id)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (itemExists == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.ElementNotExists);
                }

                set.Update(item);
                await SaveChangesAsync(context).ConfigureAwait(false);
                return item;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(UpdateAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }
        }

        [HttpDelete]
        public virtual async Task<bool> DeleteAsync(long? id)
        {
            try
            {
                if (id == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.NullObject);
                }

                if (id == 0)
                {
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest_PrimaryKeyNotSet);
                }

                var context = _context.CheckContext();
                var currentUser = await _manager.GetUserAsync(User).ConfigureAwait(false);

                var set = context.Set<TU>();
                var itemExists = await set.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == currentUser!.Id).ConfigureAwait(false);
                if (itemExists == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.ElementNotExists);
                }

                set.Remove(itemExists);
                await SaveChangesAsync(context).ConfigureAwait(false);

                return true;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(DeleteAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }
        }

        protected abstract IQueryable<TU> AddIncludes(IQueryable<TU> query);

        private static async Task SaveChangesAsync(IDatabaseContext context)
        {
            var changedCount = await context.SaveChangesAsync().ConfigureAwait(false);
            if (changedCount != 1)
            {
                throw new InternalServerException(string.Format(DataModel.Resources.Errors.NotSaved, typeof(TU).Name));
            }
        }
    }
}
