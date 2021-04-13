using System;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using System.Linq;
using System.Threading;
using UnityEngine.Events;

public class ManualOrderPickHandler : MonoBehaviour, ITab
{
    public OrderItem[] OrderItems { get { return dataHandler?.AllOrderItems; } }

    private GameObject manualOrderItemPrefab;

    private ScrollHandler scrollHandler;

    CancellationTokenSource cancellationTokenSource;

    private DataHandler dataHandler;


    public async void Enable()
    {
        // order item's itemprefab will have to be setup (as it might have been set differently)
        foreach (var item in OrderItems)
        {
            item.SwitchPrefab(manualOrderItemPrefab);
        }

        StartScroll();

        // logic denoting what to do with 
    }

    public void Disable()
    {
        // close any possible loose ends
        cancellationTokenSource.Cancel();

        // remove scrollHandler
        scrollHandler.ItemSelected.RemoveListener(ItemClicked);
        Destroy(scrollHandler);
    }



    public ITab Construct(DataHandler dataHandler)
    {
        this.dataHandler = dataHandler;

        manualOrderItemPrefab = Resources.Load("Prefabs/ManualOrderItem", typeof(GameObject)) as GameObject;
        if (!manualOrderItemPrefab) Debug.LogError("Prefab ManualOrderItem does not exist");

        return this;
    }

    private void ItemClicked(int clickedItemIndex)
    {

        Debug.Log("ScrollHandler finished: " + clickedItemIndex);


        // logic denoting what to do now with clicked orderItem
        // switch tab etc.

        throw new NotImplementedException();
    }

    /// <summary>
    /// Instantiates a scrollHandler, initialises it with orderItems, sets properties on instantiated order item prefabs, and subscribes to the scrollHandler's ItemSelected event
    /// </summary>
    private void StartScroll()
    {
        scrollHandler = scrollHandler ?? Instantiate((Resources.Load("Prefabs/ScrollHandler", typeof(GameObject)) as GameObject), transform).GetComponent<ScrollHandler>();

        // this next two operations are ugly, is there a better approach?

        // Instantiate each order item
        scrollHandler.Init(OrderItems);

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
        var locPK = Helper.FindChildWithTag(orderItem.InstantiatedScrollableItem, "LocPK")?.GetComponent<TextMeshPro>();
        if (locPK != null && orderItem.LocationCode != null)
        {
            locPK.SetText(orderItem.LocationCode.GetLocPKAsString(), false);
        }

        // amount to pick
        var amount = Helper.FindChildWithTag(orderItem.InstantiatedScrollableItem, "Amount")?.GetComponent<TextMeshPro>();
        if (amount != null && orderItem.AmountToTake.HasValue)
        {
            amount.SetText(orderItem.AmountToTake.ToString(), false);
        }

        // item name
        var itemName = Helper.FindChildWithTag(orderItem.InstantiatedScrollableItem, "ItemName")?.GetComponent<TextMeshPro>();

        if (itemName != null && orderItem.NameOfItem != null)
        {
            itemName.SetText(orderItem.NameOfItem, false);
        }
    }

    /// <summary>
    /// Is called once the gameobject that ManualOrderPickHandler is attached to is set to be destroyed by Unity
    /// (e.g., when unity closes)
    /// </summary>
    private void OnDestroy()
    {
        Disable();
    }
}
