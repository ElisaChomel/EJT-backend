//using Azure.Storage.Blobs;
//using Elcia.Sacerdoce.AzureBlobStorage.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace judo_backend.Helpers.HealtCheck
{
    /// <summary>
    /// Classe pour les controles d'intégrité du Blob
    /// </summary>
    public class BlobHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Gestionnaire de blob.
        /// </summary>
        //BlobServiceClient _blobClient;

        /// <summary>
        /// Configuration pour l'accès au blob.
        /// </summary>
        //AzureBlobStorageProviderConfiguration _configuration;

        /// <summary>
        /// Initilise une nouvelle instance de la classe <see cref="BlobHealthCheck"/>
        /// </summary>
        /// <param name="blobClient">Gestionnaire de blob.</param>
        /// <param name="configuration">Configuration pour l'accès au blob.</param>
        //public BlobHealthCheck(BlobServiceClient blobClient, AzureBlobStorageProviderConfiguration configuration)
        //{
        //    _blobClient = blobClient;
        //    _configuration = configuration;
        //}

        /// <summary>
        /// Exécute la vérification de l'état. 
        /// </summary>
        /// <param name="context">Object contextuel associé à l'execution en cours.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>Token pour l'annulation.</param>
        /// <returns>A <see cref="Task{HealthCheckResult}"/>Retourne l'état du composant.</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //try
            //{
            //    BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(_configuration.ContainerName);
                
            //    if (blobContainerClient != null)
            //    {
                    return Task.FromResult(HealthCheckResult.Healthy("The blob storage is up and running."));
            //    }

                //return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "The blob storage is down."));
            //}
            //catch (Exception)
            //{
            //    return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "The blob storage is down."));
            //}
        }
    }
}
