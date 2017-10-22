using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Contract;

namespace Toucan.Server.Core
{
    public class DateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            string modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            string value = valueProviderResult.FirstValue;

            var resolver = bindingContext.HttpContext.RequestServices.GetService<IHttpServiceContextResolver>();

            IDomainContext context = resolver.Resolve();

            DateTime? utcDate = value.ToSourceUtc(context.Culture, context.SourceTimeZone);

            if (!utcDate.HasValue)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"DateTime is not in a parseable format");
                return Task.CompletedTask;
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(utcDate.Value);
                return Task.CompletedTask;
            }
        }
    }
}