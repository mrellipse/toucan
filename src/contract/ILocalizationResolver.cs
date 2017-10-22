using System.Collections.Generic;
using Toucan.Contract.Model;

namespace Toucan.Contract
{
    public interface ILocalizationResolver
    {
        IEnumerable<IKeyValue> ResolveSupportedCultures();
        object ResolveCulture(string cultureName);
    }
}
