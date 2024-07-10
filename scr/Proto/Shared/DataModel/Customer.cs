namespace DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

    public class Customer : IEntity
    {
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.Customer_EmailIsRequired))]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string UId { get; set; } = Guid.NewGuid().ToString();

        public long Ids { get; set; }
    }
}
