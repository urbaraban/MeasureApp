using Xamarin.Essentials;

namespace SureMeasure.Tools
{
    class Setting
    {
        public static string Get(string Key, string def_value)
        {
            if (Preferences.ContainsKey(Key) == false)
            {
                Preferences.Set(Key, def_value);
            }

            return Preferences.Get(Key, def_value);
        }

        public static void Set(string Key, string Value)
        {
                Preferences.Set(Key, Value);
        }

        public static void Remove(string Key)
        {
            if (Preferences.ContainsKey(Key) == true)
            {
                Preferences.Remove(Key);
            }
        }

        public static void Clear()
        {
            Preferences.Clear();
        }
    }
}
