using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.CustomValidation
{
    public class IdentityPasswordValidation:ValidationAttribute
    {
        protected override ValidationResult IsValid(object password, ValidationContext validationContext)
        {
            if(password==null) return new ValidationResult("Password is Required");
            var strValue = password.ToString();

            if (!Regex.IsMatch(strValue, @"\d"))
            {
                return new ValidationResult("Password must contain at least one digit.");
            }

            if (!strValue.Any(char.IsUpper))
            {
                return new ValidationResult("Password must contain at least one uppercase letter.");
            }

            if (!strValue.Any(char.IsLower))
            {
                return new ValidationResult("Password must contain at least one lowercase letter.");
            }


            return ValidationResult.Success;
        }
    }
}
