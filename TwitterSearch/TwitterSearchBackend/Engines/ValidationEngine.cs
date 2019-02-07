using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend.Engines
{
    public interface IValidationEngine : ITwitterSearchServiceBase
    {
        ValidationResult ValidateSearchText(string searchText);
    }

    public class ValidationEngine : TwitterSearchServiceBase, IValidationEngine
    {
        public ValidationResult ValidateSearchText(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new ValidationResult(ErrorCodes.InvalidSearchParameter);
            //potential future validation failures

            return new ValidationResult(true);
        }
    }
}
