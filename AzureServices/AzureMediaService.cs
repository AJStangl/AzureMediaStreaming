using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureMediaStreaming.Settings;
using EnsureThat;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            IOptions<ClientSettings> clientSettings,
            IAzureMediaServicesClient azureMediaServicesClient)
        {
            _logger = logger;
            _clientSettings = clientSettings?.Value ?? throw new ArgumentNullException(nameof(clientSettings));
            _azureMediaServicesClient = azureMediaServicesClient;
        }


        public async Task<Transform> GetOrCreateTransformAsync(string transformName)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(transformName, nameof(transformName));

            Transform transform = await _azureMediaServicesClient.Transforms.GetAsync(_clientSettings.ResourceGroup,
                _clientSettings.AccountName, transformName);

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
                _clientSettings.AccountName, transformName, output);

            return transform;
        }

        public async Task<Asset> CreateOutputAssetAsync(string assetName)
        {
            EnsureArg.IsNotEmptyOrWhiteSpace(assetName, nameof(assetName));

            Asset outputAsset =
                await _azureMediaServicesClient.Assets.GetAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName, assetName);
            Asset asset = new Asset();
            string outputAssetName = assetName;

            if (outputAsset != null)
            {
                string uniqueness = $"-{Guid.NewGuid().ToString("N")}";
                outputAssetName += uniqueness;
                _logger.LogWarning("Warning – found an existing Asset with name = " + assetName);
                _logger.LogInformation("Creating an Asset with this name instead: " + outputAssetName);
            }

            return await _azureMediaServicesClient.Assets.CreateOrUpdateAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName,
                outputAssetName, asset);
        }

        public async Task<Asset> CreateInputAssetAsync(string assetName,
            string fileToUpload)
        {
            Asset asset = await _azureMediaServicesClient.Assets.CreateOrUpdateAsync(_clientSettings.ResourceGroup,
                _clientSettings.AccountName, assetName, new Asset());

            var response = await _azureMediaServicesClient.Assets.ListContainerSasAsync(
                _clientSettings.ResourceGroup,
                _clientSettings.AccountName,
                assetName,
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddHours(4).ToUniversalTime());

            var sasUri = new Uri(response.AssetContainerSasUrls.First());

            CloudBlobContainer container = new CloudBlobContainer(sasUri);
            var blob = container.GetBlockBlobReference(Path.GetFileName(fileToUpload));

            await blob.UploadFromFileAsync(fileToUpload);

            return asset;
        }

        public async Task<Job> SubmitJobAsync(string transformName, string outputAssetName, string jobName)
        {
            JobInputHttp jobInput =
                new JobInputHttp(files: new[]
                {
                    "https://ajstangl.blob.core.windows.net/asset-0ce38fc1-77cc-4c8f-ad68-398ffb401032/ch01-20200524-224458-224511-103000000000.avi?sv=2017-04-17&sr=c&si=4babe4e2-2852-47e3-b2ac-2dacb3ea5a45&sig=3cs3aNGMT8Ymkxs3xb1MLNGwKSt1FUQjSoyX%2Byzks3s%3D&st=2020-05-31T18%3A23%3A46Z&se=2120-05-31T18%3A23%3A46Z"
                });

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(outputAssetName),
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

        public async Task<Job> WaitForJobToFinishAsync(string transformName,
            string jobName)
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

        public async Task DownloadOutputAssetAsync(string assetName,
            string outputFolderName)
        {
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

        public async Task CleanUpAsync(string transformName, List<string> assetNames,
            string jobName)
        {
            await _azureMediaServicesClient.Jobs.DeleteAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName, transformName,
                jobName);

            assetNames.ForEach(async assetName =>
            {
                await _azureMediaServicesClient.Assets.DeleteAsync(_clientSettings.ResourceGroup, _clientSettings.AccountName,
                    assetName);
            });
        }
    }
}