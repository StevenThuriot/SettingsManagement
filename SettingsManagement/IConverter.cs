namespace SettingsManagement
{
    public interface IConverter<T>
    {
        T Convert(string value);
    }
}
