using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EStalls.Data.Models
{
    public class OrderItem
    {
        [Key]
        [Required]
        public Guid OrderId { get; set; }

        [Key]
        [Required]
        public Guid ItemId { get; set; }

        /// <summary>
        /// 注文時点での作品価格
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// 注文時の作品タイトル
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 注文時の販売者表示名
        /// </summary>
        [Required]
        public string SellerDisplayName { get; set; }
    }
}
