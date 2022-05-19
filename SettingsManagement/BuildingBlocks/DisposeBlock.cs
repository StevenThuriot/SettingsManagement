namespace SettingsManagement.BuildingBlocks;

class DisposeBlock : PersistBaseBlock
{
    public DisposeBlock(Block block) : base(block, nameof(IDisposable.Dispose))
    {
    }
}
