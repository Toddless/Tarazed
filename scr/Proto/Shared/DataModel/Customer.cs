namespace DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

    public class Customer
    {
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsRequired))]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string UId { get; set; } = Guid.NewGuid().ToString();

        public long Id { get; set; }
    }
}
