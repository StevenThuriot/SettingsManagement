namespace SettingsManagement
{
    public interface ISettingsManager
    {
        void Persist();
        void Refresh();
    }
}