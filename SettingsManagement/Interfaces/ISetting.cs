namespace SettingsManagement.Interfaces
{
    interface ISetting
    {
        string GetReadableValue();
        void Persist();
        void Refresh();
        void Refresh(bool overwriteChanges);
    }
}