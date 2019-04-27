using RFKBackend.Shared.Contracts;
using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Engines
{
    public abstract class ValidationEngine<T> : IValidationEngine<T> where T : class
    {
        public virtual ValidationResult<T> Validate(T objToValidate)
        {
            return new ValidationResult<T>()
            {
                IsSuccess = objToValidate != null,
                ValidatedData = objToValidate,
                ValidationErrors = null
            };
        }
    }
}
