﻿namespace Umbraco.Cms.Core.Models.Membership;

public class ChangeBackofficeUserPasswordModel
{
    public required string NewPassword { get; set; }

    /// <summary>
    ///     The old password - used to change a password when: EnablePasswordRetrieval = false
    /// </summary>
    public string? OldPassword { get; set; }

    /// <summary>
    ///     The user requesting the password change
    /// </summary>
    public required IUser User { get; set; }
}
