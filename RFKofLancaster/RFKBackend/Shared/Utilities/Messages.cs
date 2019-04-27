using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFKBackend
{
    public static class Messages
    {
        public static string CreateVolunteer_Success(string input) { return CreateGeneric_Success("volunteer", input); }
        public static string CreateVolunteer_Fail(string input = null) { return CreateGeneric_Fail("volunteer", input); }
        public static string SaveVolunteer_Fail(string input = null) { return SaveGeneric_Fail("volunteer", input); }
        public static string ValidateVolunteer_Fail(string input = null) { return ValidateGeneric_Fail("volunteer", input); }

        public static string CreateCabin_Success(string input) { return CreateGeneric_Success("cabin", input); }

        #region privates
        private static string CreateGeneric_Success(string modelType, string input) { return $"Successfully created {modelType}: {input}"; }
        private static string CreateGeneric_Fail(string modelType, string input = null) { return $"Error creating {modelType}: {input}"; }

        private static string SaveGeneric_Fail(string modelType, string input = null) { return $"Error saving {modelType}: {input}"; }

        private static string ValidateGeneric_Fail(string modelType, string input = null) { return $"Validation failed for {modelType}: {input}"; }
        #endregion
    }
}
