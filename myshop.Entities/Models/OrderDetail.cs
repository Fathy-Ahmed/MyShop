
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace myshop.Entities.Models
{
	public class OrderDetail
	{
		public int Id { get; set; }
		public int OrderHeaderId { get; set; }
		[ValidateNever]
		[ForeignKey("OrderHeaderId")]	
		public OrderHeader OrderHeader { get; set; }

		public int ProductId { get; set; }
		[ValidateNever]
		public Product Product { get; set; }

		public int Count { get; set; }
		public decimal Price { get; set; }

	}
}
