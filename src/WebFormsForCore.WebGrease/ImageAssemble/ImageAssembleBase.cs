// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssembleBase.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Packing type of Assembled image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.ImageAssemble
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    using Css.ImageAssemblyAnalysis;

    using WebGrease.Extensions;

    /// <summary>Provides the base implementation for Assembling image,
    /// which can be overridden by child classes.
    /// <remarks>
    /// Uses:
    /// (Octree based quantizer + Dithering) - Ron's code</remarks>
    /// </summary>
    internal abstract class ImageAssembleBase
    {
        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>Initializes a new instance of the <see cref="ImageAssembleBase"/> class.</summary>
        /// <param name="context">The context.</param>
        public ImageAssembleBase(IWebGreaseContext context)
        {
            this.context = context;
        }

        #region Properties

        /// <summary>
        /// Gets Image Format
        /// </summary>
        internal abstract ImageType Type
        {
            get;
        }

        /// <summary>
        /// Gets default extension for Image type.
        /// Default extension is the first type defined in 
        /// SupportedExtenions property.
        /// </summary>
        internal abstract string DefaultExtension
        {
            get;
        }

        /// <summary>
        /// Gets or sets Assembled file name
        /// </summary>
        internal string AssembleFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Assembled Image Packing Type
        /// </summary>
        internal SpritePackingType PackingType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets ImageMap object for Xml Logging
        /// </summary>
        internal ImageMap ImageXmlMap
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Padding value in pixel between images
        /// </summary>
        internal int PaddingBetweenImages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the command string to execute optimizer Tool exe that
        /// will be used to optimize sprite images. e.g optipng for PNG images
        /// </summary>
        internal string OptimizerToolCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets Image Format
        /// </summary>
        protected abstract ImageFormat Format
        {
            get;
        }

        #endregion

        #region Public Methods

        /// <summary>Assembles images based on their extension using orientation provided (Auto is default).
        /// Images are combined using Octree based quantizer + dithering algorithm.
        /// Images are also packed in rectangle using Nuclex Rectangle Packer algorithm.</summary>
        /// <param name="inputImages">The input Images.</param>
        /// <returns>The <see cref="bool"/>. True if something was assembled, false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all on purpose.")]
        internal virtual bool Assemble(List<BitmapContainer> inputImages)
        {
            Bitmap newImage = null;
            try
            {
                // If there is only one image then don't pack images
                if (inputImages.HasAtLeast(2))
                {
                    // If Orientation is Auto (which is by default) then get the orientation 
                    // wherein the assembled file size would be minimal.
                    // Else Pack the files according to the value specified.
                    switch (this.PackingType)
                    {
                        case SpritePackingType.Horizontal:
                            newImage = this.PackHorizontal(inputImages, true, null);
                            break;
                        case SpritePackingType.Vertical:
                            newImage = this.PackVertical(inputImages, true, null);
                            break;
                        default:
                            newImage = this.PackVertical(inputImages, true, null);
                            break;
                    }

                    if (newImage != null)
                    {
                        this.SaveAndHashImage(newImage, newImage.Width, newImage.Height);
                        return true;
                    }
                }
                else if (inputImages.Any())
                {
                    // Pass through, used for image by themselves or images that should be hashed but not sprited.
                    var image = inputImages.First();
                    this.ImageXmlMap.AppendToXml(image.InputImage.AbsoluteImagePath, this.AssembleFileName, image.Width, image.Height, 0, 0, "passthrough", true, image.InputImage.Position);
                    image.BitmapAction(bitmap => this.SaveAndHashImage(image.Bitmap, image.Width, image.Height));
                    return true;
                }
            }
            catch (OutOfMemoryException ex)
            {
                this.context.Log.Error(ex);

                // If there this type of exception is thrown while Image.FromFile(path)
                // then thrown a custom exception. Ref: http://msdn.microsoft.com/en-us/library/stf701f5.aspx
                var imageException = new ImageAssembleException(ImageAssembleStrings.ImageLoadOutofMemoryExceptionMessage, ex);
                throw imageException;
            }
            catch (Exception ex)
            {
                this.context.Log.Error(ex);
                try
                {
                    Safe.FileLock(
                        new FileInfo(this.AssembleFileName),
                        () =>
                        {
                            // First delete the invalid file, if it exists
                            if (File.Exists(this.AssembleFileName))
                            {
                                File.Delete(this.AssembleFileName);
                            }
                        });
                }
                catch (Exception)
                {
                }

                throw;
            }
            finally
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
            }

            return false;
        }

        /// <summary>Saves and hashed the bitmap.</summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void SaveAndHashImage(Bitmap bitmap, int width, int height)
        {
            var hash = this.context.GetBitmapHash(bitmap, this.Format);

            this.AssembleFileName = this.HashImage(hash);

            var targetFileInfo = new FileInfo(this.AssembleFileName);
            Safe.FileLock(
                targetFileInfo,
                () =>
                    {
                        if (!targetFileInfo.Exists)
                        {
                            this.SaveImage(bitmap);
                        }
                    });

            // Add the sprite image size to the imagemap output element.
            this.ImageXmlMap.UpdateSize(this.AssembleFileName, width, height);
        }

        #endregion

        #region Abstract / Overrideable Methods

        /// <summary>Saves image in a particular format.</summary>
        /// <param name="newImage">Image to be saved</param>
        protected virtual void SaveImage(Bitmap newImage)
        {
            try
            {
                if (!File.Exists(this.AssembleFileName))
                {
                    // This try-catch block checks if there is some GDI+ error occured while saving the image. 
                    // Incase an exception is thrown, remove the invalid file, if it is generated.
                    newImage.Save(this.AssembleFileName, this.Format);
                }
            }
            catch (ExternalException ex)
            {
                // If ExternalException is raised then there is a problem with Image Format or 
                // Image is save to the same file from where it was created. (ref: http://msdn.microsoft.com/en-us/library/9t4syfhh.aspx)
                // This exception is thrown also if access is denied to the path.
                // Handle this condition and throw a custom exception with actual exception as inner exception.
                var imageException = new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, ImageAssembleStrings.ImageSaveExternalExceptionMessage, this.AssembleFileName), ex);
                throw imageException;
            }
        }

        /// <summary>Runs the optimizer command on the saved image.</summary>
        protected void OptimizeImage()
        {
            // If Optimizer tool command is provided then execute the tool
            if (!string.IsNullOrEmpty(this.OptimizerToolCommand))
            {
                this.OptimizerToolCommand = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + this.OptimizerToolCommand;

                // Execute PNG Optimization tool
                // Find EXE Path and then pass it to ProcessStartInfo
                // Parse arugments and pass it to ProcessStartInfo as well
                // Start Process and wait for it to complete.
                var toolName = ".exe";
                var toolCommand = string.Format(CultureInfo.InvariantCulture, this.OptimizerToolCommand, this.AssembleFileName);
                var pos = toolCommand.IndexOf(toolName, StringComparison.OrdinalIgnoreCase);
                var exePath = toolCommand.Substring(0, pos + toolName.Length + 1);
                Trace.WriteLine("Image Optimization Executable - " + exePath);
                if (!File.Exists(exePath))
                {
                    throw new FileNotFoundException("Could not locate the image optimization executable.", exePath);
                }

                var procStartInfo = new ProcessStartInfo(exePath);
                procStartInfo.CreateNoWindow = true;
                procStartInfo.Arguments = toolCommand.Replace(exePath, string.Empty);
                procStartInfo.UseShellExecute = false;

                // Start the process
                var proc = Process.Start(procStartInfo);

                // Wait for process to complete
                proc.WaitForExit();
            }
        }

        /// <summary>Hashes the Assembled Image using MD5 hash algorithm</summary>
        /// <param name="hash">The Hash</param>
        protected string HashImage(string hash)
        {
            var fileInfo = new FileInfo(this.AssembleFileName);

            var newName = hash + fileInfo.Extension;

            var destinationDirectory = fileInfo.DirectoryName;

            // use the first 2 chars for a directory name, the last chars for the file name
            var destinationFilePath = Path.Combine(destinationDirectory, newName.Substring(0, 2));

            // this will be the 2 char subdir, and if it already exists this method does nothing
            Directory.CreateDirectory(destinationFilePath);

            // now get the file 
            destinationFilePath = Path.Combine(destinationFilePath, newName.Remove(0, 2));

            if (!this.ImageXmlMap.UpdateAssembledImageName(this.AssembleFileName, destinationFilePath))
            {
                throw new ImageAssembleException(null, this.AssembleFileName, "Operation failed while replacing assembled image name: '" + this.AssembleFileName + "' with hashed name.");
            }

            return destinationFilePath;
        }

        /// <summary>Packs images in rectangle in Horizontal orientation and makes an entry in Xml Map file.</summary>
        /// <param name="originalBitmaps">Dictionary of image url and its corresponding Bitmap.</param>
        /// <param name="useLogging">Flag to indicate if logging should be done.</param>
        /// <param name="pixelFormat">Nullable PixelFormat.</param>
        /// <returns>Assembled image.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Legacy Code, Fix TODO")]
        protected Bitmap PackHorizontal(List<BitmapContainer> originalBitmaps, bool useLogging, PixelFormat? pixelFormat)
        {
            var maxHeight = originalBitmaps.Max(c => c.Height);
            var totalWidth = originalBitmaps.Sum(c => c.Width) + (originalBitmaps.Count * this.PaddingBetweenImages);

            // New bitmap will have width as sum of all widths and heigh as max height
            var newImage = pixelFormat != null
                                  ? new Bitmap(totalWidth, maxHeight, (PixelFormat)pixelFormat)
                                  : new Bitmap(totalWidth, maxHeight);

            // Sort images descending by Height
            var result = originalBitmaps.OrderByDescending(entry => entry.Height);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                var xpoint = 0;
                var addOutputNode = true;

                foreach (var entry in result)
                {
                    entry.BitmapAction(bitmap => graphics.DrawImage(bitmap, new Rectangle(xpoint, 0, entry.Width, entry.Height)));

                    // Log to XmlMap if logging enabled
                    if (useLogging)
                    {
                        this.ImageXmlMap.AppendToXml(entry.InputImage.AbsoluteImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, 0, null, addOutputNode, entry.InputImage.Position);
                        addOutputNode = false;
                        foreach (var duplicateImagePath in entry.InputImage.DuplicateImagePaths)
                        {
                            this.ImageXmlMap.AppendToXml(duplicateImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, 0, "duplicate", addOutputNode, entry.InputImage.Position);
                        }
                    }

                    xpoint += entry.Width + this.PaddingBetweenImages;
                }
            }

            return newImage;
        }

        /// <summary>Packs images in rectangle using the orientation provided and makes an entry in Xml Map file.</summary>
        /// <param name="originalBitmaps">Dictionary of image url and its corresponding Bitmap.</param>
        /// <param name="useLogging">Flag to indicate if logging should be done.</param>
        /// <param name="pixelFormat">Nullable PixelFormat.</param>
        /// <returns>Assembled image.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Legacy Code, Fix TODO")]
        protected Bitmap PackVertical(List<BitmapContainer> originalBitmaps, bool useLogging, PixelFormat? pixelFormat)
        {
            var maxWidth = originalBitmaps.Max(c => c.Width);
            var totalHeight = originalBitmaps.Sum(c => c.Height);

            // Add padding
            totalHeight += originalBitmaps.Count * this.PaddingBetweenImages;

            // New bitmap will have width as max width and heigh as sum of all heights
            var spriteImage = pixelFormat != null
                                     ? new Bitmap(maxWidth, totalHeight, (PixelFormat)pixelFormat)
                                     : new Bitmap(maxWidth, totalHeight);

            // Sort images descending by width
            var result = originalBitmaps.OrderByDescending(entry => entry.Width);

            using (var graphics = Graphics.FromImage(spriteImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                var ypoint = 0;
                var addOutputNode = true;

                foreach (var entry in result)
                {
                    // Horizontal position for the image in vertical sprite changes as per market (LTR/RTL) .
                    var xpoint = 0;

                    // When not set explicitly, Image position is Left. This case is possible in two scenarios:
                    // 1. In LTR markets, the image to be sprited is Left aligned in CSS (normal scenario).
                    // 2. In RTL markets, the image to be sprited is Left aligned in CSS.
                    // When any of above occurs, render the image on left side in sprite (horizontal position = 0). 
                    // Image position - Right. This case is possible in two scenarios:
                    // 1. In LTR markets, the image to be sprited is Right aligned in CSS. 
                    // 2. In RTL markets, the image to be sprited is Right aligned in CSS (normal scenario).
                    // When any of above occurs, render the image on right side in sprite (horizontal position = Sprite Image Width - Current Image Width).
                    switch (entry.InputImage.Position)
                    {
                        case ImagePosition.Right:
                            xpoint = spriteImage.Width - entry.Width;
                            break;
                        case ImagePosition.Center:

                            // xpoint will have value using Standard Arithmetic Rounding
                            // i.e. if value is half way between two integers then it will be
                            // used as the next integer value (higher).
                            // e.g 
                            // 3.5 will become 4
                            // 4.5 will become 5
                            xpoint = (spriteImage.Width - entry.Width + 1) / 2;
                            break;
                        default:
                            break;
                    }

                    entry.BitmapAction(bitmap => graphics.DrawImage(bitmap, new Rectangle(xpoint, ypoint, entry.Width, entry.Height)));

                    // Log to XmlMap if logging enabled
                    if (useLogging)
                    {
                        this.ImageXmlMap.AppendToXml(entry.InputImage.AbsoluteImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, ypoint * -1, null, addOutputNode, entry.InputImage.Position);
                        addOutputNode = false;
                        foreach (var duplicateImagePath in entry.InputImage.DuplicateImagePaths)
                        {
                            this.ImageXmlMap.AppendToXml(duplicateImagePath, this.AssembleFileName, entry.Width, entry.Height, xpoint * -1, ypoint * -1, "duplicate", addOutputNode, entry.InputImage.Position);
                        }
                    }

                    ypoint += entry.Height + this.PaddingBetweenImages;
                }
            }

            return spriteImage;
        }

        #endregion
    }
}
