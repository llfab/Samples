using System;
using System.Collections.Generic;
using ic4;

namespace ImagingSourceCaptureTest
{
    public class WindowsImagingSourceCameraStream
    {
        #region Fields

        private Grabber _grabberDevice;
        private QueueSink _queueSink;

        private readonly object _deviceAccess;

        #endregion

        #region Construction

        public WindowsImagingSourceCameraStream()
        {
            _deviceAccess = new object();
        }

        #endregion

        #region Public Access

        public bool Init()
        {
            try
            {
                ic4.Library.Init();

                lock (_deviceAccess)
                {
                    bool result = InitInternal();

                    Console.WriteLine(string.Format("Done. Success[{0}]", result));

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
                return false;
            }
        }

        public void Exit()
        {
            try
            {
                lock (_deviceAccess)
                {
                    ExitInternal();
                    Console.WriteLine(string.Format("Done."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
            }
        }

        #endregion

        #region Private Methods

        private bool InitInternal()
        {
            try
            {
                IReadOnlyList<DeviceInfo> deviceList = ic4.DeviceEnum.Devices;
                if (deviceList.Count == 0)
                {
                    Console.WriteLine(string.Format("No camera found."));
                    return false;
                }

                DeviceInfo deviceInfo = deviceList[0];
                Console.WriteLine(string.Format("Found Model[{0}] Name[{1}]", deviceInfo.ModelName, deviceInfo.UniqueName));

                Grabber grabber = new Grabber();
                _grabberDevice = grabber;

                bool success = StartCapture(deviceInfo);
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
                return false;
            }
        }

        private void ExitInternal()
        {
            try
            {
                StopCapture();

                if (_grabberDevice != null)
                {
                    _grabberDevice.Dispose();
                    _grabberDevice = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
            }
        }

        private bool StartCapture(DeviceInfo deviceInfo)
        {
            try
            {
                _grabberDevice.DeviceOpen(deviceInfo);

                PropertyMap propertyMap = _grabberDevice.DevicePropertyMap;
                propertyMap.SetValue(ic4.PropId.Width, 1280);
                propertyMap.SetValue(ic4.PropId.Height, 960);
                //propertyMap.SetValue(ic4.PropId.Width, 1440);
                //propertyMap.SetValue(ic4.PropId.Height, 1080);
                propertyMap.SetValue(ic4.PropId.AcquisitionFrameRate, 30);

                Console.WriteLine(string.Format("Resolution[{0}x{1}] DesiredFps[{2}] PixelFormat[{3}]",
                    propertyMap.GetValueLong(ic4.PropId.Width), propertyMap.GetValueLong(ic4.PropId.Height),
                    propertyMap.GetValueDouble(ic4.PropId.AcquisitionFrameRate),
                    propertyMap.GetValueString(ic4.PropId.PixelFormat)));

                _queueSink = new ic4.QueueSink([PixelFormat.BGRa8]);
                _queueSink.FramesQueued += OnFramesQueued;

                _grabberDevice.StreamSetup(_queueSink, StreamSetupOption.AcquisitionStart);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
                StopCapture();
                return false;
            }
        }

        private void StopCapture()
        {
            try
            {

                if (_grabberDevice != null)
                {
                    _grabberDevice.StreamStop();

                    if (_queueSink != null)
                    {
                        _queueSink.FramesQueued -= OnFramesQueued;
                        _queueSink.Dispose();
                        _queueSink = null;
                    }

                    _grabberDevice.DeviceClose();
                }

                Console.WriteLine(string.Format("done."));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("error.\n{0}", ex));
            }
        }

        #endregion

        #region Capture Callbacks

        private unsafe void OnFramesQueued(object sender, QueueSinkEventArgs e)
        {
            try
            {
                if (e.Sink.TryPopOutputBuffer(out ImageBuffer buffer))
                {
                    using (ImageBuffer imageBuffer = buffer)
                    {
                        // comment below line if you don't want to see the frame data
                        Console.WriteLine(string.Format("Frame available. Width[{0}] Height[{1}] PixelFormat[{2}]",
                            imageBuffer.ImageType.Width, imageBuffer.ImageType.Height, imageBuffer.ImageType.PixelFormat));
                        System.GC.Collect();
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("No frame available."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error.\n{0}", ex));
            }
        }

        #endregion
    }
}
