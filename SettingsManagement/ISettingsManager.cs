using System.Collections.Generic;

namespace SettingsManagement
{
    public interface ISettingsManager
    {
        void Persist();
        void Refresh();
        IEnumerable<string> GetReadableValues();
    }
}