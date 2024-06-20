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
            if (item.UId.HasValue)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.));
            }

            item.UId = Guid.NewGuid();
            return await base.CreateAsync(item);
        }

        public override Task<bool?> DeleteAsync(long? id)
        {
            throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
        }

        [HttpDelete]
        public async Task<bool?> DeleteByGuidAsync(Guid? uid)
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

            return true;
        }

        [HttpGet]
        public async Task<Customer?> GetCustomerAsync(Guid? uid)
        {
            var context = Context.CheckContext();

            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            Customer? customer = await FindCustomerAsync(uid, context);
            return customer;
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
