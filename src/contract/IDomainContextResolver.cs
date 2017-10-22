using System;
using System.Globalization;

namespace Toucan.Contract
{
    public interface IDomainContextResolver
    {
        IDomainContext Resolve(bool cache = true);
    }
}