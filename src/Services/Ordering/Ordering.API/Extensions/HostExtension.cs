using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ordering.API.Extensions
{
    public static class HostExtension
    {
        public static WebApplication MigrateDatabase<TContext>(this WebApplication app,
            Action<TContext, IServiceProvider> seeder,
            int? retry = 0) where TContext : DbContext
        {
            int retryForAvailability = retry.Value;

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associateed with context {context.GetType().Name}.");
                    InvokeSeeder(seeder, context, services);

                    logger.LogInformation($"Migrated database associateed with context {context.GetType().Name}.");
                }
                catch (SqlException ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database on context {context.GetType().Name}");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(app, seeder, retryForAvailability);
                    }
                }
                return app;
            }
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder,
            TContext context,
            IServiceProvider services) where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
