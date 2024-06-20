namespace DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

    public class Customer : IEntity
    {
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.NameIsRequired))]
        [MinLength(5, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.NameIsToShort))]
        [MaxLength(50, ErrorMessageResourceType =typeof(Errors), ErrorMessage = nameof(Errors.NameIsToLong))]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType =typeof(Errors), ErrorMessage = nameof(Errors.Customer_EmailIsRequired))]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.Customer_Password))]
        public string PasswortHash { get; set; } = string.Empty;

        public Guid? UId { get; set; }

        [Key]
        public long Id { get; set; }
    }
}
