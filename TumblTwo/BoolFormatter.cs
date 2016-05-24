using System;

namespace TumblTwo
{
    public class BoolFormatter : ICustomFormatter, IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
            {
                return string.Empty;
            }

            bool value = (bool)arg;
            switch (format ?? string.Empty)
            {
                case "YesNo":
                    {
                        return (value) ? "Yes" : "No";
                    }
                case "OnlineOffline":
                    {
                        return (value) ? "Online" : "Offline";
                    }
                default:
                    {
                        return value.ToString();
                    }
            }
        }
    }
}
