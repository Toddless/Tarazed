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
        private readonly IUserValidator<ApplicationUser> _userValidator;
        private readonly UserManager<ApplicationUser> _manager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CustomerController> _logger;
        private readonly DatabaseContext _context;

        public CustomerController(
            DatabaseContext context,
            IUserValidator<ApplicationUser> userValidator,
            UserManager<ApplicationUser> manager,
            ILogger<CustomerController> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userValidator = userValidator;
            _manager = manager;
            _roleManager = roleManager;
        }

        [HttpPut("UpdateCustomer")]
        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            var currentUser = await _manager.GetUserAsync(User)!;
            if (currentUser == null)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.NotFound));
            }

            if (currentUser.Id != customer.UId)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
            }

            currentUser.Email = customer.Email;
            try
            {
                bool isValid = currentUser.ValidateObject(out List<ValidationResult> results);
                if (!isValid)
                {
                    results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                var result = await _manager.UpdateAsync(currentUser);
                if (!result.Succeeded)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }
            }
            catch (InternalServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }

            customer.Email = currentUser?.Email ?? string.Empty;
            return customer;
        }

        [HttpDelete("DeleteCustomer")]
        public async Task<bool?> DeleteCustomerAsync()
        {
            if (User.IsInRole("Admin"))
            {
                throw new ServerException("This user cannot be deleted");
            }

            var user = await _manager.GetUserAsync(User);
            if (user == null)
            {
                throw new ServerException("This user cannot be deleted");
            }

            try
            {
                var context = _context.CheckContext();

                var set = context.Set<ApplicationUser>();
                set.Remove(user);
                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(ApplicationUser).Name));
                }

                return true;
            }
            catch (ServerException)
            {
                throw;
            }
            catch (InternalServerException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }

        [HttpGet("GetCustomer")]
        public async Task<Customer?> GetCustomerAsync()
        {
            try
            {
                var user = await _manager.GetUserAsync(User);
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
            catch (Exception)
            {
                throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            }
        }
    }
}
