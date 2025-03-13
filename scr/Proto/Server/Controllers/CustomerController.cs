namespace Server.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Server.Extensions;
    using Server.Filters;

    [Authorize]
    [Produces("application/json")]
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
            ArgumentNullException.ThrowIfNull(_manager);
            ArgumentNullException.ThrowIfNull(_logger);
            ArgumentNullException.ThrowIfNull(_context);
            _logger = logger;
            _context = context;
            _manager = manager;
        }

        [HttpPut("UpdateCustomer")]
        public async Task<Customer?> UpdateCustomerAsync([FromBody]Customer customer)
        {
            var body = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            if (User == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NullObject);
            }

            if (customer == null)
            {
                throw new ServerException(DataModel.Resources.Errors.NullObject);
            }

            var currentUser = await _manager.GetUserAsync(User) ?? throw new ServerException(DataModel.Resources.Errors.NotFound);
            if (currentUser.Id != customer.UId)
            {
                throw new ServerException(DataModel.Resources.Errors.DeletingById);
            }

            currentUser.UserName = customer.Email;
            currentUser.Email = customer.Email;
            try
            {
                bool isValid = ValidateObject(customer, out List<ValidationResult> results);
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

            var user = await _manager.GetUserAsync(User) ?? throw new ServerException(DataModel.Resources.Errors.NullObject);
            try
            {
                var set = _context.Set<ApplicationUser>();
                set.Remove(user);
                var changedCount = await _context.SaveChangesAsync();
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

                var user = await _manager.GetUserAsync(User) ?? throw new ServerException(DataModel.Resources.Errors.NullObject);

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

        private static bool ValidateObject(object obj, out List<ValidationResult> results)
        {
            ArgumentNullException.ThrowIfNull(obj);

            var validationContext = new ValidationContext(obj, serviceProvider: null, items: null);
            results = new();
            return Validator.TryValidateObject(obj, validationContext, results, true);
        }
    }
}
