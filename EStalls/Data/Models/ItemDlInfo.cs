using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Data.Models
{
    public class ItemDlInfo
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [StringLength(50)]
        public string Version { get; set; }

        [Required]
        public string DlFileNames { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RegistrationTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UpdateTime { get; set; }
    }
}
