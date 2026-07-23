using Microsoft.EntityFrameworkCore;

namespace SmartFactory.Api.Data;

/// <summary>
/// One-time data seeding for the SmartFactory database. The code-first migrations
/// create the empty schema; this fills it with the demo dataset (identical to the
/// original seed) by executing database/seed.sql when the database has no rows yet.
/// Idempotent: it does nothing once the data is present.
/// </summary>
public static class DatabaseSeeder
{
    public static void Seed(SmartFactoryDbContext context, IWebHostEnvironment environment)
    {
        // Roles is the first table the seed populates; its presence means we are already seeded.
        if (context.Roles.Any())
        {
            return;
        }

        var seedFile = Path.Combine(environment.ContentRootPath, "database", "seed.sql");
        if (!File.Exists(seedFile))
        {
            return;
        }

        // seed.sql is plain, portable multi-row INSERTs (snake_case columns matching the
        // EF model), ordered so foreign keys resolve — it runs as a single SQL Server batch.
        var sql = File.ReadAllText(seedFile);
        context.Database.ExecuteSqlRaw(sql);
    }
}
