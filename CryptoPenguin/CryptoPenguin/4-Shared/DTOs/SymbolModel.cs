using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.DTOs
{
    public class SymbolModel
    {
        public string Symbol { get; set; }
        public long Id { get; set; }
        public bool IsActive { get; set; }
    }
}
