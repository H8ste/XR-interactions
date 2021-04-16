using UnityEngine;
using ZXing;

namespace Assets.Scripts.Interfaces
{
    public interface IDecoder
    {
        Result[] DecodeMultiple(byte[] rawRgb, int width, int height, RGBLuminanceSource.BitmapFormat format);
        Result DecodeSingle(Color32[] colors, int width, int height);
    }
}


