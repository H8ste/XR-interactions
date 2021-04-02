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

    /* OrderItem Properties */

    // private LockPK locationCode
    private short remainingStock;
    public short RemainingStock { get { return remainingStock; } }

    private short amountToTake;
    public short AmountToTake { get { return amountToTake; } }

    private string nameOfItem;
    public string NameOfItem { get { return nameOfItem; } }

    private short itemID;

    public short ItemID { get { return itemID; } }

    private bool isScanned;
    public bool IsScanned { get { return isScanned; } }

    public OrderItem(short stock, short amount, string name, short ID, bool scanned)
    {
        remainingStock = stock;
        amountToTake = amount;
        nameOfItem = name;
        itemID = ID;
        isScanned = scanned;
    }

}
