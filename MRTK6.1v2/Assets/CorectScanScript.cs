using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorectScanScript : MonoBehaviour
{
    [SerializeField]
    Text uiText;
    public RawImage box;
    public Texture check;

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
        uiText.text = "Correct scan";
        box.texture = check;
    }


}
