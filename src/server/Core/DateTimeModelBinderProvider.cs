using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Toucan.Server.Core
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
                return new DateTimeModelBinder();

            return null;
        }
    }
}