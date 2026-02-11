using System;
using System.ComponentModel.DataAnnotations;

namespace UsalClinic.Web.Validation
{
    public class PastDateAttribute : ValidationAttribute
    {
        public PastDateAttribute()
        {
            ErrorMessage = "Date must be in the past.";
        }

        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date < DateTime.Today;
            }
            return true; 
        }
    }
}
