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

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase<Customer>
    {
        private readonly MyConfigKeys _configKeys;
        private readonly IUserValidator<IdentityUser> _userValidator;
        private readonly UserManager<IdentityUser> _manager;

        public CustomerController(
            IDatabaseContext context,
            IUserValidator<IdentityUser> userValidator,
            UserManager<IdentityUser> manager,
            ILogger<CustomerController> logger,
            MyConfigKeys configKeys)
            : base(context, manager, logger)
        {
            _configKeys = configKeys;
            _userValidator = userValidator;
            _manager = manager;
        }

        [HttpPut]
        [AllowAnonymous]
        public override async Task<Customer?> CreateAsync(Customer? item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.UId.HasValue)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.AlreadyExist));
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

            var user = await _manager.FindByIdAsync(id);

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

        #region NotFinished

        // [HttpPut("Something")]
        // public async Task<Customer?> ChangeRoleAsync(Guid? uid, string role)
        // {
        //    if (uid == null || uid == Guid.Empty)
        //    {
        //        throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
        //    }

        // try
        //    {
        //        var context = Context.CheckContext();

        // Customer? customer = await FindCustomerAsync(uid, context);
        //        customer!.Role = role;
        //        return customer;
        //    }
        //    catch (ServerException)
        //    {
        //        throw;
        //    }
        //    catch (Exception)
        //    {
        //        throw new InternalServerException(nameof(DataModel.Resources.Errors.InternalException));
        //    }
        // }

        #endregion

    }
}
