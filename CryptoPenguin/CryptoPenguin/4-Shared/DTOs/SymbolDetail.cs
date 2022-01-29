using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.DTOs
{
    public class SymbolDetail
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PriceIncrement { get; set; }
        public int PriceIncrementDecimalPlaces
        {
            get
            {
                return (int)Math.Round(Math.Abs(Math.Log((double)this.PriceIncrement, 10.0)), 0);
            }
        }
    }
}
