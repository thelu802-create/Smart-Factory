namespace SmartFactory.Api.Models.Database;

/// <summary>
/// A single tunable application parameter, stored as a key-value pair so business
/// rules (thresholds, allowed values, ...) can be changed in the database without
/// a code change. ValueType documents how Value should be parsed ("number", "list",
/// "string"); Category groups related settings (e.g. "Camera").
/// </summary>
public sealed class AppSettingEntity
{
    // Named SettingKey/SettingValue (not Key/Value) so the columns are setting_key /
    // setting_value — 'key' and 'value' are reserved words in SQL Server.
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
}
