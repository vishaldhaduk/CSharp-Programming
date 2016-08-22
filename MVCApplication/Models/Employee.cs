using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVCApplication.Models
{
    public class FirstNameValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validatinContext)
        {
            if (value == null)
                return new ValidationResult("Please Give First Name");
            else
                if (value.ToString().Contains("@"))
                    return new ValidationResult("First Name should not contain @");

            return ValidationResult.Success;
        }
    }

    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [FirstNameValidation]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Salary { get; set; }
    }
}