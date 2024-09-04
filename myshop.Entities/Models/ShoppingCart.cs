using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myshop.Entities.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int productId { get; set; }
        [ForeignKey("productId")]
        [ValidateNever]
        public Product Product { get; set; }

        [Range(1, 100, ErrorMessage = "You must enter a value between 1 and 100")]
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
