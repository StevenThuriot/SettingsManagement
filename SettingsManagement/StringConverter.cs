namespace SettingsManagement
{
    class StringConverter : IValueConverter<string>
    {
        public string Convert(string value) => value;

        public string ConvertBack(string value) => value;
    }
}
