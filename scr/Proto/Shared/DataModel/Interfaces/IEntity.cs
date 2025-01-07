namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public interface IEntity
    {
        [Key]
        long Id { get; set; }

        [ForeignKey("AspNetUsers")]
        string CustomerId { get; set; }
    }
}
