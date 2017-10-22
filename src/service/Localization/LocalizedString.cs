
using Toucan.Contract.Model;

namespace Toucan.Service.Model
{
    public class LocalizedString : ILocalizedString
    {
        public LocalizedString(string cultureName, string name, string value)
        {
            this.CultureName = cultureName;
            this.Name = name;
            this.Value = value;
        }

        public string CultureName { get; private set; }

        public string Name { get; private set; }

        public string Value { get; private set; }
    }
}