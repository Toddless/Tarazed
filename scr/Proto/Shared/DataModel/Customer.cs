namespace DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Customer
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswortHash { get; set; } = string.Empty;

        public Guid? UId
        {
            get; set;
        }

        [Key]
        public long Id
        {
            get; set;
        }
    }
}
