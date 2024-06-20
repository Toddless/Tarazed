namespace DataModel
{
    using System.ComponentModel.DataAnnotations;

    public interface IEntity
    {
        [Key]
        long Id { get; set; }
    }
}
