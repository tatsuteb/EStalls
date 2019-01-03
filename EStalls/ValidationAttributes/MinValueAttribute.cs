using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EStalls.ValidationAttributes
{
    public class MinValueAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly double _minValue;

        public MinValueAttribute(double minValue)
        {
            this._minValue = minValue;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new NotImplementedException();
            }

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-valuegreaterthan", this.FormatErrorMessage(context.ModelMetadata.DisplayName ?? context.ModelMetadata.PropertyName));
            MergeAttribute(context.Attributes, "data-val-valuegreaterthan-min", this._minValue.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, this._minValue);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!double.TryParse(value.ToString(), out var inputValue))
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

            return inputValue >= this._minValue
                ? ValidationResult.Success
                : new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
        }

        private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }
    }
}
