using Microsoft.Extensions.DependencyInjection;

namespace Toucan.Server
{
    public static partial class Extensions
    {

        public static void ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(Filters.GlobalExceptionFilter));
            });
        }

    }
}
