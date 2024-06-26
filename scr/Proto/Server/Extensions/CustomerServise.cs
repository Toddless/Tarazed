namespace DataModel
{
    using DataAccessLayer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    public class CustomerServise : ICustomerService
    {
        private readonly IDatabaseContext _dbContext;
        private readonly IPasswordHasher<Customer> _passwordHasher;

        public CustomerServise(IDatabaseContext dbContext, IPasswordHasher<Customer> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Customer> AuthenticateAsync(string email, string password)
        {
            var customer = await _dbContext.Set<Customer>().FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null)
            {
                throw new ServerException(nameof(Resources.Errors.EmailOrPassword));
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(customer, customer.PasswortHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new ServerException(nameof(Resources.Errors.EmailOrPassword));
            }

            return customer;
        }
    }
}
