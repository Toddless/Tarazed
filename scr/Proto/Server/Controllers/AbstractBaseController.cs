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
    [Produces("application/json")]
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
            ArgumentNullException.ThrowIfNull(manager);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            _manager = manager;
            _context = context;
            _logger = logger;
        }

        protected IDatabaseContext Context
        {
            get { return _context; }
        }

        protected ILogger Logger
        {
            get { return _logger; }
        }

        protected UserManager<ApplicationUser> Manager
        {
            get { return _manager; }
        }

        /// <summary>
        ///  Retrieves a collection of items of type <typeparamref name="TU"/> associated with the current user.
        ///  Optionally filters the items by the provided IDs and includes additional data if specified.
        /// </summary>
        /// <param name="ids"> A collection of IDs representing the items to retrieve. If <see langword="null"/>, retrieves all items belonging to the current user.</param>
        /// <param name="loadAdditionalData">A boolean flag indicating whether to include additional related data with each item. Defaults to <see langword="false"/>.</param>
        /// <returns>The task result is an <see cref="IEnumerable{TU}"/> containing
        /// the items associated with the current user. Returns <c>null</c> if no items are found.</returns>
        /// <exception cref="InternalServerException">Thrown if an unexpected error occurs during the operation.</exception>
        [HttpGet]
        public async virtual Task<IEnumerable<TU>?> GetAsync(IEnumerable<long>? ids, bool loadAdditionalData = false)
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
                // falls null eingegeben wurde, gibt alle items des currentsUsers zurück
                IQueryable<TU> query = context.Set<TU>().Where(o => o.CustomerId == currentUser.Id);

                if (ids?.Any() ?? false)
                {
                    query = query.Where(o => ids.Contains(o.Id));
                }

                if (loadAdditionalData)
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

        /// <summary>
        /// Creates a new instance of <typeparamref name="TU"/> and saves it to the database.
        /// Validates the item before creation and assigns the current user's ID as the CustomerId.
        /// </summary>
        /// <param name="item"> The item to create. Must not be null and must have an unset primary key (Id = 0).</param>
        /// <returns>The task result contains the created item of type <typeparamref name="TU"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the provided item is null.</exception>
        /// <exception cref="InternalServerException">Thrown if an unexpected error occurs during the operation.</exception>
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

        /// <summary>
        ///  Updates an existing instance of <typeparamref name="TU"/> in the database.
        /// </summary>
        /// <param name="item"> The item to update. Must not be null and must have a valid primary key (Id > 0).</param>
        /// <returns>The task result contains the updated item of type <typeparamref name="TU"/>.</returns>
        /// <exception cref="InternalServerException">Thrown if an unexpected error occurs during the operation.</exception>
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
                    .Where(o => o.CustomerId == currentUser!.Id && item.Id == o.Id)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (itemExists == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.ElementNotExists);
                }

                item.CustomerId = currentUser!.Id;

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

        /// <summary>
        /// Deletes an existing instance of <typeparamref name="TU"/> from the database based on the provided identifier.
        /// </summary>
        /// <param name="id">The identifier of the item to delete. Must not be null or zero.</param>
        /// <returns>The task result is <see langword="true"/> if the item was successfully deleted.</returns>
        /// <exception cref="InternalServerException">Thrown if an unexpected error occurs during the operation.</exception>
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
