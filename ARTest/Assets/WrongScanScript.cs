using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrongScanScript : MonoBehaviour
{
    [SerializeField]
    Text uiText;
    public RawImage box;
    public Texture cross;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Scan()
    {
        Debug.Log("scanned Correct");
        uiText.text = "wrong box, scan box 1";
        box.texture = cross;
    }
}
