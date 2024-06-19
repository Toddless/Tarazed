namespace Server.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IDatabaseContext? _context;
        private readonly ILogger? _logger;

        public CustomerController(IDatabaseContext context, ILogger<CustomerController> logger)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            _context = context;
            _logger = logger;
        }

        [HttpPut("{name}, {email}, {pass}")]
        public async Task<Customer?> CreateCustomersAsync(string? name, string? email, string? pass)
        {
            // mögliches null
            var context = _context.CheckContext();

            var customer = new Customer
            {
                Name = name ?? string.Empty,
                Email = email ?? string.Empty,
                PasswortHash = pass ?? string.Empty,
                UId = Guid.NewGuid(),
            };

            bool isValid = IValidateObjectExtension.ValidateObject(customer, out List<ValidationResult> results);
            if (!isValid)
            {
                results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        [HttpDelete("{uid}")]
        public async Task<bool?> DeleteCustomerAsync(Guid? uid)
        {
            var context = _context.CheckContext();

            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            // mögliches null
            Customer? customer = await FindCustomerAsync(uid, context);
            context.Customers.Remove(customer);

            var changedCount = await context.SaveChangesAsync();
            if (changedCount != 1)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotDeleted));
            }

            return true;
        }

        [HttpGet("{uid}")]
        public async Task<Customer> GetCustomerAsync(Guid? uid)
        {
            var context = _context.CheckContext();

            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            // mögliches null
            Customer? customer = await FindCustomerAsync(uid, context);
            return customer;
        }

        [HttpPost]
        public async Task<Customer> UpdateCustomerAsync(Customer? customer)
        {
            var context = _context.CheckContext();

            if (customer == null || customer.Id == 0L)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            bool isValid = IValidateObjectExtension.ValidateObject(customer, out List<ValidationResult> results);
            if (!isValid)
            {
                results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            context.Customers.Update(customer!);
            var changedCount = await context.SaveChangesAsync();
            if (changedCount != 1)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            _logger?.LogInformation($"Customer with uId {customer.UId} changed his name and email to {customer.Name} , {customer.Email}.");
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

        // private static bool ValidateUser(Customer customer, out List<ValidationResult> results)
        // {
        //    var validationContext = new ValidationContext(customer, serviceProvider: null, items: null);
        //    results = new List<ValidationResult>();
        //    return Validator.TryValidateObject(customer, validationContext, results, true);
        // }

        // private IDatabaseContext CheckContext()
        // {
        //    var context = _context;
        //    if (context == null)
        //    {
        //        throw new ServerException(nameof(DataModel.Resources.Errors.ContextNotSet));
        //    }

        // return context;
        // }
    }
}
