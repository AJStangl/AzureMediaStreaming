using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.Controllers.Models;
using AzureMediaStreaming.Settings;
using EnsureThat;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureMediaStreaming.AzureServices
{
    class AzureMediaService : IAzureMediaService
    {
        private readonly ILogger<AzureMediaService> _logger;
        private readonly ClientSettings _clientSettings;
        private readonly IAzureMediaServicesClient _azureMediaServicesClient;
        public AzureMediaService(
            ILogger<AzureMediaService> logger,
            IOptions<ClientSettings> clientSettings)
        {
            _logger = logger;
            _clientSettings = clientSettings?.Value ?? throw new ArgumentNullException(nameof(clientSettings));
            _azureMediaServicesClient = GetAzureMediaServicesClient();
        }

        public async Task<Transform> GetOrCreateTransformAsync()
        {
            const string adaptiveStreamingTransformName = "MyTransformWithAdaptiveStreamingPreset";
            Transform transform = await _azureMediaServicesClient
                .Transforms
                .GetAsync(
                    _clientSettings.ResourceGroup,
                    _clientSettings.AccountName,
                    adaptiveStreamingTransformName);

            if (transform != null) return transform;

            TransformOutput[] output =
            {
                new TransformOutput
                {
                    Preset = new BuiltInStandardEncoderPreset
                    {
                        // This sample uses the built-in encoding preset for Adaptive Bitrate Streaming.
                        PresetName = EncoderNamedPreset.AdaptiveStreaming
                    }
                }
            };
            transform = await _azureMediaServicesClient.Transforms.CreateOrUpdateAsync(_clientSettings.ResourceGroup,
                _clientSettings.AccountName, adaptiveStreamingTransformName, output);

            return transform;
        }
        /// <summary>
        /// Calls media service to check if an asset exists and if not to create one, store it in blob and return the
        /// asset.
        /// </summary>
        public async Task<Asset> CreateInputAssetAsync(MediaAsset mediaAssetFileDto)
        {
            // TODO: This inital check will be performed by a database call to get back the metadata associated with the asset
            Asset asset = await _azureMediaServicesClient.Assets.CreateOrUpdateAsync(_clientSettings.ResourceGroup,
                _clientSettings.AccountName, mediaAssetFileDto.InputAssetName, new Asset());

            // Use Media Services API to get back a response that contains SAS URL for the Asset container into which to
            // upload blobs. That is where you would specify read-write permissions and the expiration time for the SAS
            // URL.
            var response = await _azureMediaServicesClient.Assets.ListContainerSasAsync(
                _clientSettings.ResourceGroup,
                _clientSettings.AccountName,
                mediaAssetFileDto.InputAssetName,
                AssetContainerPermission.ReadWrite,
                DateTime.UtcNow.AddHours(4).ToUniversalTime());

            var sasUri = new Uri(response.AssetContainerSasUrls.First());

            // Use Storage API to get a reference to the Asset container that was created by calling Asset's
            // CreateOrUpdate method.
            CloudBlobContainer container = new CloudBlobContainer(sasUri);

            var blob = container.GetBlockBlobReference(mediaAssetFileDto.FormFile.FileName);

            // Use Storage API to upload the file into the container in storage.
            await blob.UploadFromStreamAsync(mediaAssetFileDto.FormFile.OpenReadStream());

            return asset;
        }

        public async Task<Asset> CreateOutputAssetAsync(string inputAssetName, string outputAssetName)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(inputAssetName, nameof(inputAssetName));
            EnsureArg.IsNotEmptyOrWhiteSpace(outputAssetName, nameof(outputAssetName));
            Asset asset = new Asset();
            return await _azureMediaServicesClient.Assets.CreateOrUpdateAsync(
                _clientSettings.ResourceGroup,
                _clientSettings.AccountName,
                outputAssetName,
                asset);
            // TODO: Hypothetically we should be able to check if the asset exists and if it does we should not add anything
            // Check if an asset already exists.
            // Asset outputAsset =
            //     await _azureMediaServicesClient
            //         .Assets
            //         .GetAsync(
            //             _clientSettings.ResourceGroup,
            //             _clientSettings.AccountName,
            //             inputAssetName);
        }

        public async Task<Job> SubmitJobAsync(
            string transformName,
            string jobName,
            string inputAssetName,
            string outputAssetName)
        {
            // Use the name of the created input asset to create the job input.
            JobInput jobInput = new JobInputAsset(assetName: inputAssetName);

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(outputAssetName)
            };

            Job job = await _azureMediaServicesClient.Jobs.CreateAsync(
                _clientSettings.ResourceGroup,
                _clientSettings.AccountName,
                transformName,
                jobName,
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                });

            return job;
        }

        public async Task<Job> WaitForJobToFinishAsync(string transformName, string jobName)
        {
            // TODO: Use an event grid https://docs.microsoft.com/en-us/azure/media-services/latest/monitor-events-portal-how-to
            const int sleepIntervalMinutes = 60 * 1000;

            Job job;
            do
            {
                job = await _azureMediaServicesClient.Jobs.GetAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName,
                    transformName, jobName);

                _logger.LogInformation($"Job is '{job.State}'.");
                for (int i = 0; i < job.Outputs.Count; i++)
                {
                    JobOutput output = job.Outputs[i];
                    _logger.LogInformation($"\tJobOutput[{i}] is '{output.State}'.");
                    if (output.State == JobState.Processing)
                    {
                        _logger.LogInformation($"  Progress: '{output.Progress}'.");
                    }
                }

                if (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled)
                {
                    await Task.Delay(sleepIntervalMinutes);
                }
            } while (job.State != JobState.Finished && job.State != JobState.Error && job.State != JobState.Canceled);

            return job;
        }

        public async Task<StreamingLocator> CreateStreamingLocatorAsync(string assetName, string locatorName)
        {
            StreamingLocator locator = await _azureMediaServicesClient.StreamingLocators.CreateAsync(
                _clientSettings.ResourceGroup,
                _clientSettings.AccountName,
                locatorName,
                new StreamingLocator
                {
                    AssetName = assetName,
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
                });

            return locator;
        }

        public async Task<IList<string>> GetStreamingUrlsAsync(string locatorName)
        {
            const string defaultStreamingEndpointName = "default";

            IList<string> streamingUrls = new List<string>();

            StreamingEndpoint streamingEndpoint =
                await _azureMediaServicesClient.StreamingEndpoints.GetAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName,
                    defaultStreamingEndpointName);

            if (streamingEndpoint != null)
            {
                if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                {
                    await _azureMediaServicesClient.StreamingEndpoints.StartAsync(_clientSettings.ResourceGroup,
                        _clientSettings.AccountName, defaultStreamingEndpointName);
                }
            }
            // Should Probably handle this...
            else
            {
                return new List<string>();
            }

            ListPathsResponse paths =
                await _azureMediaServicesClient.StreamingLocators.ListPathsAsync(_clientSettings.ResourceGroup,
                    _clientSettings.AccountName, locatorName);

            paths.StreamingPaths.ToList().ForEach(path =>
            {
                UriBuilder uriBuilder = new UriBuilder
                {
                    Scheme = "https",
                    Host = streamingEndpoint?.HostName,
                    Path = path?.Paths?.FirstOrDefault()
                };
                streamingUrls.Add(uriBuilder.ToString());
            });

            return streamingUrls;
        }

        public async Task DownloadOutputAssetAsync(string assetName)
        {
            const string outputFolderName = @"Output";
            if (!Directory.Exists(outputFolderName))
            {
                Directory.CreateDirectory(outputFolderName);
            }

            AssetContainerSas assetContainerSas = await _azureMediaServicesClient.Assets.ListContainerSasAsync(
                _clientSettings.ResourceGroup,
                _clientSettings.AccountName,
                assetName,
                permissions: AssetContainerPermission.Read,
                expiryTime: DateTime.UtcNow.AddHours(1).ToUniversalTime());

            Uri containerSasUrl = new Uri(assetContainerSas?.AssetContainerSasUrls?.FirstOrDefault());

            IList<Task> downloadTasks = new List<Task>();

            CloudBlobContainer container = new CloudBlobContainer(containerSasUrl);

            string directory = Path.Combine(outputFolderName, assetName);
            Directory.CreateDirectory(directory);

            _logger.LogInformation($"Downloading output results to '{directory}'...");

            BlobContinuationToken continuationToken = null;
            do
            {
                BlobResultSegment segment = await container.ListBlobsSegmentedAsync(
                    null,
                    true,
                    BlobListingDetails.None,
                    null,
                    continuationToken,
                    null,
                    null);
                segment.Results.ToList().ForEach(blobItem =>
                {
                    CloudBlockBlob blob = blobItem as CloudBlockBlob;
                    if (blob != null)
                    {
                        string path = Path.Combine(directory, blob.Name);
                        downloadTasks.Add(blob.DownloadToFileAsync(path, FileMode.Create));
                    }
                });
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            await Task.WhenAll(downloadTasks);

            _logger.LogInformation("Download complete.");
        }

        public async Task CleanUpAsync(string transformName, List<string> assetNames, string jobName)
        {
            await _azureMediaServicesClient.Jobs.DeleteAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName, transformName,
                jobName);

            assetNames.ForEach(async assetName =>
            {
                await _azureMediaServicesClient.Assets.DeleteAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName,
                    assetName);
            });
        }

        private AzureMediaServicesClient GetAzureMediaServicesClient()
        {
            ClientCredential clientCredential =
                new ClientCredential(_clientSettings?.AadClientId, _clientSettings?.AadSecret);
            var serviceClientCredentials = ApplicationTokenProvider.LoginSilentAsync(_clientSettings?.AadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure).Result;
            return new AzureMediaServicesClient(_clientSettings?.ArmEndpoint, serviceClientCredentials)
            {
                SubscriptionId = _clientSettings?.SubscriptionId,
            };
        }
    }
}