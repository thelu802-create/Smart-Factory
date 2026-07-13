using SmartFactory.Api.Data;
using SmartFactory.Api.Repositories;
using SmartFactory.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
builder.Services.AddSingleton<SampleDataService>();
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<FormsRepository>();
builder.Services.AddScoped<SafetyRepository>();
builder.Services.AddScoped<NotificationsRepository>();

var app = builder.Build();

app.UseCors("Frontend");
app.MapControllers();

app.Run();
