// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssembleException.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright> 
// <summary>
//   Image Assemble Exception is thrown for custom exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>Custom Exception class for Image Assemble Tool</summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Avoiding CAS problems in lower trust")]
    public class ImageAssembleException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ImageAssembleException class.
        /// </summary>
        public ImageAssembleException()
        {
        }

        /// <summary>Initializes a new instance of the ImageAssembleException class.</summary>
        /// <param name="message">Message to be displayed.</param>
        public ImageAssembleException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the ImageAssembleException class.</summary>
        /// <param name="message">Message to be displayed.</param>
        /// <param name="innerException">Inner Exception</param>
        public ImageAssembleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the ImageAssembleException class.</summary>
        /// <param name="imageName">Image name</param>
        /// <param name="spriteName">Sprite name</param>
        /// <param name="message">Display message</param>
        internal ImageAssembleException(string imageName, string spriteName, string message)
            : base(message)
        {
            this.ImageName = imageName;
            this.SpriteName = spriteName;
        }

        /// <summary>Initializes a new instance of the ImageAssembleException class.</summary>
        /// <param name="imageName">Image name</param>
        /// <param name="spriteName">Sprite name</param>
        /// <param name="message">Display message</param>
        /// <param name="innerException">Inner Exception object</param>
        internal ImageAssembleException(string imageName, string spriteName, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ImageName = imageName;
            this.SpriteName = spriteName;
        }

        /// <summary>Initializes a new instance of the ImageAssembleException class</summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected ImageAssembleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // make sure parameters are not null
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            // base class already called, now get out custom fields
            this.ImageName = info.GetString("ImageName");
            this.SpriteName = info.GetString("SpriteName");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Current Image under process
        /// </summary>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets Sprite Image Name
        /// </summary>
        public string SpriteName { get; private set; }

        #endregion

        #region Methods

#if !SILVERLIGHT
        /// <summary>GetObjectData override method</summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(
            SerializationInfo info, StreamingContext context)
        {
            // make sure parameters are not null
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            // call base class
            base.GetObjectData(info, context);

            // output our custom fields
            info.AddValue("ImageName", this.ImageName);
            info.AddValue("SpriteName", this.SpriteName);
        }
#endif

        #endregion Methods
    }
}
