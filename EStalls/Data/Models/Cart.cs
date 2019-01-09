using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Data.Models
{
    public class Cart
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid Uid { get; set; }
    }
}
