namespace SmartFactory.Api.Models.Database;

/// <summary>
/// A monitoring camera and the factory area it watches. Master data that replaces
/// the previously hard-coded camera list and the camera→area map in CameraRepository.
/// </summary>
public sealed class CameraEntity
{
    public string CameraCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AreaId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
