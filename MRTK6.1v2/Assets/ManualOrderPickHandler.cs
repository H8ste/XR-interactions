using System;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using System.Linq;

public class ManualOrderPickHandler : MonoBehaviour /*, IState*/
{
    private OrderItem[] orderItems;

    public OrderItem[] OrderItems { get { return orderItems; } }

    private GameObject manualOrderItemPrefab;

    private ScrollHandler scrollHandler;

    private int manualOrderPickID = -1;


    private bool ManualOrderPickHasBeenSelected { get { return manualOrderPickID != -1; } }

    public async Task<int> ManualOrderPick(OrderItem[] availableOrderItems)
    {
        // sql view hopefully pulls with respect to performant route
        orderItems = availableOrderItems.OrderBy(orderItem => orderItem.IsScanned).ToArray();

        manualOrderItemPrefab = Resources.Load("Prefabs/ManualOrderItem", typeof(GameObject)) as GameObject;
        if (!manualOrderItemPrefab) Debug.LogError("Prefab ManualOrderItem does not exist");

        Enable();

        StartScroll();

        return await WaitForManualOrderPick();
    }

    private async Task<int> WaitForManualOrderPick()
    {
        // while (!ManualOrderPickHasBeenSelected)
        // {
        //     await Task.Delay(TimeSpan.FromSeconds(1));
        //     Debug.Log("still waiting");
        // }

        // manual order pick has been selected
        // return manualOrderPickID;
        return -1;
    }

    private void StartScroll()
    {
        scrollHandler = Instantiate((Resources.Load("Prefabs/ScrollHandler", typeof(GameObject)) as GameObject), transform).GetComponent<ScrollHandler>();


        // this next two operations are ugly, is there a better approach?

        // Instantiate each order item
        scrollHandler.Init(orderItems);

        // For each spawned order item, set correct properties
        print("length: " + orderItems.Length);
        foreach (var orderItem in orderItems)
        {
            print("setting properties on prefab");
            SetPropertiesOnInstantiatedInstance(orderItem);
        }

        scrollHandler.ItemSelected.AddListener((int returnVar) =>
        {
            Debug.Log("ScrollHandler finished: " + returnVar);
            manualOrderPickID = returnVar;
        });
    }

    // private void 
    private void SetPropertiesOnInstantiatedInstance(OrderItem orderItem)
    {
        // setting properties on prefab

        // scan state
        orderItem.InstantiatedScrollableItem.GetComponentInChildren<ImageHandler>()?.SetImage(orderItem.IsScanned);

        // loc code
        var locPK = FindChildWithTag(orderItem.InstantiatedScrollableItem, "LocPK")?.GetComponent<TextMeshPro>();
        if (locPK != null && orderItem.LocationCode != null)
        {
            locPK.SetText(orderItem.LocationCode.GetLocPKAsString(), false);
        }
        else print("locPK");


        // amount to pick
        var amount = FindChildWithTag(orderItem.InstantiatedScrollableItem, "Amount")?.GetComponent<TextMeshPro>();
        if (amount != null && orderItem.AmountToTake.HasValue)
        {
            amount.SetText(orderItem.AmountToTake.ToString(), false);
        }
        else print("amount");

        // item name
        var itemName = FindChildWithTag(orderItem.InstantiatedScrollableItem, "ItemName")?.GetComponent<TextMeshPro>();

        if (itemName != null && orderItem.NameOfItem != null)
        {
            itemName.SetText(orderItem.NameOfItem, false);
            print("succes: nameOfItem" + orderItem.NameOfItem);
        }
        else print("itemname");
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

    }

    private GameObject FindChildWithTag(GameObject root, string tag)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            print(t.tag);
            if (t.tag == tag) return t.gameObject;
        }

        Debug.LogError("Couldn't find child with tag");
        return null;

    }
}
