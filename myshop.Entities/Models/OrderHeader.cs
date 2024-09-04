
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }
		public decimal OrderTotalPrice { get; set; }

		
		// Data of user
		public string ApplicationUserId { get; set; }
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		[DataType(DataType.PhoneNumber)]
		public string? Phone { get; set; }


		public DateTime OrderDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public DateTime? PaymentDate { get; set; }

		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public string? TrackingNumber { get; set; }
		public string? Carrier { get; set; }

		//  Stripe Properties
		public string? SessionId { get; set; }
		public string? PaymentIntendId { get; set; }

		


	}
}
