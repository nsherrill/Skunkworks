using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPenguin.Shared.DTOs
{
    public class OrderDetail
    {
        public bool IsActive { get; internal set; }
        public bool StopTriggered { get; internal set; }
        public string Id { get; internal set; }
        public bool IsBuy { get; internal set; }
        public bool IsSell { get; internal set; }
    }
}
