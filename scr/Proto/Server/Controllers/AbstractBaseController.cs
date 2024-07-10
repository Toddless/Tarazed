namespace Server.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Authorization;
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

        public AbstractBaseController(
            IDatabaseContext context,
            ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            _context = context;
            _logger = logger;
        }

        [HttpPut]
        public async virtual Task<TU?> CreateAsync(TU item)
        {
            if (item.Id != 0)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            bool isValid = item.ValidateObject(out List<ValidationResult> result);
            if (!isValid)
            {
                result.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            try
            {
                var context = _context.CheckContext();
                context.Set<TU>().Add(item);
                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(TU).Name));
                }

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
            if (item == null || item.Id == 0)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Exercise_NotFound));
            }

            bool isValid = item.ValidateObject(out List<ValidationResult> results);
            if (!isValid)
            {
                results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            try
            {
                var context = _context.CheckContext();
                context.Set<TU>().Update(item);
                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(TU).Name));
                }

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
        public virtual async Task<bool?> DeleteAsync(long? id)
        {
            if (id == null || id == 0)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            try
            {
                var context = _context.CheckContext();

                var set = context.Set<TU>();
                var existingItem = await set.FirstOrDefaultAsync(x => x.Id == id);
                if (existingItem == null)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                set.Remove(existingItem);
                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(TU).Name));
                }

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
    }
}
