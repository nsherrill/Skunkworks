using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public class ValidationResult<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public string[] ValidationErrors { get; set; }
        public T ValidatedData { get; set; }
    }
}
