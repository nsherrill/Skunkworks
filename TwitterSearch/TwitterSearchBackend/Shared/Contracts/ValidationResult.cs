using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterSearchBackend
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Error { get; set; }

        public ValidationResult()
        {

        }

        public ValidationResult(string errorText)
            : base()
        {
            this.IsValid = false;
            this.Error = errorText;
        }

        public ValidationResult(bool isValid)
            : base()
        {
            this.IsValid = isValid;
            this.Error = null;
        }
    }
}
