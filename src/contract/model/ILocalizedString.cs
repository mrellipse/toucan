
namespace Toucan.Contract.Model
{
    public interface ILocalizedString
    {
        string CultureName { get; }
        string Name { get; }
        string Value { get; }
    }
}