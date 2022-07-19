namespace HyperUI.Core;

/// <summary>
/// Request for creating a property group.
/// </summary>
/// <param name="GroupName">Group name.</param>
/// <param name="Specification">Specification.</param>
public record CreatePropertyGroupRequest(string GroupName, string? Specification);

