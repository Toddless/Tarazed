namespace DataModel
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
