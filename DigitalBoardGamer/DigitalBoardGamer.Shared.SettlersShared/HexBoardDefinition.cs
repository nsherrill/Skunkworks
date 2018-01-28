using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBoardGamer.Shared.SettlersShared
{
    public class HexBoardDefinition : HexType
    {
        public int MaxHexCount { get; set; }

        public HexBoardDefinition() { }
        public HexBoardDefinition(HexBoardDefinition original)
        {
            this.MaxHexCount = original.MaxHexCount;
            this.Name = original.Name;
            this.ImageUrl = original.ImageUrl;
            this.BackupColor = original.BackupColor;
            this.TypeId = original.TypeId;
        }
    }
}
