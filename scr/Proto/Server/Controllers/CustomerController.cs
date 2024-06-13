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
            _context = databaseContext;
        }

        [HttpPut()]
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            throw new NotImplementedException();
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        [HttpPut("{name}, {email}")]
        public async Task<Customer> CreateCustomersAsync(Customer customer, string name, string email)
        {
            customer.Name = name;
            customer.Email = email;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        [HttpDelete("{id}")]
        public async Task<Customer> DeleteCustomerAsync(Customer customer, int id)
        {
            customer.Id = id;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
