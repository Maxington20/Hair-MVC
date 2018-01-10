using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hair.Models
{
	[ModelMetadataTypeAttribute(typeof(AppointmentDetailsMetadata))]

	public partial class AppointmentDetail : IValidatableObject
	{
		HairContext _context = HairContext.Context;

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (_context.Appointment.Where(x=> x.AppointmentId == this.AppointmentId).Any(x => x.Completed) == true)
			{
				//write all of this crap down
				//first one is error message. second one is the field where the error message is displayed
				//if no second thing is there, it will go to summary (validation summary at top of form)
				yield return new ValidationResult("The Appointment is Closed", new[] { "AppointmentId" });
			}

			if (_context.Product.Any(x => x.ProductId == this.ProductId) == false)
			{
				yield return new ValidationResult("This is not a valid product", new[] { "ProductId" });
			}

			if (_context.Appointment.Any(x => x.AppointmentId == this.AppointmentId) == false)
			{
				yield return new ValidationResult("This appointment does not exist", new[] { "AppointmentId" });
			}

			if (Quantity < 1)
			{
				yield return new ValidationResult("The Quantity cannot be 0", new[] { "Quantity" });
			}

			if (Discount < 0 || Discount > 0.5)
			{
				yield return new ValidationResult("The Discount cannot be less than 0 or greater than 0.5", new[] { "Discount" });
			}

			//_context.Product.SingleOrDefault(x => x.RetailPrice).Where(x => x.ProductId == this.ProductId));
			


			Total = (RetailPrice * Quantity) - (RetailPrice * Quantity * Discount);

			yield return ValidationResult.Success;

			//throw new NotImplementedException();
		}
	}

	public partial class AppointmentDetailsMetadata
	{
		public int AppointmentDetailId { get; set; }
		public int AppointmentId { get; set; }
		public int ProductId { get; set; }
		public int ProcedureMinutes { get; set; }
		public double RetailPrice { get; set; }
		public int Quantity { get; set; }
		public double Discount { get; set; }
		[DisplayFormat(DataFormatString = "{0:c}")]
		public double Total { get; set; }
		public string Comments { get; set; }

		public Appointment Appointment { get; set; }
		public Product Product { get; set; }
	}
}
