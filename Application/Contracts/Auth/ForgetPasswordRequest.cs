using System.ComponentModel.DataAnnotations;

namespace TechSpire.APi.Contracts.Auth;

public record ForgetPasswordRequest
(
    [EmailAddress]
    [Required]
    string Email
    );
