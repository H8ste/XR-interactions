using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZXing;

public class ZXingDecoder : IDecoder
{
    private BarcodeReader Scanner { get; }

    public ZXingDecoder()
    {
        Scanner = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = false,
            Options = { TryHarder = false, PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE } }
        };
    }

    public string[] DecodeMultiple(byte[] rawRgb, int width, int height, RGBLuminanceSource.BitmapFormat format)
    {
        if (rawRgb == null || rawRgb.Length == 0 || width == 0 || height == 0)
        {
            return null;
        }


        string[] returnValue = null;

        try
        {
            var result = Scanner.DecodeMultiple(rawRgb, width, height, format);
            returnValue = result?.Select(item => item.Text).ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }


        return returnValue;
    }

    public string Decode(Color32[] colors, int width, int height)
    {
        if (colors == null || colors.Length == 0 || width == 0 || height == 0)
        {
            return null;
        }

        string value = null;

        try
        {
            //Scanner.DecodeMultiple()
            var result = Scanner.Decode(colors, width, height);

            if (result != null)
            {
                value = result.Text;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return value;
    }
}
