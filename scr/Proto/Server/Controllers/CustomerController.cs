namespace Server.Controllers
{
    using System;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Server.Filters;
    using Server.Resources;

    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase<Customer>
    {
        private readonly MyConfigKeys _configKeys;
        private readonly IUserValidator<IdentityUser> _userValidator;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseContext _context;

        public CustomerController(
            DatabaseContext context,
            IUserValidator<IdentityUser> userValidator,
            UserManager<IdentityUser> userManager,
            ILogger<CustomerController> logger,
            RoleManager<IdentityRole> roleManager,
            MyConfigKeys configKeys)
            : base(context, userManager, logger)
        {
            _context = context;
            _configKeys = configKeys;
            _userValidator = userValidator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPut]
        [AllowAnonymous]
        public override async Task<Customer?> CreateAsync(Customer? item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return await base.CreateAsync(item);
        }

        public override Task<bool?> DeleteAsync(long? id)
        {
            throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
        }

        public override async Task<Customer?> UpdateAsync(Customer item)
        {
            var x = new IdentityUser();
            x.Email = item.Email;
            IdentityResult y = await _userValidator.ValidateAsync(Manager, x);
            return null;
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

            //var user = await _manager.FindByIdAsync(id);

            return true;
            //IdentityResult s = await _manager.DeleteAsync(user);


            //if (uid == null || uid == Guid.Empty)
            //{
            //    throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            //}

            //try
            //{
            //    var context = Context.CheckContext();

            //    Customer? customer = await FindCustomerAsync(uid, context);
            //    context.Customers.Remove(customer!);

            //    var changedCount = await context.SaveChangesAsync();
            //    if (changedCount != 1)
            //    {
            //        throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(Customer).Name));
            //    }

            //    return true;
            //}
            //catch (ServerException)
            //{
            //    throw;
            //}
            //catch (InternalServerException)
            //{
            //    throw;
            //}
            //catch (Exception)
            //{
            //    throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
            //}
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

        //[HttpPut("ChangeRole")]
        //public async Task<bool> ChangeRoleAsync()
        //{
        //    var roleManager = _roleManager;
        //    var userManager = _userManager;
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        throw new ServerException("Customer not exist.");
        //    }

        //    var findNewRoleId = await roleManager.GetRoleNameAsync(newRole);
        //    if (findNewRoleId == null)
        //    {
        //        throw new ServerException("Something is wrong.");
        //    }

        //    var identityUser = await userManager.FindByIdAsync(id);
        //    if (identityUser == null)
        //    {
        //        throw new ServerException("Something is wrong.");
        //    }

        //    var userEntity = new IdentityUserRole<string> { UserId = identityUser.Id, RoleId = newRole.Id };
        //    var addingNewRole = await _context.UserRoles.AddAsync(userEntity);
        //    var changedCount = await _context.SaveChangesAsync();
        //    if (changedCount != 1)
        //    {
        //        throw new ServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(Customer).Name));
        //    }

        //    return true;

            //await strategy.ExecuteAsync(async () =>
            // {
            //     using (var transaction = await _context.Database.BeginTransactionAsync())
            //     {
            //         var userEntity = new IdentityUserRole<string> { UserId = identityUser.Id, RoleId = newRole.Id };
            //         var addingNewRole = await _context.UserRoles.AddAsync(userEntity);
            //         var changedCount = await _context.SaveChangesAsync();
            //         if (changedCount != 1)
            //         {
            //             await transaction.RollbackAsync();
            //             throw new ServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(Customer).Name));
            //         }

            //         await transaction.CommitAsync();
            //     }
            // });
        //}
    }
}
