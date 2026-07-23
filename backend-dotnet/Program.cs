using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using SmartFactory.Api.Data;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

// QuestPDF Community license (free for this use). Must be set before generating any PDF.
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Code-first EF Core context for the SmartFactory SQL Server database. snake_case
// convention keeps table/column names identical to the original schema design.
builder.Services.AddDbContext<SmartFactoryDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("SmartFactorySqlServer"))
        .UseSnakeCaseNamingConvention());
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
            (uri.Host == "localhost" || uri.Host == "127.0.0.1"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<SampleDataService>();
builder.Services.AddScoped<AppSettingsService>();
builder.Services.AddSingleton<SqlServerConnectionFactory>();
builder.Services.AddScoped<FormsRepository>();
builder.Services.AddScoped<SafetyRepository>();
builder.Services.AddScoped<NotificationsRepository>();
builder.Services.AddScoped<WorkforceRepository>();
builder.Services.AddScoped<WarehouseRepository>();
builder.Services.AddScoped<CameraRepository>();
builder.Services.AddScoped<UsersRepository>();

var app = builder.Build();

// Apply code-first migrations and seed the demo data on startup.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SmartFactoryDbContext>();
    context.Database.Migrate();
    DatabaseSeeder.Seed(context, app.Environment);
}

app.UseCors("Frontend");
app.MapControllers();

app.Run();
