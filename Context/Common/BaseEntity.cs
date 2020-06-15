using System;

namespace AzureMediaStreaming.Context.Common
{
    /// <summary>
    ///     Base class for common entity properties
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        ///     The base Id for the Entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     The date the record was initially created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     The date the record was updated.
        /// </summary>
        public DateTime UpdatedDate { get; set; }
    }
}