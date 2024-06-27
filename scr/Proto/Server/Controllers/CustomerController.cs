namespace Server.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Server.Extensions;
    using Server.Filters;
    using Server.Resources;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase<Customer>
    {
        private readonly ICustomerService _customerService;
        private readonly MyConfigKeys _configKeys;

        public CustomerController(IDatabaseContext context, ILogger<CustomerController> logger, IConfiguration configuration, ICustomerService customerService, MyConfigKeys configKeys)
            : base(context, logger)
        {
            _customerService = customerService;
            _configKeys = configKeys;
        }

        [HttpPut]
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

            if (item.UId == null || item.UId == Guid.Empty)
            {
                item.UId = Guid.NewGuid();
            }

            return await base.CreateAsync(item);
        }

        public override Task<bool?> DeleteAsync(long? id)
        {
            throw new ServerException(nameof(DataModel.Resources.Errors.DeletingById));
        }

        [HttpDelete("DeleteByGuid")]
        public async Task<bool?> DeleteByGuidAsync(Guid? uid)
        {
            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            try
            {
                var context = Context.CheckContext();

                Customer? customer = await FindCustomerAsync(uid, context);
                context.Customers.Remove(customer!);

                var changedCount = await context.SaveChangesAsync();
                if (changedCount != 1)
                {
                    throw new InternalServerException(string.Format(nameof(DataModel.Resources.Errors.NotSaved), typeof(Customer).Name));
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

        [HttpGet]
        public async Task<Customer?> GetCustomerAsync(Guid? uid)
        {
            if (uid == null || uid == Guid.Empty)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            try
            {
                var context = Context.CheckContext();

                Customer? customer = await FindCustomerAsync(uid, context);
                return customer;
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
        //[HttpPut("Something")]
        //public async Task<Customer?> ChangeRoleAsync(Guid? uid, string role)
        //{
        //    if (uid == null || uid == Guid.Empty)
        //    {
        //        throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
        //    }

        //    try
        //    {
        //        var context = Context.CheckContext();

        //        Customer? customer = await FindCustomerAsync(uid, context);
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
        //}

        #endregion

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            var customerService = _customerService;
            var configKeys = _configKeys;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await customerService.AuthenticateAsync(model.UserEmail, model.Password);

            if (customer == null)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.EmailOrPassword));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKeys.JWTKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var sectoken = new JwtSecurityToken(
                configKeys.JWTIssuer,
                configKeys.JWTIssuer,
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(sectoken);

            return Ok(token);
        }

        private static async Task<Customer?> FindCustomerAsync(Guid? uid, IDatabaseContext context)
        {
            var customer = await context.Customers.FirstOrDefaultAsync(x => x.UId == uid);
            if (customer == null)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Customer_NotFound));
            }

            return customer;
        }
    }
}
