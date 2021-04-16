using System;
using UnityEngine;
using TMPro;
using System.Threading;

public class ManualOrderPickHandler : MonoBehaviour, ITab
{
    public OrderItem[] OrderItems { get { return dataHandler?.AllOrderItems; } }

    private GameObject manualOrderItemPrefab;

    private ScrollHandler scrollHandler;

    CancellationTokenSource cancellationTokenSource;

    private DataHandler dataHandler;


    /* ITab Methods */
    public ITab Instantiate(DataHandler dataHandler)
    {
        this.dataHandler = dataHandler;

        manualOrderItemPrefab = Resources.Load("Prefabs/ManualOrderItem", typeof(GameObject)) as GameObject;
        if (!manualOrderItemPrefab) Debug.LogError("Prefab ManualOrderItem does not exist");

        return this;
    }

    public void Enable()
    {
        // order item's itemprefab will have to be setup (as it might have been set differently)
        foreach (var item in OrderItems)
        {
            item.SwitchPrefab(manualOrderItemPrefab);
        }

        StartScroll(); 
    }

    public void Disable()
    {
        // close any possible loose ends
        cancellationTokenSource.Cancel();

        // remove scrollHandler
        scrollHandler.ItemSelected.RemoveListener(ItemClicked);
        Destroy(scrollHandler);
    }


    /* Private Methods */
    private void ItemClicked(int clickedItemIndex)
    {
        Debug.Log("ScrollHandler finished: " + clickedItemIndex);

        // todo: logic denoting what to do now with clicked orderItem
        // switch tab etc.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Instantiates a scrollHandler, initialises it with orderItems, sets properties on instantiated order item prefabs, and subscribes to the scrollHandler's ItemSelected event
    /// </summary>
    private void StartScroll()
    {
        scrollHandler = scrollHandler ?? 
            Instantiate((Resources.Load("Prefabs/ScrollHandler", typeof(GameObject)) as GameObject), transform).GetComponent<ScrollHandler>().Instantiate(OrderItems);

        // For each spawned order item, set correct properties
        foreach (var orderItem in OrderItems)
        {
            SetPropertiesOnInstantiatedInstance(orderItem);
        }

        scrollHandler.ItemSelected.AddListener(ItemClicked);
    }

    /// <summary>
    /// Sets the provided orderItem's text/image elements within its instantiated object to their respective values
    /// </summary>
    /// <param name="orderItem">orderItem to set properties on its instantiated instance</param>
    private void SetPropertiesOnInstantiatedInstance(OrderItem orderItem)
    {
        // scan state
        orderItem.InstantiatedScrollableItem.GetComponentInChildren<ImageHandler>()?.SetImage(orderItem.IsScanned);

        // loc code
        if (orderItem.LocationCode != null && Helper.FindChildWithTag(orderItem.InstantiatedScrollableItem, "LockPK", out var locPK))
        {
            locPK.GetComponent<TextMeshPro>()?.SetText(orderItem.LocationCode.GetLocPKAsString(), false);
        }

        // amount to pick
        if (orderItem.AmountToTake.HasValue && Helper.FindChildWithTag(orderItem.InstantiatedScrollableItem, "Amount", out var amount))
        {
            amount.GetComponent<TextMeshPro>()?.SetText(orderItem.AmountToTake.ToString(), false);
        }

        // item name
        if (orderItem.NameOfItem != null && Helper.FindChildWithTag(orderItem.InstantiatedScrollableItem, "ItemName", out var itemName))
        {
            itemName.GetComponent<TextMeshPro>()?.SetText(orderItem.NameOfItem, false);
        }
    }

    /* Unity Methods */
    /// <summary>
    /// Is called once the gameobject that ManualOrderPickHandler is attached to is set to be destroyed by Unity
    /// (e.g., when unity closes)
    /// </summary>
    private void OnDestroy()
    {
        Disable();
    }
}
