using Accord.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.TaskAwaitableWithoutEndValidation;
using System.Windows.Threading;
using System.Windows;
using Accord.Video;
using System.Diagnostics;
using SharpAvi.Output;
using SharpAvi;
using System.Windows.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SharpAvi.Codecs;


namespace Open.recui.ViewModels
{
    public class MainWindowViewModel: ObservableObject
    {
        private bool _isRecording = false;
        private bool _isNotRecording = true;
        private string _statusText = "Status: aguardando";  
        private DispatcherTimer _timer;
        private int _frameRate = 10; // Define a taxa de quadros
        private AviWriter _writer;
        private IAviVideoStream _videoStream;
        private Thread _recordingThread;


        public MainWindowViewModel() 
        {
            StartRecording = new AsyncRelayCommand(StartScreenRecording);
            StopRecording = new AsyncRelayCommand(StopScreenRecording); 
        }

        public bool IsRecording
        {

            get => _isRecording;
            set => SetProperty(ref _isRecording, value);
        }

        public bool IsNotRecording
        {

            get => _isNotRecording;
            set => SetProperty(ref _isNotRecording, value);
        }

        public string StatusText
        {

            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private async Task StartScreenRecording() 
        {
           
            IsNotRecording = false;
            _writer = new AviWriter("screencapture.avi")
            {
                FramesPerSecond = _frameRate,
                EmitIndex1 = true
            };

            _videoStream = _writer.AddMJpegWpfVideoStream((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
            _videoStream.Width = (int)SystemParameters.PrimaryScreenWidth;
            _videoStream.Height = (int)SystemParameters.PrimaryScreenHeight;
            //_videoStream.Codec = CodecIds.MotionJpeg;
            //_videoStream.BitsPerPixel = BitsPerPixel.Bpp24;

            IsRecording = true;
            _recordingThread = new Thread(RecordScreen);
            _recordingThread.Start();

            StatusText = "Status: gravando";

        }

        private async Task StopScreenRecording()
        {
            IsRecording = false;
            await Task.Delay(100);
            IsNotRecording = true;
            _isRecording = false;
            _recordingThread.Join();
            _writer.Close();

            StatusText = "Status: parado";

        }

        private async void RecordScreen()
        {
            var screenBounds = new Rectangle(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
            var buffer = new byte[screenBounds.Width * screenBounds.Height * 4];

            while (_isRecording)
            {
                using (var bitmap = new Bitmap(screenBounds.Width, screenBounds.Height))
                {
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(screenBounds.Location, System.Drawing.Point.Empty, screenBounds.Size);
                        
                    }

                    
                    var bits = bitmap.LockBits(screenBounds, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    var native = bits.Scan0;
                    try
                    {
                        Marshal.Copy( native, buffer, 0, buffer.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bits);
                        
                    }
                    
                    

                    _videoStream.WriteFrame(true, buffer, 0, buffer.Length);
                }

                await Task.Delay(1000 / _frameRate);
            }
        }
    

        public IAsyncRelayCommand StartRecording { get; }
        public IAsyncRelayCommand StopRecording { get; }
    }
}
