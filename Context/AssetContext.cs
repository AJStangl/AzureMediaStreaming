using AzureMediaStreaming.Context.Models;
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
        public DbSet<AssetMetaDataEntity> AssetMetaDataEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AssetEntityMap());
            builder.ApplyConfiguration(new StreamingUrlMap());
            builder.ApplyConfiguration(new AssetMetaDataEntityMap());
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

        public class AssetMetaDataEntityMap : IEntityTypeConfiguration<AssetMetaDataEntity>
        {
            public void Configure(EntityTypeBuilder<AssetMetaDataEntity> builder)
            {
                builder
                    .HasIndex(x => x.Id)
                    .IsUnique();

                builder.HasIndex(x => x.AssetEntityId);

                builder
                    .HasOne(d => d.AssetEntity)
                    .WithOne(p => p.AssetMetaDataEntity);

                builder.Property(x => x.CreatedDate)
                    .HasDefaultValueSql(GetDateSql);

                builder.Property(x => x.UpdatedDate)
                    .HasDefaultValueSql(GetDateSql);

                builder.Property(x => x.FirstName)
                    .HasMaxLength(100);

                builder.Property(x => x.LastName)
                    .HasMaxLength(100);

                builder.Property(x => x.City)
                    .HasMaxLength(100);

                builder.Property(x => x.State)
                    .HasMaxLength(100);

                builder.Property(x => x.Street)
                    .HasMaxLength(100);

                builder.Property(x => x.ZipCode)
                    .HasMaxLength(100);

                builder.Property(x => x.Date);

                builder.Property(x => x.Time);
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