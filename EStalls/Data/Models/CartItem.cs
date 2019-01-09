using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Data.Models
{
    public class CartItem
    {
        [Key]
        [Required]
        public Guid CartId { get; set; }

        [Key]
        [Required]
        public Guid ItemId { get; set; }
    }
}
