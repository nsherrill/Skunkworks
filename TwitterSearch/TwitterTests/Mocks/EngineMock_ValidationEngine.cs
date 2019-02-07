using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterSearchBackend;
using TwitterSearchBackend.Engines;

namespace TwitterTests.Mocks
{
    public class EngineMock_ValidationEngine : TwitterSearchServiceBase, IValidationEngine
    {
        public ValidationResult ValidateSearchText(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new ValidationResult(ErrorCodes.InvalidSearchParameter);
            //else if (searchText.Contains("null"))
            //    return new ValidationResult(ErrorCodes.InvalidSearchParameter);
            //else if (searchText.Contains("empty"))
            //    return new ValidationResult(ErrorCodes.InvalidSearchParameter);

            return new ValidationResult(true);
        }
    }
}
