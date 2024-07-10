namespace Server.Controllers
{
    using System;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Server.Filters;
    using Server.Resources;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController
        : AbstractBaseController<ApplicationUser>
    {
        private readonly MyConfigKeys _configKeys;
        private readonly IUserValidator<ApplicationUser> _userValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseContext _context;

        public CustomerController(
            DatabaseContext context,
            IUserValidator<ApplicationUser> userValidator,
            UserManager<ApplicationUser> userManager,
            ILogger<CustomerController> logger,
            RoleManager<IdentityRole> roleManager,
            MyConfigKeys configKeys)
            : base(context, logger)
        {
            _context = context;
            _configKeys = configKeys;
            _userValidator = userValidator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpDelete("Delete by Id")]
        public override Task<bool?> DeleteAsync(long? id)
        {
            throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
        }

        public override async Task<ApplicationUser?> UpdateAsync(ApplicationUser item)
        {
            var x = await _userManager.GetUserAsync(User);
            if (x == null)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.ContextNotSet));
            }

            if (x.Ids != item.Ids)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
            }

            x.Email = item.Email;
            IdentityResult y = await _userValidator.ValidateAsync(_userManager, x);
            var result = base.UpdateAsync(x);
            return item;
        }

        [HttpDelete("DeleteByGuid")]
        public async Task<bool?> DeleteByGuidAsync(string id)
        {
            if (!ModelState.IsValid)
            {
                throw new ServerException("Something go wrong");
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ServerException("Wrong or empty Id");
            }

            if (User.IsInRole("Admin"))
            {
                throw new ServerException("This user cannot be deleted");
            }

            return true;
        }

        [HttpGet]
        public async Task<Customer?> GetCustomerAsync()
        {
            try
            {
                return new Customer();
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
