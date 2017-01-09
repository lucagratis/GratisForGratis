using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.DataAnnotations
{
    public class RequiredIfNotNullAttribute : RequiredAttribute
    {
        private String[] PropertyName { get; set; }

        public RequiredIfNotNullAttribute(params String[] propertyName)
        {
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            Object instance = context.ObjectInstance;
            Type type = instance.GetType();
            foreach (string property in PropertyName)
            {
                Object propertyvalue = type.GetProperty(property).GetValue(instance, null);
                if (propertyvalue != null && !string.IsNullOrWhiteSpace(propertyvalue.ToString()))
                {
                    ValidationResult result = base.IsValid(value, context);
                    if (result != ValidationResult.Success)
                        return result;
                }
            }
            return ValidationResult.Success;
        }
    }
}
