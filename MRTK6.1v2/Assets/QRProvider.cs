using System.Threading;
using UnityEngine;
using Vuforia;
using System.Diagnostics;
using System.Linq;
using ZXing;

public class QRProvider : MonoBehaviour
{
    /* Accessible properties */
    public ResultPoint[][] QrCodesAsResultPoints { get { return result?.Select(qrCode => qrCode.ResultPoints).ToArray(); } }
    public string[] QrCodesAsStrings { get { return result?.Select(qrCode => qrCode.Text).ToArray(); } }

    Result[] result;
    public Result[] Result { get { return result; } }


    /* Vuforia Camera Properties */
    private bool mAccessCameraImage = true;

    // The desired camera image pixel format
    private PIXEL_FORMAT mPixelFormat = PIXEL_FORMAT.GRAYSCALE;// or RGBA8888, RGB888, RGB565, YUV

    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;


    /* FrameRate properties */
    int fpsCounter = 0;
    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    
    [SerializeField]
    float m_refreshTime = 0.5f;


    /* Decoder properties */
    ZXingDecoder decoder;
    Thread decodeThread = null;


    /* Debugging purporses */
    [SerializeField]
    TextMesh textRef;

    /* CTor */
    public QRProvider Instantiate()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);

        // Instantiate decoder
        decoder = new ZXingDecoder();

        return this;
    }

    /* Unity Methods */
    private void Update()
    {
        if (result != null)
        {
            var printableResult = string.Join(", ", QrCodesAsStrings);
            print("QR CODES : " + printableResult);
            if (textRef != null) textRef.text = printableResult;
        }
        else
        {
            if (textRef != null) textRef.text = "Found no QR Codes";
        }
    }

    /* Private Methods */
    /// <summary>
    /// Called when Vuforia is started
    /// </summary>
    private void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            UnityEngine.Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to register pixel format " + mPixelFormat.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }
    }

    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    private void OnPause(bool paused)
    {
        if (paused)
        {
            UnityEngine.Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            UnityEngine.Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// <summary>
    /// Called each time the Vuforia state is updated
    /// </summary>
    private void OnTrackablesUpdated()
    {
        if (mFormatRegistered && fpsCounter > m_lastFramerate / 3 && m_lastFramerate > 30)
        {
            fpsCounter = 0;
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
                if (image != null)
                {
                    byte[] pixels = image.Pixels;
                    if (decodeThread != null)
                    {
                        // if a decode is already running, abort said decode
                        decodeThread.Abort();
                        decodeThread = null;
                    }
                    else
                    {
                        // start new decode thread
                        // if fps issues arise, might need to look into decode a subset of the given pixels
                        decodeThread = new Thread(() => FindAndDecodeQR(pixels, image.Width, image.Height, ZXing.RGBLuminanceSource.BitmapFormat.Gray8));
                        decodeThread.Start();
                    }
                }
            }
        }

        TrackFrameRate();
    }

    void FindAndDecodeQR(byte[] pixels, int imageWidth, int imageHeight, ZXing.RGBLuminanceSource.BitmapFormat format)
    {
        Stopwatch decodeStopWatch = Stopwatch.StartNew();

        result = decoder.DecodeMultiple(pixels, imageWidth, imageHeight, format);

        decodeStopWatch.Stop();
        UnityEngine.Debug.Log($"Decoded {result.Length} qrCodes in {decodeStopWatch.ElapsedMilliseconds} milliseconds");
    }

    // records the current frames per second - used to limit amount of qr-decodes are run pr second
    void TrackFrameRate()
    {
        fpsCounter++;

        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            m_lastFramerate = (float)m_frameCounter / m_timeCounter;
            m_frameCounter = 0;
            m_timeCounter = 0.0f;
        }
    }

    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
        UnityEngine.Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }

    /// <summary>
    /// Register the camera pixel format
    /// </summary>
    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            UnityEngine.Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }
}
