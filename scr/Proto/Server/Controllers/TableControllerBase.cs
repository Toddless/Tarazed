namespace Server.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    public class TableControllerBase<TU, TV, TW> : Controller
        where TU : class, IEntity
        where TV : class, IEntity
        where TW : class, IMappingEntity
    {

        public TableControllerBase()
        {
        }

        protected IDatabaseContext Context { get; private set; }

        protected ILogger Logger { get; private set; }

        [HttpPut]
        public virtual async Task<TU?> CreateAsync(TU item, long relationId)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existing = await Context.Set<TV>().FirstOrDefaultAsync(x => x.Id == relationId);
                    var context = Context.CheckContext();
                    if (item.Id != 0)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                    }

                    bool isValid = item.ValidateObject(out List<ValidationResult> result);
                    if (!isValid)
                    {
                        result.ForEach(x => Logger?.LogDebug(x.ErrorMessage));
                        throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                    }

                    context.Set<TU>().Add(item);
                    var mapping = Activator.CreateInstance<TW>();
                    mapping.CreateMapping(relationId, item.Id);
                    Context.Set<TW>().Add(mapping);
                    await context.SaveChangesAsync();
                    return item;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // log
                    throw ex;
                }
            }
        }

        public virtual async Task<TU> DeleteAsync (TU item, long relationId)
        {
            using (var transaction = await Context.Database.BeginTransactionAsync())
            {

            }
        }
    }
}
