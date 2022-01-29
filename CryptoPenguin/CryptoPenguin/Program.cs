using CryptoPenguin.Managers;
using CryptoPenguin.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IPenguinManager empMgr = new EmperorPenguinManager();
            await empMgr.Start();
        }
    }
}
