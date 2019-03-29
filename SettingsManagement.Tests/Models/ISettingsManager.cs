namespace SettingsManagement.Tests.Models
{
    public interface ISettingsManager
    {
        void Persist();
        void Refresh();
    }
}