using UnityEngine;
using UnityEngine.UI;

public class ScanFeedback : MonoBehaviour
{
    //Istate parent;

    private Texture correctScan;
    private Texture CorrectScan { get { return correctScan; } set { correctScan = value; } }

    private Texture inCorrectScan;
    private Texture InCorrectScan { get { return inCorrectScan; } set { inCorrectScan = value; } }

    private RawImage scanImg;
    private Texture ScanImg { get { return correctScan; } set { correctScan = value; } }

    public ScanFeedback(bool isCorrectScan, Texture correctScan, Texture inCorrectScan, RawImage scanImg /*, Istate parent*/)
    {

        this.correctScan = correctScan;
        this.inCorrectScan = inCorrectScan;
        this.scanImg = scanImg;

        isItemScanned(isCorrectScan);

    }
    public void isItemScanned(bool isScannedCorrectly)
    {

        if (isScannedCorrectly)
        {
            scanImg.texture = correctScan;
        }
        if (!isScannedCorrectly)
        {
            scanImg.texture = inCorrectScan;
        }

    }
    private void OnDestroy()
    {
        
    }

}
