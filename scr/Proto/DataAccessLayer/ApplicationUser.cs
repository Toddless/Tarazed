namespace DataAccessLayer
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DataModel;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IEntity
    {
        [Key, Column(Order = 1)]
        public long Ids { get; set; }
    }
}
