using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using System.Diagnostics;

public class CameraAccess : MonoBehaviour
{
    [SerializeField]
    TextMesh textRef;

    private bool mAccessCameraImage = true;

    // The desired camera image pixel format
    private PIXEL_FORMAT mPixelFormat = PIXEL_FORMAT.GRAYSCALE;// or RGBA8888, RGB888, RGB565, YUV

    // Boolean flag telling whether the pixel format has been registered
    private bool mFormatRegistered = false;

    RawImage shownImage;

    Thread parseThread = null;

    int fpsCounter = 0;

    string[] result;

    int m_frameCounter = 0;
    float m_timeCounter = 0.0f;
    float m_lastFramerate = 0.0f;
    [SerializeField]
    float m_refreshTime = 0.5f;



    ZXingDecoder parser;
    void Start()
    {
        // Register Vuforia life-cycle callbacks:

        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);

        parser = new ZXingDecoder();
    }

    private void Update()
    {
        if (result != null)
        {
            var printableResult = string.Join(", ", result);
            print("QR CODES : " + printableResult);
            textRef.text = printableResult;
        }
        else
        {
            textRef.text = "Found no QR Codes";
        }
    }


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

                    if (parseThread != null)
                    {
                        parseThread.Abort();
                        parseThread = null;
                    }
                    else
                    {
                        string imageInfo = mPixelFormat + " image: \n";
                        imageInfo += " size: " + image.Width + " x " + image.Height + "\n";
                        imageInfo += " bufferSize: " + image.BufferWidth + " x " + image.BufferHeight + "\n";
                        imageInfo += " stride: " + image.Stride;
                        //UnityEngine.Debug.Log(imageInfo);

                        // if fps issues arise, might need to look into decode a subset of the given pixels
                        //int startIndex = (int)(Mathf.FloorToInt((1f / 5f) * image.Width) + (((1f / 5f) * image.Height) * image.Width));
                        //var endIndex = pixels.Length - startIndex;
                        //UnityEngine.Debug.Log("startIndex: " + startIndex + ". endIndex: " + endIndex + ". % saved: " + (100 - ((endIndex - startIndex) / pixels.Length) * 100));
                        //byte[] newArr = new byte[endIndex-startIndex];
                        //Array.Copy(pixels, startIndex, newArr, 0, endIndex - startIndex);

                        //parseThread = new Thread(() => FindAndDecodeQR(pixels, image.Width, image.Height, ZXing.RGBLuminanceSource.BitmapFormat.Gray8));
                        parseThread = new Thread(() => FindAndDecodeQR(pixels , image.Width, image.Height, ZXing.RGBLuminanceSource.BitmapFormat.Gray8));
                        parseThread.Start();
                    }



                    if (pixels != null && pixels.Length > 0)
                    {
                        //UnityEngine.Debug.Log("Image pixels: " + pixels[0] + "," + pixels[1] + "," + pixels[2] + ",...");
                    }
                }
            }
        }

        fpsCounter++;

        RecordFrameRate();
    }



    void FindAndDecodeQR(byte[] pixels, int imageWidth, int imageHeight, ZXing.RGBLuminanceSource.BitmapFormat format)
    {
        Stopwatch stopWatch = Stopwatch.StartNew();
        result = parser.DecodeMultiple(pixels, imageWidth, imageHeight, format);
        //UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds + ":" + pixels.Length);
        stopWatch.Stop();
        return;
    }


    void RecordFrameRate()
    {
        if (m_timeCounter < m_refreshTime)
        {
            m_timeCounter += Time.deltaTime;
            m_frameCounter++;
        }
        else
        {
            //This code will break if you set your m_refreshTime to 0, which makes no sense.
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