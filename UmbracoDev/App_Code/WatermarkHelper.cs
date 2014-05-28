using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Adds a transparent watermark to a image, saves the image under folder the media folder
/// </summary>
/// <remarks>Checks if an image already is saved with a watermark then returns that url</remarks>
/// <param name="imageUrl">Url to the image</param>
/// <param name="posX">Width position, default is Right</param>
/// <param name="posY">Height position, default is Bottom</param>
/// <param name="opacity">Opacity of the watermark, default is 0.3f</param>
/// <param name="watermarkerPath">Path to watermarkimage, default is in root media/watermark.png</param>
/// <param name="overwrite">Overwrites a already saved image</param>
/// <returns></returns>
public static class WatermarkHelper
{
    private static string _watermarkPath = "media/watermark.png"; 

    private static string _filename = String.Empty;
    private static string _savePath = String.Empty;
    private static string _relativePath = String.Empty;
    private static float _opacity = 0;
    


    public static string CreateWatermark(string imageUrl, WatermarkPositionX posX = WatermarkPositionX.Right, 
                                                          WatermarkPositionY posY = WatermarkPositionY.Bottom, 
                                                          float opacity = 0.3f, 
                                                          string watermarkerPath = "", 
                                                          bool overwrite = false )
    {
        _opacity = opacity;
        if (!String.IsNullOrEmpty(watermarkerPath))
        {
            ParsePath(watermarkerPath);
        }

        //Get the path and URI's
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        var absoluteUri = new Uri(appPath + imageUrl);

        if (!absoluteUri.IsFile)
            return imageUrl;

        _filename = Path.GetFileNameWithoutExtension(absoluteUri.LocalPath);
        _savePath = Path.GetDirectoryName(absoluteUri.LocalPath) + "/" + _filename + "_watermark.jpg";
        _relativePath = _savePath.Replace(appPath, "");

        if (File.Exists(_savePath) && !overwrite)
            return _relativePath;
            
            try
            {
                //Get image from URL
                var image = Image.FromFile(absoluteUri.LocalPath);
                var imageWidth = image.Width;
                var imageHeight = image.Height;

                //create a Bitmap the Size of the original photograph
                var newImage = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);

                newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                //load the Bitmap into a Graphics object 
                var graphicsNewImage = Graphics.FromImage(newImage);

                //create a image object containing the watermark
                var imageWatermark = new Bitmap(appPath + _watermarkPath);
                var wmWidth = imageWatermark.Width;
                var wmHeight = imageWatermark.Height;

                //Set the rendering quality for this Graphics object
                graphicsNewImage.SmoothingMode = SmoothingMode.AntiAlias;

                //Draws the photo Image object at original size to the graphics object.
                graphicsNewImage.DrawImage(
                    image, 
                    new Rectangle(0, 0, imageWidth, imageHeight), 
                    0, 
                    0,  
                    imageWidth,
                    imageHeight, 
                    GraphicsUnit.Pixel);  


                var bmWatermark = new Bitmap(newImage);
                bmWatermark.SetResolution(image.HorizontalResolution, image.VerticalResolution);
              
                //Load this Bitmap into a new Graphic Object
                var grWatermark = Graphics.FromImage(bmWatermark);

                //Get modifications 
                var imageAttributes = GetWatermarkModifications(_opacity);

                var xPosOfWm = GetXpos(posX, imageWidth, wmWidth);
                var yPosOfWm = GetYpos(posY, imageHeight, wmHeight);

                grWatermark.DrawImage(imageWatermark,
                                      new Rectangle(xPosOfWm, yPosOfWm, wmWidth, wmHeight), 
                                      0, 
                                      0,
                                      wmWidth,
                                      wmHeight,
                                      GraphicsUnit.Pixel, 
                                      imageAttributes);

                //Replace the original photgraphs bitmap with the new Bitmap
                image = bmWatermark;

                //Encode to assure keep quality
                EncoderParameters myEncoderParameters;
                var jgpEncoder = SetJpgQuality(out myEncoderParameters, 95L);

                //Save 
                using (var ms = new MemoryStream())
                {
                    //if (!Directory.Exists(appPath))
                    //    Directory.CreateDirectory(appPath);

                    image.Save(_savePath, jgpEncoder, myEncoderParameters);

                    image.Save(ms, jgpEncoder,myEncoderParameters);
                    ms.Close();
                }
                //Cleanup
                bmWatermark.Dispose();
                imageWatermark.Dispose();
                image.Dispose();
                newImage.Dispose();
            }
            catch (Exception e)
            {
                return imageUrl;
            }

        return _relativePath;

    }

    private static void ParsePath(string watermarkPath)
    {
        var data = watermarkPath.Split(';');
        _watermarkPath = data[0];
        _opacity = float.Parse(data[1]);

    }

    private static ImageAttributes GetWatermarkModifications(float opacity)
    {
        var imageAttributes = new ImageAttributes();

        ////Transparent
        float[][] colorMatrixElements = { 
                                            new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},       
                                            new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},        
                                            new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},        
                                            new float[] {0,  0,  0,  opacity, 0},        
                                            new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}};
        ////GrayImage
        //float[][] colorMatrixElements =
        //    {
        //        new float[] {.3f, .3f, .3f, 0, 0},
        //        new float[] {.59f, .59f, .59f, 0, 0},
        //        new float[] {.11f, .11f, .11f, 0, 0},
        //        new float[] {0, 0, 0, 1, 0},
        //        new float[] {0, 0, 0, 0, 1}
        //    };
  
        imageAttributes.SetColorMatrix(new ColorMatrix(colorMatrixElements), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        return imageAttributes;
    }

    private static ImageCodecInfo SetJpgQuality(out EncoderParameters myEncoderParameters, long quality)
    {
        var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
        var encoder = Encoder.Quality;
        myEncoderParameters = new EncoderParameters(1);
        myEncoderParameters.Param[0] = new EncoderParameter(encoder, quality);
        return jgpEncoder;
    }

    private static int GetYpos(WatermarkPositionY posY, int imageHeight, int wmHeight)
    {
        if (posY == WatermarkPositionY.Bottom)
        {
            return (imageHeight - wmHeight) - 10;
        }
        if (posY == WatermarkPositionY.Center)
        {
            return (int)Math.Floor((imageHeight - wmHeight) / 2.0);
        }
        return 10;
    }

    private static int GetXpos(WatermarkPositionX posX, int imageWidth, int wmWidth)
    {
        if (posX == WatermarkPositionX.Right)
        {
            return (imageWidth - wmWidth) - 10;
        }
        if (posX == WatermarkPositionX.Center)
        {
            return (int)Math.Floor((imageWidth - wmWidth) / 2.0);
        }
        return 10;
            
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {

        var codecs = ImageCodecInfo.GetImageDecoders();

        return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
    }
}
public enum WatermarkPositionX
{
    Left,
    Center,
    Right
}

public enum WatermarkPositionY
{
    Top,
    Center,
    Bottom
}