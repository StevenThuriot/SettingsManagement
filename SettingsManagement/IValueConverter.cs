namespace SettingsManagement
{
    public interface IValueConverter<T>
    {
        T Convert(string value);
        string ConvertBack(T value);
    }
}
