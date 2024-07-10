namespace DataModel
{
    using System.ComponentModel.DataAnnotations;

    public interface IEntity
    {
        [Key]
        long Ids { get; set; }
    }

    // public interface IForeignKey<TOther>{

    // long ForeignKey { get; set; }

    // }
}
