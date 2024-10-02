namespace Server
{
    using DataAccessLayer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class MyUserStore : UserStore<ApplicationUser>, IUserStore<ApplicationUser>
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

        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var result = await base.CreateAsync(user, cancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            var roleName = "User";
            var userRole = await _roleManager.FindByNameAsync(roleName);
            if (userRole == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"{roleName} role not found." });
            }

            var userEntity = new IdentityUserRole<string> { UserId = user.Id, RoleId = userRole.Id };
            _context.UserRoles.Add(userEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

// im fall wenn man rolle braucht, muss man das ins migration hinzugefügt werden und auch MyUserStore im Startup wieder eingeschaltet.
//   migrationBuilder.InsertData(
//       table: "AspNetRoles",
//       columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
//       values: new object[] { "1", "Admin", "Admin", "a79ca8c5-7ecf-489d-a292-a99c2fd88717" });
//   migrationBuilder.InsertData(
//       table: "AspNetRoles",
//       columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
//       values: new object[] { "2", "User", "User", "42e40409-d827-445a-8075-fc93a3d7f210" });
    }
}
