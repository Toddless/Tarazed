namespace Server
{
    using System.Threading;
    using System.Threading.Tasks;
    using DataAccessLayer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class UserStore : UserStore<IdentityUser>, IUserStore<IdentityUser>
    {
        public UserStore(DatabaseContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {
        }

        public override Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken = default)
        {
            return base.CreateAsync(user, cancellationToken);
        }
    }
}
