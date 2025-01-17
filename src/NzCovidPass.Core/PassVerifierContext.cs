﻿using System;
using NzCovidPass.Core.Cwt;
using NzCovidPass.Core.Models;
using NzCovidPass.Core.Shared;

namespace NzCovidPass.Core
{
    /// <summary>
    /// Encapsulates details of the pass verification process.
    /// </summary>
    public class PassVerifierContext : ValidationContext
    {
        private CwtSecurityToken _token;

        /// <summary>
        /// Gets the token that was verified.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Will only be set when <see cref="ValidationContext.HasSucceeded" /> is <see langword="true" />.
        /// </para>
        /// <para>
        /// Attempting to access when <see cref="ValidationContext.HasSucceeded" /> is <see langword="false" /> will throw an <see cref="InvalidOperationException" />.
        /// </para>
        /// </remarks>
        public CwtSecurityToken Token => (HasSucceeded && _token != null) ?
            _token :
            throw new InvalidOperationException("Token has not been set.");

        /// <summary>
        /// Gets the public COVID pass contained in the token that was verified.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Will only be available when <see cref="ValidationContext.HasSucceeded" /> is <see langword="true" />.
        /// </para>
        /// <para>
        /// Attempting to access when <see cref="ValidationContext.HasSucceeded" /> is <see langword="false" /> will throw an <see cref="InvalidOperationException" />.
        /// </para>
        /// </remarks>
        public PublicCovidPass Pass => Token?.Credential?.CredentialSubject;

        /// <summary>
        /// Indicates that validation has succeeded for this context, with the provided <paramref name="token" />.
        /// </summary>
        /// <param name="token">The verified token.</param>
        public void Succeed(CwtSecurityToken token)
        {
            base.Succeed();

            _token = token;
        }

        /// <summary>
        /// Invalid pass components failure reason.
        /// </summary>
        public static FailureReason InvalidPassComponents => new FailureReason(nameof(InvalidPassComponents), "Pass payload must be in the form '<prefix>:/<version>/<base32-encoded-CWT>'.");

        /// <summary>
        /// Invalid pass payload failure reason.
        /// </summary>
        public static FailureReason EmptyPassPayload => new FailureReason(nameof(EmptyPassPayload), "Pass payload must not be empty or whitespace.");

        /// <summary>
        /// Failed prefix validation failure reason.
        /// </summary>
        public static FailureReason PrefixValidationFailed(string validPrefix) =>
            new FailureReason(nameof(PrefixValidationFailed), $"Pass prefix does not have a valid value [Valid prefix = {validPrefix}].");

        /// <summary>
        /// Failed version validation failure reason.
        /// </summary>
        public static FailureReason VersionValidationFailed(int validVersion) =>
            new FailureReason(nameof(VersionValidationFailed), $"Pass version does not have a valid value [Valid version = {validVersion}].");

        /// <summary>
        /// Failed token read failure reason.
        /// </summary>
        public static FailureReason TokenReadFailed => new FailureReason(nameof(TokenReadFailed), "Token read failed");

        /// <summary>
        /// Failed token validation failure reason.
        /// </summary>
        public static FailureReason TokenValidationFailed => new FailureReason(nameof(TokenValidationFailed), "Token validation failed.");
    }
}
