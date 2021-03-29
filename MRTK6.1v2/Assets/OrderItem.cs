using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderItem : IScrollableItem
{
    /* IScrollableItem Properties */
    private GameObject itemPrefab;
    public GameObject ItemPrefab { get { return itemPrefab; } set { itemPrefab = value; } }

    private OnClick onClick;
    public OnClick OnClick { get { return onClick; } set { onClick = value; } }

    private int scrollableID;
    public int ScrollableID { get { return scrollableID; } set { scrollableID = value; } }

    private GameObject instantiatedScrollableItem;
    public GameObject InstantiatedScrollableItem { get { return instantiatedScrollableItem; } set { instantiatedScrollableItem = value; } }

    /* OrderItem Properties */

    // private LockPK locationCode
    private short remainingStock;
    private short amountToTake;
    private string nameOfItem;
    private short itemID;
    private bool isScanned;
}
