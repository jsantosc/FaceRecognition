namespace FaceRecognition.Domain.CoreFramework
{
    /// <summary>
    /// Base entity class for all domains
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the domain.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; protected set; }
    }
}