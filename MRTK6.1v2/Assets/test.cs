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
        StartManualOrderPick();   
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

    public async void StartManualOrderPick(){
        var manualoph = gameObject.AddComponent<ManualOrderPickHandler>();

        var returnint = await manualoph.ManualOrderPick(new OrderItem[]{
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 0, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(50, 20, 12, 10, 10, "L"), 1, 0, 9, "Pizza", 251, true),
            new OrderItem(null, new LocPK(60, 20, 13, 11, 10, "L"), 2, 0, 1, "Smør", 24, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 3, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 4, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 5, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 6, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 7, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 8, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 9, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 10, 0, 1, "Frikadeller", 254, false),
            new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 11, 0, 1, "Frikadeller", 254, false),
            });

        Debug.Log("ManualOrderPickChosenID:" + returnint);

    }
}
