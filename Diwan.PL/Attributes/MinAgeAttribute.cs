using System.ComponentModel.DataAnnotations;

namespace Diwan.PL.Attributes
{
    public class MinAgeAttribute:ValidationAttribute
    {
        private readonly int _minAge;
        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }
        protected override ValidationResult? IsValid(object? date, ValidationContext validationContext)
        {
            if (date is null)
            {
                return new ValidationResult("Date Of Birth is Required!");
            }

            DateTime Data;
            try
            {
                Data = (DateTime)date;
            }
            catch
            {
                return new ValidationResult("Invalid Data Of Birth!");
            }

            int age = DateTime.Today.Year - Data.Year;
            if(Data > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            if (age < _minAge)
            {
                return new ValidationResult($"You must be at least {_minAge} years old!");
            }
            return ValidationResult.Success;
        }
    }
}
