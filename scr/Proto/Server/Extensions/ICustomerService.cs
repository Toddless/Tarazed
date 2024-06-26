namespace Server.Extensions
{
    using DataModel;

    public interface ICustomerService
    {
        Task<Customer> AuthenticateAsync(string email, string password);
    }
}
