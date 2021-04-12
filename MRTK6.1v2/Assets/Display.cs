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
            Debug.LogError("location code is null");
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


        locationCode.text = orderItemToSet.LocationCode.GetLocPKAsString();
        remainingStock.text = "Lag: " + orderItemToSet.RemainingStock.ToString();
        amountToTake.text = "Stk: " + orderItemToSet.AmountToTake.ToString();
        nameOfItem.text = orderItemToSet.NameOfItem;
        itemIDText.text = "ID: " + orderItemToSet.ItemID.ToString();
        
    }
    
    
  
}
