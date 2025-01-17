﻿using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using NzCovidPass.Core.Cwt;
using NzCovidPass.Core.Verification;

namespace NzCovidPass.Core.Shared
{
    /// <summary>
    /// Extension methods to configure an <see cref="IServiceCollection" /> for the verifier.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static Action<PassVerifierOptions> DefaultConfigureOptions => (options) => { };
        private static Action<HttpClient> DefaultConfigureClient => (client) => { };

        /// <summary>
        /// Adds <see cref="PassVerifier" /> and related services to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" />.</param>
        /// <param name="configureOptions">The action used to configure the <see cref="PassVerifierOptions" />.</param>
        /// <param name="configureClient">The action used to configure the <see cref="HttpClient" />.</param>
        /// <returns>The updated <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddNzCovidPassVerifier(
            this IServiceCollection services,
            Action<PassVerifierOptions> configureOptions = null,
            Action<HttpClient> configureClient = null)
        {
            if (services == null)
                throw new ArgumentNullException("services");

            services.Configure<PassVerifierOptions>(configureOptions ?? DefaultConfigureOptions);

            services.AddHttpClient(nameof(HttpDecentralizedIdentifierDocumentRetriever), configureClient ?? DefaultConfigureClient);

            services.AddSingleton<ICwtSecurityTokenReader, CwtSecurityTokenReader>();
            services.AddSingleton<ICwtSecurityTokenValidator, CwtSecurityTokenValidator>();
            services.AddSingleton<IVerificationKeyProvider, DecentralizedIdentifierDocumentVerificationKeyProvider>();
            services.AddSingleton<IDecentralizedIdentifierDocumentRetriever, HttpDecentralizedIdentifierDocumentRetriever>();
            services.AddSingleton<PassVerifier>();

            return services;
        }
    }
}
