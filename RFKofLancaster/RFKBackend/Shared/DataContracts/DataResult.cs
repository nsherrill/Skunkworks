using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend.Shared.DataContracts
{
    public abstract class DataResult<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }

        public virtual void SetSuccess(T obj)
        {
            Data = obj;
            IsSuccess = true;
        }

        public virtual void SetError(string errorMessage, Exception e = null)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }
    }
}
