
using Toucan.Contract.Model;

namespace Toucan.Service.Model
{
    public class KeyValue : IKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}