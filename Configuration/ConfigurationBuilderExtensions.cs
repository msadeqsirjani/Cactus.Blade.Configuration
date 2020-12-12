using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Cactus.Blade.Configuration
{
    /// <summary>
    /// Extension methods for adding configuration providers to an instance of <see cref="IConfigurationBuilder"/>.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// The default value for whether a configuration should be reloaded when its source changes.
        /// </summary>
        public const bool DefaultReloadOnChange = false;

        /// <summary>
        /// Sets the value of the <see cref="Config.Root"/> property by building the specified
        /// <see cref="IConfigurationBuilder"/>.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IConfigurationBuilder"/> that will be the source of
        /// the <see cref="Config.Root"/> property.
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder"/></returns>
        public static IConfigurationBuilder SetConfigRoot(this IConfigurationBuilder builder)
        {
            Config.SetRoot(builder.Build);

            return builder;
        }

        /// <summary>
        /// Sets the value of the <see cref="Config.Root"/> property to the <see cref="IConfiguration"/>
        /// containing the merged configuration of the application and the <see cref="IWebHost"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to configure.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder SetConfigRoot(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices((context, services) =>
            {
                if (!Config.IsLocked && Config.IsDefault)
                    Config.SetRoot(context.Configuration);
            });
        }

        /// <summary>
        /// Adds the ASP.NET Core app settings.json configuration provider to the builder using the configuration file "appsettings.json",
        /// relative to the base path stored in <see cref="IConfigurationBuilder.Properties"/> of the builder.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="builder"/> is null.</exception>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAppSettingsJson(this IConfigurationBuilder builder) =>
            builder.AddAppSettingsJson(DefaultReloadOnChange);

        /// <summary>
        /// Adds the ASP.NET Core app settings.json configuration provider to the builder using the configuration file "appsettings.json",
        /// relative to the base path stored in <see cref="IConfigurationBuilder.Properties"/> of the builder.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the app settings.json file changes.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="builder"/> is null.</exception>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddAppSettingsJson(this IConfigurationBuilder builder, bool reloadOnChange)
        {
            if (builder.IsNull())
                throw new ArgumentNullException(nameof(builder));

            builder = builder.AddJsonFile("AppSettings.json", true, reloadOnChange);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment.IsNullOrEmpty())
                environment = Environment.GetEnvironmentVariable("CACTUS_ENVIRONMENT");

            if (environment.IsNotNullOrEmpty())
                builder = builder.AddJsonFile($"AppSettings.{environment}.json", true, reloadOnChange);

            return builder;
        }
    }
}
