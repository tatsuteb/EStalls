using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Data.Models
{
    public class PurchasedItem
    {
        [Key]
        [Required]
        public Guid UserId { get; set; }

        [Key]
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; }
    }
}
