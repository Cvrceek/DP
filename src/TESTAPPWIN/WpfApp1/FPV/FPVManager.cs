using OpenCvSharp;
using OpenCvSharp.Text;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using System.IO;
using System.Windows.Media.Media3D;
using System.Drawing.Imaging;


namespace WpfApp1.FPV
{
    public class FPVManager
    {
        private CustomVideoCapture videoCapture;
        private bool isRunning;
        private CancellationTokenSource cancellationTokenSource;
        public event EventHandler<BitmapSource> FrameChanged;

        public void Run(string deviceID)
        {
            videoCapture = new CustomVideoCapture(deviceID);
            if (!videoCapture.IsOpened())
                throw new Exception("Error with Cam");

            isRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => getFrame(cancellationTokenSource.Token));
        }

        private void getFrame(CancellationToken token)
        {
            while (isRunning && !token.IsCancellationRequested)
            {
                using (var frame = new Mat())
                {
                    videoCapture.Read(frame);

                    if (frame.Empty() || frame.Total() == 0 || Cv2.Mean(frame)[0] == 0)
                    {
                        FrameChanged?.Invoke(this, NoVideo());
                    }
                    else
                    {
                        var bitmap = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Rgb24, null,
                                frame.Data, (int)frame.Step() * frame.Height, (int)frame.Step());
                        bitmap.Freeze();
                        FrameChanged?.Invoke(this, bitmap);
                    }
                    //cca 30fps
                    Thread.Sleep(33);
                }
            }
        }

        public static BitmapSource NoVideo()
        {
            using (var noSignalFrame = new Mat(480, 640, MatType.CV_8UC3, Scalar.Black))
            {

                Cv2.PutText(noSignalFrame, ResX.Strings.FPV_NoVideo, new OpenCvSharp.Point(150, 240),
                    HersheyFonts.HersheySimplex, 2, Scalar.White, 3);
                var bitmap = BitmapSource.Create(noSignalFrame.Width, noSignalFrame.Height, 96, 96, PixelFormats.Rgb24, null,
                      noSignalFrame.Data, (int)noSignalFrame.Step() * noSignalFrame.Height, (int)noSignalFrame.Step());
                bitmap.Freeze();
                return bitmap;
            }
        }

        public static BitmapSource LoadingCam()
        {
            using (var noSignalFrame = new Mat(480, 640, MatType.CV_8UC3, Scalar.Black))
            {

                Cv2.PutText(noSignalFrame, ResX.Strings.FPV_LoadingCam, new OpenCvSharp.Point(150, 240),
                    HersheyFonts.HersheySimplex, 2, Scalar.White, 3);
                var bitmap = BitmapSource.Create(noSignalFrame.Width, noSignalFrame.Height, 96, 96, PixelFormats.Rgb24, null,
                      noSignalFrame.Data, (int)noSignalFrame.Step() * noSignalFrame.Height, (int)noSignalFrame.Step());
                bitmap.Freeze();
                return bitmap;
            }
        }

        public static BitmapSource NoCam()
        {
            using (var noSignalFrame = new Mat(480, 640, MatType.CV_8UC3, Scalar.Black))
            {

                Cv2.PutText(noSignalFrame, ResX.Strings.FPV_ProblemWithCam, new OpenCvSharp.Point(150, 240),
                    HersheyFonts.HersheySimplex, 2, Scalar.White, 3);
                var bitmap = BitmapSource.Create(noSignalFrame.Width, noSignalFrame.Height, 96, 96, PixelFormats.Rgb24, null,
                      noSignalFrame.Data, (int)noSignalFrame.Step() * noSignalFrame.Height, (int)noSignalFrame.Step());
                bitmap.Freeze();
                return bitmap;
            }
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
