using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;

public static class DbExtensions
{
    public static async Task<IApplicationBuilder> UseMigrationAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        await context.Database.MigrateAsync();

        return app;
    }
}
