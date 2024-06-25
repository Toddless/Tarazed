namespace Server.Controllers
{
    using System;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase<Customer>
    {
        public CustomerController(IDatabaseContext context, ILogger<CustomerController> logger)
            : base(context, logger)
        {
        }

        [HttpPut]
        public override async Task<Customer?> CreateAsync(Customer? item)
        {
            using (var transaktion = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (item == null)
                    {
                        throw new ArgumentNullException(nameof(item));
                    }

                    if (item.UId.HasValue)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.AlreadyExist));
                    }

                    item.UId = Guid.NewGuid();
                    await transaktion.CommitAsync();
                    return await base.CreateAsync(item);
                }
                catch (Exception)
                {
                    await transaktion.RollbackAsync();
                    throw new ServerException(nameof(DataModel.Resources.Errors.InternalException));
                }
            }
        }

        public override Task<bool?> DeleteAsync(long? id)
        {
            throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
        }

        [HttpDelete("DeleteByGuid")]
        public async Task<bool?> DeleteByGuidAsync(Guid? uid)
        {
            using (var transaktion = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var context = Context.CheckContext();

                    if (uid == null || uid == Guid.Empty)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
                    }

                    Customer? customer = await FindCustomerAsync(uid, context);
                    context.Customers.Remove(customer!);

                    var changedCount = await context.SaveChangesAsync();
                    if (changedCount != 1)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotDeleted));
                    }

                    await transaktion.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    await transaktion.RollbackAsync();
                    throw new ServerException(nameof(DataModel.Resources.Errors.InternalException));
                }
            }
        }

        [HttpGet]
        public async Task<Customer?> GetCustomerAsync(Guid? uid)
        {
            using (var transaktion = await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var context = Context.CheckContext();

                    if (uid == null || uid == Guid.Empty)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
                    }

                    Customer? customer = await FindCustomerAsync(uid, context);
                    await transaktion.CommitAsync();
                    return customer;
                }
                catch (Exception)
                {
                    await transaktion.RollbackAsync();
                    throw new ServerException(nameof(DataModel.Resources.Errors.InternalException));
                }
            }
        }

        private static async Task<Customer?> FindCustomerAsync(Guid? uid, IDatabaseContext context)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var customer = await context.Customers.FirstOrDefaultAsync(x => x.UId == uid);
                    if (customer == null)
                    {
                        throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
                    }

                    await transaction.CommitAsync();
                    return customer;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw new ServerException(nameof(DataModel.Resources.Errors.InternalException));
                }
            }
        }
    }
}
