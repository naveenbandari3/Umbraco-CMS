// Copyright (c) Umbraco.
// See LICENSE for more details.

using System.ComponentModel;

namespace Umbraco.Cms.Core.Configuration.Models;

/// <summary>
///     Typed configuration options for legacy machine key settings used for migration of members from a v8 solution.
/// </summary>
[UmbracoOptions(Constants.Configuration.ConfigLegacyPasswordMigration)]
public class LegacyPasswordMigrationSettings
{
    private const string StaticDecryptionKey = "";

    /// <summary>
    ///     Gets the decryption algorithm.
    /// </summary>
    /// <remarks>
    ///     Currently only AES is supported. This should include all machine keys generated by Umbraco.
    /// </remarks>
    public string MachineKeyDecryption => "AES";

    /// <summary>
    ///     Gets or sets the decryption hex-formatted string key found in legacy web.config machineKey configuration-element.
    /// </summary>
    [DefaultValue(StaticDecryptionKey)]
    public string MachineKeyDecryptionKey { get; set; } = StaticDecryptionKey;
}
