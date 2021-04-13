using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Print()
    {
        Debug.Log("printing");

    }

    public void PrintWithString(TextMeshPro param)
    {
        Debug.Log("Printing: " + param.text.ToString());
    }
}
