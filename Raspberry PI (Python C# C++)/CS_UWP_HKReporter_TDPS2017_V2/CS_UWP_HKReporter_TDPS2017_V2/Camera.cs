using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Printers;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Storage;
using Windows.UI.Xaml;
using CS_UWP_HKRepoter_TDPS2017_TcpIpManager;
using Windows.Media.MediaProperties;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using CS_UWP_HKRepoter_TDPS2017_Exception;

namespace CS_UWP_HKRepoter_TDPS2017_Camera
{
	class Camera
	{
		#region Singleton

		private static Camera _instance;

		protected Camera()
		{
			Init();
		}

		public static Camera GetInstance()
		{
			if (_instance == null)
			{
				_instance = new Camera();
			}
			return _instance;
		}

		#endregion

		private MediaCapture _camera;

		public MediaCapture CameraCapture => _camera;

		private DispatcherTimer _sendPhotoTimer;
		private TcpIpManager _tcpIpManager;

		private async void Init()
		{
			_tcpIpManager = TcpIpManager.GetInstance();

			_sendPhotoTimer = new DispatcherTimer();
			_sendPhotoTimer.Interval = TimeSpan.FromMilliseconds(500);
			_sendPhotoTimer.Tick += TimerTick;

			_camera = new MediaCapture();
			await _camera.InitializeAsync();
		}

		public async void StartSendingServiceAsync()
		{
			_camera.StartPreviewAsync();
			_sendPhotoTimer.Start();
		}

		public async void StopSendingServiceAsync()
		{
			await _camera.StopPreviewAsync();
			_sendPhotoTimer.Stop();
		}

		protected bool GetPhotoTaskOnTime = false;

		private Task listenTask;

		private async void TimerTick(object sender, object e)
		{
			if (listenTask?.IsCompleted == false)
			{
				listenTask = null;
				return;
			}
			if (GetPhotoTaskOnTime == false)
			{
				GetPhotoTaskOnTime = true;
				if (_tcpIpManager.State == TcpIpManager.NowState.NotFindServer)
				{
					listenTask =  _tcpIpManager.ListenAsync();
					GetPhotoTaskOnTime = false;
				}
				else if (_tcpIpManager.State == TcpIpManager.NowState.FindServer)
				{
					StorageFile file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.ReplaceExisting);

					// Get information about the preview
					var previewProperties = _camera.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

					// Create a video frame in the desired format for the preview frame
					VideoFrame videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);
					try
					{
						VideoFrame previewFrame = await _camera.GetPreviewFrameAsync(videoFrame);
						SoftwareBitmap previewBitmap = previewFrame.SoftwareBitmap;
						await SaveSoftwareBitmapToFileAsync(previewBitmap, file);
					}
					catch (Exception)
					{
						return;
					}
					_tcpIpManager.SendPictureAsync(file.Path);

					GetPhotoTaskOnTime = false;
				}
				else
				{
					throw new LogicErrorException();
				}
			}
		}

		private async Task SaveSoftwareBitmapToFileAsync(SoftwareBitmap softwareBitmap, StorageFile outputFile)
		{
			using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
			{
				// Create an encoder with the desired format
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

				// Set the software bitmap
				encoder.SetSoftwareBitmap(softwareBitmap);

				// Set additional encoding parameters, if needed
				encoder.BitmapTransform.ScaledWidth = (uint)softwareBitmap.PixelWidth;
				encoder.BitmapTransform.ScaledHeight = (uint)softwareBitmap.PixelHeight;
				encoder.BitmapTransform.Rotation = Windows.Graphics.Imaging.BitmapRotation.None;
				encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
				encoder.IsThumbnailGenerated = true;

				try
				{
					await encoder.FlushAsync();
				}
				catch (Exception err)
				{
					switch (err.HResult)
					{
						case unchecked((int)0x88982F81): //WINCODEC_ERR_UNSUPPORTEDOPERATION
														 // If the encoder does not support writing a thumbnail, then try again
														 // but disable thumbnail generation.
							encoder.IsThumbnailGenerated = false;
							break;
						default:
							throw err;
					}
				}

				if (encoder.IsThumbnailGenerated == false)
				{
					await encoder.FlushAsync();
				}
			}
		}
	}
}
