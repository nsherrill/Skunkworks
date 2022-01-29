using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.BaseObjects
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public ServiceResult<T> SetSuccess(T result)
        {
            this.Result = result;
            this.IsError = false;
            this.IsSuccess = true;
            return this;
        }
        public ServiceResult<T> SetFailure(string message)
        {
            this.Message = message;
            this.IsError = true;
            this.IsSuccess = false;
            return this;
        }
    }
}
