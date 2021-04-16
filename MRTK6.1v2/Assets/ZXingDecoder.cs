using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZXing;

public class ZXingDecoder : IDecoder
{
    private BarcodeReader Scanner { get; }

    /* CTor */
    public ZXingDecoder()
    {
        Scanner = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = false,
            Options = { TryHarder = false, PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE } }
        };
    }

    /* Public Methods */

    /// <summary>
    /// Using passed image, finds and returns decoded QR codes as Result objects
    /// </summary>
    /// <param name="image">Image to decode</param>
    /// <param name="width">width of image</param>
    /// <param name="height">height of image</param>
    /// <param name="format">Format of image</param>
    /// <return>An array of QR codes as a result object</return>
    public Result[] DecodeMultiple(byte[] image, int width, int height, RGBLuminanceSource.BitmapFormat format)
    {
        if (!ValidImage(image, width, height))
        {
            return null;
        }

        Result[] returnValue = null;
        try
        {
            returnValue = Scanner.DecodeMultiple(image, width, height, format);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return returnValue;
    }
    
    /// <summary>
    /// Using passed image, finds and returns a QR code as a result object
    /// </summary>
    /// <param name="image">Image to decode</param>
    /// <param name="width">width of image</param>
    /// <param name="height">height of image</param>
    /// <return>An QR code as a result object</return>
    public Result DecodeSingle(Color32[] image, int width, int height)
    {
        if (!ValidImage(image, width, height))
        {
            return null;
        }

        Result returnValue = null;

        try
        {
            returnValue = Scanner.Decode(image, width, height);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return returnValue;
    }

    /* Private Methods */
    bool ValidImage(Color32[] image, int width, int height)
    {
        if (image == null || image.Length == 0 || width == 0 || height == 0)
        {
            return false;
        }

        return true;
    }

    bool ValidImage(byte[] image, int width, int height)
    {
        if (image == null || image.Length == 0 || width == 0 || height == 0)
        {
            return false;
        }

        return true;
    }
}
