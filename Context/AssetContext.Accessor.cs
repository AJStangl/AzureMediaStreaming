using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureMediaStreaming.Context.Models;
using AzureMediaStreaming.Controllers.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureMediaStreaming.Context
{
    public partial class AssetContext
    {
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity) entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity) entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task CreateUpdateAssetEntity(MediaAsset mediaAsset)
        {

            AssetEntity assetEntity = new AssetEntity
            {
                Id = Guid.NewGuid(),
                FileName = mediaAsset.FormFile.FileName,
                AssetName = mediaAsset.AssetName,
                InputAssetName = mediaAsset.InputAssetName,
                OutputAssetName = mediaAsset.OutputAssetName,
                JobName = mediaAsset.JobName,
                LocatorName = mediaAsset.LocatorName,
            };
            assetEntity.StreamingUrl = mediaAsset.StreamingUrls.Select(x =>
            {
                var url = new StreamingUrl
                {
                    Id = Guid.NewGuid(), AssetEntityId = assetEntity.Id, AssetEntity = assetEntity, Url = x.Url
                };
                return url;
            }).ToHashSet();
            AssetEntities.Add(assetEntity);
            await SaveChangesAsync();
        }

        public async Task<AssetEntity> GetAssetsByName(string filename)
        {
            return await AssetEntities
                .Include(x => x.StreamingUrl)
                .FirstOrDefaultAsync(x => x.FileName == filename);
        }
        public List<StreamingUrl> GetStreamingUrl()
        {
            throw new System.NotImplementedException();
        }
    }
}