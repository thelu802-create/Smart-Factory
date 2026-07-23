namespace SmartFactory.Api.Models.Requests;

/// <summary>
/// Payload for a (simulated) camera detection. CameraCode, EventType, and Severity
/// are required; Confidence is a 0..1 score. A high-severity, high-confidence
/// detection automatically raises a linked safety alert and notification.
/// </summary>
public sealed record CameraDetectionRequest(string? CameraCode, string? EventType, string? Severity, double? Confidence);
