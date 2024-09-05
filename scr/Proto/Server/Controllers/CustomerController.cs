namespace Server.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Server.Extensions;
    using Server.Filters;

    public class CustomerController
        : Controller
    {
        private readonly UserManager<ApplicationUser> _manager;
        private readonly ILogger<CustomerController> _logger;
        private readonly IDatabaseContext _context;

        public CustomerController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<CustomerController> logger)
        {
            _logger = logger;
            _context = context;
            _manager = manager;
        }

        [HttpPut("UpdateCustomer")]
        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            if (User == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NullObject);
            }

            if (customer == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NullObject);
            }

            var currentUser = await _manager.GetUserAsync(User) !;
            if (currentUser == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NotFound);
            }

            if (currentUser.Id != customer.UId)
            {
                throw new ServerException(DataModel.Resources.Errors.DeletingById);
            }

            currentUser.Email = customer.Email;
            try
            {
                bool isValid = currentUser.ValidateObject(out List<ValidationResult> results);
                if (!isValid)
                {
                    results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest);
                }

                var result = await _manager.UpdateAsync(currentUser);
                if (!result.Succeeded)
                {
                    throw new ServerException(DataModel.Resources.Errors.InvalidRequest);
                }
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(UpdateCustomerAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }

            customer.Email = currentUser?.Email ?? string.Empty;
            return customer;
        }

        [HttpDelete("DeleteCustomer")]
        public async Task<bool?> DeleteCustomerAsync()
        {
            if (User == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NullObject);
            }

            if (User.IsInRole("Admin"))
            {
                throw new ServerException("This user cannot be deleted");
            }

            var user = await _manager.GetUserAsync(User);
            if (user == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NullObject);
            }

            try
            {
                var context = _context.CheckContext();

                var set = context.Set<ApplicationUser>();
                set.Remove(user);
                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(DataModel.Resources.Errors.NotSaved, typeof(ApplicationUser).Name));
                }

                return true;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(DeleteCustomerAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }
        }

        [HttpGet("GetCustomer")]
        public async Task<Customer?> GetCustomerAsync()
        {
            try
            {
                if (User == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.NullObject);
                }

                var user = await _manager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ServerException(DataModel.Resources.Errors.NullObject);
                }

                return new Customer()
                {
                    Email = user?.Email ?? string.Empty,
                    UId = user?.Id ?? string.Empty,
                };
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogException(this, ex);
                _logger.LogError(ex, $"Unknown Error in {nameof(GetCustomerAsync)}.");
                throw new InternalServerException(DataModel.Resources.Errors.InternalException);
            }
        }
    }
}
