using SettingsManagement.Interfaces;

namespace SettingsManagement.BuildingBlocks;

class PersistBlock : PersistBaseBlock
{
    public PersistBlock(Block block) : base(block, nameof(ICanPersist.Persist))
    {
    }
}
