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
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.UId.HasValue)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.AlreadyExist));
            }

            return await base.CreateAsync(item);
        }

        public override Task<bool?> DeleteAsync(long? id)
        {
            throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
        }

        [HttpDelete("DeleteByGuid")]
        public async Task<bool?> DeleteByGuidAsync(Guid? uid)
        {
            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            try
            {
                var context = Context.CheckContext();

                Customer? customer = await FindCustomerAsync(uid, context);
                context.Customers.Remove(customer!);

                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(Customer).Name));
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

        [HttpGet]
        public async Task<Customer?> GetCustomerAsync(Guid? uid)
        {
            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            try
            {
                var context = Context.CheckContext();

                Customer? customer = await FindCustomerAsync(uid, context);
                return customer;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }

        private static async Task<Customer?> FindCustomerAsync(Guid? uid, IDatabaseContext context)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(x => x.UId == uid);
            if (customer == null)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            return customer;
        }
    }
}
