using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Data.Models
{
    public class Item
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [Required]
        public string PreviewFileNames { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegistrationTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }
    }
}
