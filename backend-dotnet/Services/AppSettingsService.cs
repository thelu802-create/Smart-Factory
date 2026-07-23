using System.Globalization;
using SmartFactory.Api.Data;

namespace SmartFactory.Api.Services;

/// <summary>
/// Central access to tunable parameters stored in the app_settings table. Every
/// getter falls back to a supplied default when the setting row is missing or
/// unpariseable, so the app keeps working even if a setting was never seeded.
/// This is the one place business thresholds/rules are read from, so they can be
/// changed in the database without a code change.
/// </summary>
public sealed class AppSettingsService(SmartFactoryDbContext context)
{
    public string? GetRaw(string key) =>
        context.AppSettings.Where(setting => setting.SettingKey == key).Select(setting => setting.SettingValue).FirstOrDefault();

    public string GetString(string key, string fallback) => GetRaw(key) ?? fallback;

    public int GetInt(string key, int fallback) =>
        int.TryParse(GetRaw(key), NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : fallback;

    public double GetDouble(string key, double fallback) =>
        double.TryParse(GetRaw(key), NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : fallback;

    public string[] GetList(string key, string[] fallback)
    {
        var raw = GetRaw(key);
        return string.IsNullOrWhiteSpace(raw)
            ? fallback
            : raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
