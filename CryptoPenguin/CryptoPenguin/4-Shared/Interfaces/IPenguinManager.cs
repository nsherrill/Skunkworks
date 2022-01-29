using CryptoPenguin.Shared.BaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.Interfaces
{
    public interface IPenguinManager
    {
        Task<ServiceResult<bool>> Start(string[] args = null);
    }
}
