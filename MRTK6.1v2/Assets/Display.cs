using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Display: MonoBehaviour
{
    [SerializeField]
    private TextMeshPro locationCode;
    [SerializeField]
    private TextMeshPro remainingStock;
    [SerializeField]
    private TextMeshPro amountToTake;
    [SerializeField]
    private TextMeshPro nameOfItem;
    [SerializeField]
    private TextMeshPro itemIDText;


    RackVisualizer visualizer;

    public void SetInformation(OrderItem orderItemToSet, bool createVisualizer = false)
    {
        if(locationCode == null)
        {
            
        }
        if (remainingStock   == null)
        {

        }
        if (amountToTake == null)
        {

        }
        if (nameOfItem == null)
        {

        }
        if (itemIDText == null)
        {

        }

        //locationCodeText = informationToShow (Make this once the location code is up and running)
        remainingStock.text = orderItemToSet.RemainingStock.ToString();
        amountToTake.text = orderItemToSet.AmountToTake.ToString();
        nameOfItem.text = orderItemToSet.NameOfItem;
        itemIDText.text = orderItemToSet.ItemID.ToString();


    }

  
}
