namespace Server
{
    using System.Threading;
    using System.Threading.Tasks;
    using DataAccessLayer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class MyUserStore : UserStore<IdentityUser>, IUserStore<IdentityUser>
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseContext _context;

        public MyUserStore(
            DatabaseContext databaseContext,
            RoleManager<IdentityRole> roleManager,
            DatabaseContext context,
            IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {
            _context = databaseContext;
            _roleManager = roleManager;
        }

        public override async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken = default)
        {
            var result = await base.CreateAsync(user, cancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            var userRole = await _roleManager.FindByNameAsync("User");
            if (userRole == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"{userRole} not found." });
            }

            var userEntity = new IdentityUserRole<string> { UserId = user.Id, RoleId = userRole.Id };
            _context.UserRoles.Add(userEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
    }
}
