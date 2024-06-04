using System;
using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class Customer
    {
        public String Name
        {
            get; set;
        }

        public string Email
        {
            get; set;
        }

        public string PasswortHash
        {
            get; set;
        }

        public Guid UId
        {
            get; set;
        }
        [Key]
        public long Id 
        {
            get => default;
            set
            {
            }
        }
    }
}
