using System;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;

namespace CoinActivity
{
    public class CoinReader
    {
        private string _pathToFile;
        private List<double> ListDetectedRadii = new List<double>();

        public CoinReader(string pathToFile)
        {
            _pathToFile = pathToFile;
        }

        public Bitmap ImagePreprocessing()
        {
            Mat _image;
            Mat _originalImage;
            Mat new_image;
            _originalImage = new Mat(_pathToFile, ImreadModes.Color);
            _image = _originalImage.Clone();

            // Apply grayscale
            new_image = TransformGrayScale(_image);

            // Apply Gaussian blur
            new_image = TransformGaussianBlur(new_image);

            // Apply Hough Circle detection to original image
            _originalImage = HoughSegmentation(new_image, _originalImage);

            Bitmap processedBitmap = GetProcessedBitmap(_originalImage);

            return processedBitmap;
        }

        private Mat TransformGrayScale(Mat _image)
        {
            Cv2.CvtColor(_image, _image, ColorConversionCodes.BGR2GRAY);
            return _image;
        }

        private Mat TransformGaussianBlur(Mat _image)
        {
            Cv2.GaussianBlur(_image, _image, new OpenCvSharp.Size(5, 5), 1.5);
            return _image;
        }

        private Mat HoughSegmentation(Mat _image, Mat old_image)
        {
            Mat result = old_image.Clone();  

            // Detect circles using Hough Transform
            var circleSegments = Cv2.HoughCircles(_image, HoughMethods.Gradient, 1, 30, param1: 150, param2: 50, minRadius: 10, maxRadius: 100);

            ListDetectedRadii.Clear();


            if (circleSegments != null)
            {
                foreach (var circle in circleSegments)
                {
                    // Draw the detected circle in green
                    Cv2.Circle(result, (OpenCvSharp.Point)circle.Center, (int)circle.Radius, new Scalar(0, 255, 0), 5);

                    // Display the radius at the center of the circle
                    string radiusText = $"{circle.Radius}";
                    Cv2.PutText(result, radiusText, new OpenCvSharp.Point((int)circle.Center.X - 30, (int)circle.Center.Y + 10), HersheyFonts.HersheySimplex, 0.6, new Scalar(0, 255, 0), 2);

                    // Add the radius to the list
                    ListDetectedRadii.Add(circle.Radius);
                }
            }

            old_image = result;
            return old_image;
        }

        public List<double> GetListRadii()
        {
            return ListDetectedRadii;
        }

        public Bitmap GetProcessedBitmap(Mat _image)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(_image);
        }
    }
}
