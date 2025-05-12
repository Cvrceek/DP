using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.IO;
using System.Runtime.InteropServices;

namespace RobotCommander.FPV.OpenCV
{
    public class FPVManager_OpenCV : IDisposable, IFPVManager
    {
        private CustomVideoCapture videoCapture;
        private bool isRunning;
        private CancellationTokenSource cancellationTokenSource;

        public event EventHandler<Bitmap> FrameChanged;
        Mat? previousFrame = null;
        Bitmap bitmapFrame = null;
        public void Run(string deviceID)
        {
            videoCapture = new CustomVideoCapture(deviceID);
            videoCapture.Set(VideoCaptureProperties.FrameWidth, 3840);
            videoCapture.Set(VideoCaptureProperties.FrameHeight, 2160);
            videoCapture.Set(VideoCaptureProperties.BufferSize, 1);
            videoCapture.Set(VideoCaptureProperties.Fps, 30);

            if (!videoCapture.IsOpened())
                throw new Exception("Error with Cam");


            isRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => GetFrames(cancellationTokenSource.Token));
        }

        private void GetFrames(CancellationToken token)
        {
            while (isRunning && !token.IsCancellationRequested)
            {
                using var frame = new Mat();
                videoCapture.Read(frame);

                if (frame.Empty() || frame.Total() == 0 || Cv2.Mean(frame)[0] == 0)
                {
                    FrameChanged?.Invoke(this, CreateTextFrame("error read frame"));
                }
                else
                {
                    if (previousFrame != null)
                    {
                        var diff = Cv2.Norm(previousFrame, frame, NormTypes.L2); // nebo NormTypes.L1
                        if (diff < 0.5) // Tolerance – 0 = úplně stejné, <1 velmi podobné
                            continue;
                    }

                    previousFrame?.Dispose();
                    previousFrame = frame.Clone();

                    //bitmapFrame = ConvertToAvaloniaBitmap(frame);
                    ConvertToAvaloniaBitmap(frame);
                    FrameChanged?.Invoke(this, bitmapFrame);
                    //bitmap.Dispose();
                }
                // Thread.Sleep(15); // ~30 fps
            }
        }

        public Bitmap CreateTextFrame(string text)
        {
            using var mat = new Mat(480, 640, MatType.CV_8UC3, Scalar.Black);
            Cv2.PutText(mat, text, new Point(100, 240),
                HersheyFonts.HersheySimplex, 1.5, Scalar.White, 2);
            return null;
            //return ConvertToAvaloniaBitmap(mat);
        }

        public void ConvertToAvaloniaBitmap(Mat mat)
        {
            Mat rgbaMat;

            if (mat.Type() == MatType.CV_8UC3)
            {
                rgbaMat = new Mat();
                Cv2.CvtColor(mat, rgbaMat, ColorConversionCodes.BGR2RGBA);
            }
            else if (mat.Type() == MatType.CV_8UC4)
            {
                rgbaMat = mat;
            }
            else
            {
                throw new NotSupportedException("Unsupported Mat type: " + mat.Type());
            }

            int width = rgbaMat.Width;
            int height = rgbaMat.Height;
            int stride = width * 4;

            bitmapFrame = new Bitmap(
                PixelFormat.Rgba8888,
                AlphaFormat.Unpremul,
                rgbaMat.Data,
                new Avalonia.PixelSize(width, height),
                new Avalonia.Vector(96, 96),
                stride);

            //int width = mat.Width;
            //int height = mat.Height;
            //int stride = width * 4;

            //bitmapFrame = new Bitmap(
            //    PixelFormat.Rgba8888,
            //    AlphaFormat.Unpremul,
            //    (nint)mat.Data,
            //    new Avalonia.PixelSize(width, height),
            //    new Avalonia.Vector(96, 96),
            //    stride);
        }

        public void Dispose()
        {
            isRunning = false;
            cancellationTokenSource?.Cancel();
            videoCapture?.Release();
            videoCapture?.Dispose();
        }
    }
}
