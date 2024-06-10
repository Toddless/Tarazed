namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IDatabaseContext _context;

        public CustomerController(IDatabaseContext databaseContext)
        {
            ArgumentNullException.ThrowIfNull(databaseContext);
            this._context = databaseContext;
        }

        [HttpPut()]
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            this._context.Customers.Add(customer);
            await this._context.SaveChangesAsync();
            return customer;
        }
    }
}
