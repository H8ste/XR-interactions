using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderItem: IScrollableItem
{
    /* IScrollableItem Properties */
    private GameObject instantiatedScrollableItem;
    public GameObject InstantiatedScrollableItem { get { return instantiatedScrollableItem; } set { instantiatedScrollableItem = value; } }

    private GameObject itemPrefab;
    public GameObject ItemPrefab {get {return itemPrefab; }set {itemPrefab = value;}}  

    private OnClick onClick;
    public OnClick OnClick {get {return onClick;} set {onClick = value;}} 

    private int scrollableID;
    public int ScrollableID {get {return scrollableID;} set {scrollableID = value;}}

    /* OrderItem Properties */

    // private LockPK locationCode
    
    private short remainingStock;
    private short amountToTake;
=======
    public int ScrollableID { get { return scrollableID; } set { scrollableID = value; } }

    private GameObject instantiatedScrollableItem;
    public GameObject InstantiatedScrollableItem { get { return instantiatedScrollableItem; } set { instantiatedScrollableItem = value; } }

    private float spawnedDegreeAngle;
    public float SpawnedDegreeAngle { get { return spawnedDegreeAngle; } set { spawnedDegreeAngle = value; } }


    /* OrderItem Properties */

    private LocPK locationCode;
    public LocPK LocationCode { get { return locationCode; } }

    private short? remainingStock;
    public short? RemainingStock { get { return remainingStock; } }

    private short? amountToTake;
    public short? AmountToTake { get { return amountToTake; } }

>>>>>>> #ManualOrderPick
    private string nameOfItem;
    public string NameOfItem { get { return nameOfItem; } }

    private short? itemID;
    public short? ItemID { get { return itemID; } }

    private bool isScanned;
    public bool IsScanned { get { return isScanned; } }


    public OrderItem(OnClick onClick, LocPK locationCode, int orderItemID, short remainingStock, short amountToTake, string nameOfItem, short itemID, bool isScanned)
    {
        this.onClick = onClick;
        this.locationCode = locationCode;
        this.scrollableID = orderItemID;
        this.remainingStock = remainingStock;
        this.amountToTake = amountToTake;
        this.nameOfItem = nameOfItem;
        this.itemID = itemID;
        this.isScanned = isScanned;
    }

    public void SwitchPrefab(GameObject newPrefab)
    {
        itemPrefab = newPrefab;
    }
}
