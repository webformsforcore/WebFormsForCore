// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BitmapContainer.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.ImageAssemble
{
    using System;
    using System.Drawing;
    using System.IO;

    /// <summary>The bitmap container, used to do thread safe bitmap execution.</summary>
    internal class BitmapContainer
    {
        /// <summary>The bitmap.</summary>
        private Bitmap bitmap;

        /// <summary>Initializes a new instance of the <see cref="BitmapContainer"/> class.</summary>
        /// <param name="inputImage">The input image.</param>
        internal BitmapContainer(InputImage inputImage)
        {
            this.InputImage = inputImage;
        }

        /// <summary>Gets the input image.</summary>
        internal InputImage InputImage { get; private set; }

        /// <summary>Gets or sets the bitmap.</summary>
        internal Bitmap Bitmap 
        {
            get
            {
                return this.bitmap;
            }

            set
            {
                this.bitmap = value;
                if (value != null)
                {
                    this.Width = value.Width;
                    this.Height = value.Height;
                }
                else
                {
                    this.Width = 0;
                    this.Height = 0;
                }
            }
        }

        /// <summary>Gets the width.</summary>
        internal int Width { get; private set; }

        /// <summary>Gets the height.</summary>
        internal int Height { get; private set; }

        /// <summary>The bitmap action.</summary>
        /// <param name="action">The action.</param>
        public void BitmapAction(Action<Bitmap> action)
        {
            Safe.FileLock(new FileInfo(this.InputImage.AbsoluteImagePath), Safe.MaxLockTimeout, () => action(this.Bitmap));
        }
    }
}