using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class HexType
    {
        public long TypeId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string BackupColor { get; set; }

        public HexType() { }
        public HexType(HexType original)
        {
            this.TypeId = original.TypeId;
            this.Name = original.Name;
            this.ImageUrl = original.ImageUrl;
            this.BackupColor = original.BackupColor;
        }
    }
}
