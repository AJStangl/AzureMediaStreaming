using AzureMediaStreaming.DataModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AzureMediaStreaming.Context
{
    public partial class AssetContext : DbContext, IAssetContext
    {
        private const string NewGuidSql = "NEWID()";
        private const string GetDateSql = "GETDATE()";

        public AssetContext(DbContextOptions<AssetContext> options) : base(options)
        {
        }

        public DbSet<AssetEntity> AssetEntities { get; set; }
        public DbSet<StreamingUrl> StreamingUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AssetEntityMap());
            builder.ApplyConfiguration(new StreamingUrlMap());
            base.OnModelCreating(builder);
        }

        public class AssetEntityMap : IEntityTypeConfiguration<AssetEntity>
        {
            public void Configure(EntityTypeBuilder<AssetEntity> builder)
            {
                builder
                    .HasIndex(x => x.Id)
                    .IsUnique();

                builder.Property(x => x.CreatedDate)
                    .HasDefaultValueSql(GetDateSql);

                builder.Property(x => x.UpdatedDate)
                    .HasDefaultValueSql(GetDateSql);

                builder.Property(x => x.FileName)
                    .HasMaxLength(1000);

                builder.Property(x => x.AssetName)
                    .HasMaxLength(1000);

                builder.Property(x => x.InputAssetName)
                    .HasMaxLength(1000);

                builder.Property(x => x.OutputAssetName)
                    .HasMaxLength(1000);

                builder.Property(x => x.JobName)
                    .HasMaxLength(1000);

                builder.Property(x => x.LocatorName)
                    .HasMaxLength(1000);
            }
        }

        public class StreamingUrlMap : IEntityTypeConfiguration<StreamingUrl>
        {
            public void Configure(EntityTypeBuilder<StreamingUrl> builder)
            {
                builder
                    .HasIndex(x => x.AssetEntityId);

                builder
                    .Property(e => e.Id)
                    .HasDefaultValueSql(NewGuidSql);

                builder.Property(x => x.CreatedDate)
                    .HasDefaultValueSql(GetDateSql);

                builder.Property(x => x.UpdatedDate)
                    .HasDefaultValueSql(GetDateSql);

                builder.Property(x => x.Url)
                    .HasMaxLength(1000);

                builder.HasOne(d => d.AssetEntity)
                    .WithMany(p => p.StreamingUrl)
                    .HasForeignKey(d => d.AssetEntityId);
            }
        }
    }
}