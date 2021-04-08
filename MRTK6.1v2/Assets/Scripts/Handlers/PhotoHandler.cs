using System.Linq;
using UnityEngine;

public class PhotoHandler
{
    private WebCamTexture webcam;
    public int Width { get { return webcam.width; } }
    public int Height { get { return webcam.height; } }
    public PhotoHandler()
    {
        var defaultDeviceName = WebCamTexture.devices.FirstOrDefault().name;

        webcam = new WebCamTexture(defaultDeviceName)
        {
            requestedWidth = 512,
            requestedHeight = 512,
            filterMode = FilterMode.Trilinear,
        };
    }

    public void Play()
    {
        webcam.Play();
    }

    public void Destroy()
    {
        if (webcam.isPlaying)
        {
            webcam.Stop();
        }
    }

    public bool IsReady()
    {
        return webcam != null && webcam.width > 0 && webcam.height > 0;
    }

    public Color32[] GetPixels(Color32[] data = null)
    {
        if (data == null || data.Length != webcam.width * webcam.height)
        {
            return webcam.GetPixels32();
        }
        return webcam.GetPixels32(data);
    }

    public int GetChecksum()
    {
        return (webcam.width + webcam.height + webcam.deviceName + webcam.videoRotationAngle).GetHashCode();
    }

    //public void SetCameraSize(int width, int height)
    //{
    //    webcam.width = 
    //}
}
