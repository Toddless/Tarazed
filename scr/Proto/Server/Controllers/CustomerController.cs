namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Server.Filters;

    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class CustomerController : Controller
    {
        private readonly IDatabaseContext _context;

        public CustomerController(IDatabaseContext databaseContext)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);
            _context = databaseContext;
        }

        [HttpPut()]
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
