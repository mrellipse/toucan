using System;

namespace Toucan.Common.Extensions
{
    public static class Reflection
    {
        public static string GetAssemblyName(this Type type)
        {
            return type.AssemblyQualifiedName.Split(',')[1].Trim();
        }
    }
}