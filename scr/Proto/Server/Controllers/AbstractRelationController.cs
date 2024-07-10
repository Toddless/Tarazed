namespace Server.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Server.Extensions;
    using Server.Filters;

    public abstract class AbstractRelationController<TSub, TParent> : AbstractBaseController<TSub>
        where TSub : class, IEntity
        where TParent : class, IEntity
    {
        private readonly IDatabaseContext _context;
        private readonly ILogger _logger;

        protected AbstractRelationController(
            IDatabaseContext context,
            ILogger logger)
            : base(context, logger)
        {
            _logger = logger;
            _context = context;
        }

        public override async Task<TSub?> CreateAsync(TSub item)
        {
            var context = _context.CheckContext();

            try
            {
                bool isValid = item.ValidateObject(out List<ValidationResult> result);
                if (!isValid)
                {
                    result.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                context.Set<TSub>().Add(item);
                await context.SaveChangesAsync();
                return item;
            }
            catch (ServerException)
            {
                throw;
            }
        }
    }
}
