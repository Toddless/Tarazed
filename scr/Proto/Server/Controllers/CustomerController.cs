namespace Server.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IDatabaseContext? _context;
        private readonly ILogger? _logger;

        public CustomerController(IDatabaseContext databaseContext, ILogger<CustomerController> logger)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);
            ArgumentNullException.ThrowIfNull(logger);
            _context = databaseContext;
            _logger = logger;
        }

        [HttpPut("{name}, {email}, {pass}")]
        public async Task<Customer?> CreateCustomersAsync(string? name, string? email, string? pass)
        {
            var context = _context;
            if (context == null)
            {
                _logger?.LogDebug("Context not set");
                return null;
            }

            var customer = new Customer
            {
                Name = name ?? string.Empty,
                Email = email ?? string.Empty,
                PasswortHash = pass ?? string.Empty,
                UId = Guid.NewGuid(),
            };

            var validationContext = new ValidationContext(customer, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(customer, validationContext, results, true);

            if (!isValid)
            {
                results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                return null;
            }

            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            return customer;
        }

        [HttpDelete("{uid}")]
        public async Task<bool?> DeleteCustomerAsync(Guid? uid)
        {
            var context = _context;
            if (context == null)
            {
                _logger?.LogDebug("Context not set");
                return null;
            }

            if (uid == null || uid == Guid.Empty)
            {
                _logger?.LogError($"Customer with uId {uid} not found");
                throw new Exception("Id not found");
            }

            var customer = await context.Customers.FirstOrDefaultAsync(x => x.UId == uid);
            if (customer == null)
            {
                return null;
            }

            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
            return true;
        }

        // [HttpGet("{name}")]
        // public async Task<long?> GetCustomerAsync(Guid? guid)
        // {
        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        throw new Exception("User not found");
        //    }

        // var customer = await _context.Customers.FirstOrDefaultAsync(x => (x.UId ?? Guid.Empty).ToString("D", CultureInfo.InvariantCulture) == (guid ?? Guid.Empty).ToString("D", CultureInfo.InvariantCulture));
        //    return customer?.Id;
        // }
    }
}
