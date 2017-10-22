using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// attrib: https://stackoverflow.com/questions/32782937/generically-flatten-json-using-c-sharp
public class JsonHelper
{
    public static Dictionary<string, object> DeserializeAndFlatten(string json)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        JToken token = JToken.Parse(json);
        FillDictionaryFromJToken(dict, token, "");
        return dict;
    }

    private static void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                }
                break;

            case JTokenType.Array:
                int index = 0;
                foreach (JToken value in token.Children())
                {
                    FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                    index++;
                }
                break;

            default:
                dict.Add(prefix, ((JValue)token).Value);
                break;
        }
    }

    private static string Join(string prefix, string name)
    {
        return (string.IsNullOrEmpty(prefix) ? name : prefix + "." + name);
    }
}