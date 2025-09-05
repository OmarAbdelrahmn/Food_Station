namespace Application.Contracts.Roles;

public record RoleDetailsResponse
(
    string Id,
    string Name,
    bool IsDeleted
    );
