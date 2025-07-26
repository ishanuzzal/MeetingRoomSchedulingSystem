using System.ComponentModel.DataAnnotations;

namespace Meeting_Scheduling_System.CustomValidation
{
    public class UppercaseSingleLetter : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("The field must have a value");

            var strValue = value.ToString();
            int uppercaseCount = strValue.Count(char.IsUpper);

            if (uppercaseCount != 1)
            {
                return new ValidationResult("The string must contain exactly one uppercase letter.");
            }

            return ValidationResult.Success;
        }
    }


}
