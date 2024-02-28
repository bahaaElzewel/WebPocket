using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace WebPocket
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private Newtonsoft.Json.JsonSerializer _serializer = new Newtonsoft.Json.JsonSerializer();
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(actualValue, arguments))
                : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new StreamReader(fileStream);
            using JsonTextReader reader = new JsonTextReader(streamReader);
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;
                var key = reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(key, value);
            }
        }

        private string GetString(string key)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath = Path.GetFullPath(filePath);
            if (File.Exists(fullFilePath))
            {
                var result = GetValueFromJson(key, fullFilePath);
                return result;
            }
            return string.Empty;
        }

        private string GetValueFromJson(string propertyName, string filePath)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(filePath))
                return string.Empty;
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new StreamReader(fileStream);
            using JsonTextReader reader = new JsonTextReader(streamReader);
            while(reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }
            return string.Empty;
        }
    }
}