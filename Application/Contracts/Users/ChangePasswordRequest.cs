namespace TechSpire.APi.Contracts.Users;

public record ChangePasswordRequest
(
    string CurrentPassword,
    string NewPassord
    );
