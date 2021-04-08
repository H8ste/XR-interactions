using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IDecoder
    {
        string Decode(Color32[] colors, int width, int height);
    }
}


