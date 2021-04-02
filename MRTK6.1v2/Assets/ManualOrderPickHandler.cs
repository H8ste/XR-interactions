using System;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using System.Linq;
using System.Threading;

public class ManualOrderPickHandler : MonoBehaviour /*, IState*/
{
    private OrderItem[] orderItems;

    public OrderItem[] OrderItems { get { return orderItems; } }

    private GameObject manualOrderItemPrefab;

    private ScrollHandler scrollHandler;

    private int manualOrderPickID = -1;

    CancellationTokenSource cancellationTokenSource;

    private bool ManualOrderPickHasBeenSelected { get { return manualOrderPickID != -1; } }


    /// <summary>
    /// Starts a Manual Order Pick with the provided orderItems
    /// </summary>
    /// <param name="availableOrderItems">old minimum of provided value</param>
    public async Task<int> ManualOrderPick(OrderItem[] availableOrderItems)
    {
        // sql view hopefully pulls with respect to performant route
        orderItems = availableOrderItems.OrderBy(orderItem => orderItem.IsScanned).ToArray();

        manualOrderItemPrefab = Resources.Load("Prefabs/ManualOrderItem", typeof(GameObject)) as GameObject;
        if (!manualOrderItemPrefab) Debug.LogError("Prefab ManualOrderItem does not exist");

        Enable();

        StartScroll();

        cancellationTokenSource = new CancellationTokenSource();
        return await WaitForManualOrderPick(cancellationTokenSource);
    }


    /// <summary>
    /// Coroutine that returns the selected orderItem once scrollHandler has invoked its ItemSelected Event
    /// </summary>
    private async Task<int> WaitForManualOrderPick(CancellationTokenSource cts)
    {
        while (!ManualOrderPickHasBeenSelected)
        {
            // if at any point cancellation is requested
            if (cts.IsCancellationRequested)
            {
                return -1;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
            Debug.Log("still waiting");
        }

        // manual order pick has been selected
        return manualOrderPickID;
    }



    /// <summary>
    /// Instantiates a scrollHandler, initialises it with orderItems, sets properties on instantiated order item prefabs, and subscribes to the scrollHandler's ItemSelected event
    /// </summary>
    private void StartScroll()
    {
        scrollHandler = Instantiate((Resources.Load("Prefabs/ScrollHandler", typeof(GameObject)) as GameObject), transform).GetComponent<ScrollHandler>();

        // this next two operations are ugly, is there a better approach?

        // Instantiate each order item
        scrollHandler.Init(orderItems);

        // For each spawned order item, set correct properties
        foreach (var orderItem in orderItems)
        {
            SetPropertiesOnInstantiatedInstance(orderItem);
        }

        scrollHandler.ItemSelected.AddListener((int returnVar) =>
        {
            Debug.Log("ScrollHandler finished: " + returnVar);
            manualOrderPickID = returnVar;
        });
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

    private void Enable()
    {
        // order item's itemprefab will have to be setup (as it might have been set differently)
        foreach (var item in orderItems)
        {
            item.SwitchPrefab(manualOrderItemPrefab);
        }
    }

    private void Disable()
    {
        // close any possible loose ends
        cancellationTokenSource.Cancel();
    }



    private void OnDestroy()
    {
        Disable();
    }
}
