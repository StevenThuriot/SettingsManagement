using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    class DisposeBlock : PersistBaseBlock
    {
        public DisposeBlock(Block block) : base(block, nameof(IDisposable.Dispose))
        {
        }
    }
}
