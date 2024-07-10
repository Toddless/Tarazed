namespace Server.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    public abstract class AbstractRelationController<TSub, TParent> : AbstractBaseController<TSub>
        where TSub : class, IEntity
        where TParent : class, IEntity
    {
        private readonly IDatabaseContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _manager;

        protected AbstractRelationController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger logger)
            : base(context, logger)
        {
            ArgumentNullException.ThrowIfNull(manager);
            _manager = manager;
            _logger = logger;
            _context = context;
        }

        public override async Task<TSub?> CreateAsync(TSub item)
        {
            var context = _context.CheckContext();
            return await context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    var currentUser = await _manager.GetUserAsync(this.User);
                    var relationId = currentUser?.Id;
                    if (relationId == null)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                    }

                    var existing = await context.Set<TParent>().FirstOrDefaultAsync(x => x.Id == parentId);
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

                    context.Set<TSub>().Add(item);
                    await context.SaveChangesAsync();
                    var mapping = Activator.CreateInstance<TRelation>();
                    mapping.CreateMapping(relationId.Value, item.Id);
                    context.Set<TRelation>().Add(mapping);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return item;
                }
                catch (ServerException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
