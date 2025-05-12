using Avalonia.Media.Imaging;
using OpenCvSharp;
using System;

namespace RobotCommander.FPV
{
    public interface IFPVManager
    {
        event EventHandler<Bitmap> FrameChanged;

        void ConvertToAvaloniaBitmap(Mat mat);
        Bitmap CreateTextFrame(string text);
        void Dispose();
        void Run(string deviceID);
    }
}