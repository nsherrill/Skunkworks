using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.Contracts
{
    public interface IValidationEngine<T> where T : class
    {
        ValidationResult<T> Validate(T objToValidate);
    }
}
