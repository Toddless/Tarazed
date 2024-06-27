namespace DataModel
{
    using DataAccessLayer;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    public class CustomerServise : ICustomerService
    {
        private readonly IDatabaseContext _dbContext;

        public CustomerServise(IDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer> AuthenticateAsync(string email, string passwordHash)
        {
            var context = _dbContext.CheckContext();
            var customer = await context.Set<Customer>().FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                throw new ServerException(nameof(Resources.Errors.EmailOrPassword));
            }

            if (customer.PasswortHash != passwordHash)
            {
                throw new ServerException(nameof(Resources.Errors.EmailOrPassword));
            }

            return customer;
        }
    }
}
