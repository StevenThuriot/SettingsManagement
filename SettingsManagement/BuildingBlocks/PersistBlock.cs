using SettingsManagement.Interfaces;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    class PersistBlock : PersistBaseBlock
    {
        public PersistBlock(Block block) : base(block, nameof(ICanPersist.Persist))
        {
        }
    }
}
