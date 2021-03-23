using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuRaycaster : MonoBehaviour, IMixedRealityInputActionHandler
{
    /* ScrollHandler properties*/
    [SerializeField]
    ScrollingObjectCollection scrollHandler;
    GridObjectCollection menuGrid;
    ClippingBox clippingHandler;
    List<GameObject> buttons;

    /* Raycast properties */
    int layerMask = 1 << 10;
    RaycastHit hit;

    // Last hit item, used to trigger input events e.g., activeItem.SetInputDown()
    Interactable activeItem;

    /* States regarding current interactions with scroller */
    bool isDragging = false;
    int taps = 0;

    // Local reference to Unity Game Object name for lookup
    string frontPlaneName = "SneakyPlane";

    /* Public Methods */

    /// <summary>
    /// used to setup the ScrollHandler with appropriate references for gridCollection (renderes) + clippingbox
    /// used internally on start, and externally if switching between seperate scroll handlers (vertical/horizontal)
    /// </summary>
    /// <param name="newScrollHandler">the new scrollHandler to switch to</param>
    public void SetupScrollHandler(GameObject newScrollHandler)
    {
        scrollHandler = newScrollHandler.GetComponentInChildren<ScrollingObjectCollection>();
        menuGrid = scrollHandler.GetComponentInChildren<GridObjectCollection>();
        buttons = menuGrid.GetComponentsInChildren<Interactable>().Select(item => item.gameObject).ToList();
        clippingHandler = scrollHandler.GetComponentInChildren<ClippingBox>();
        clippingHandler.ClearRenderers();
        foreach (var button in buttons)
        {
            foreach (var renderer in button.GetComponentsInChildren<Renderer>())
            {
                clippingHandler.AddRenderer(renderer);
            }
        }

        scrollHandler.UpdateContent();
    }


    /// <summary>
    /// Active scrollHandler is, through unity's event system, binded to call this method on drag start
    /// </summary>
    public void DragStart()
    {
        isDragging = true;
    }

    /* Private Methods */
    void Start()
    {
        if (scrollHandler == null)
            Debug.LogError("Not all parameters are set");
        SetupScrollHandler(scrollHandler.gameObject);
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.collider.gameObject.GetComponent<Interactable>() is Interactable hitInteractable)
            {
                activeItem = hitInteractable;
                activeItem.HasFocus = true;

                Transform frontPlane = hit.collider.gameObject.transform.Find(frontPlaneName);
                frontPlane.gameObject.SetActive(false);
                foreach (var item in buttons)
                {
                    if (item.GetComponent<Interactable>() is Interactable itemInteractable)
                    {
                        if (itemInteractable != activeItem)
                        {
                            itemInteractable.HasFocus = false;
                            var plane = item.transform.Find(frontPlaneName);
                            plane.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.PushFallbackInputHandler(gameObject);
    }
    private void OnDisable()
    {
        CoreServices.InputSystem?.PopFallbackInputHandler();
    }

    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        // not used
    }

    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        taps++;
        if (isDragging)
        {
            isDragging = false;
            taps = 0;
        }
        else
        {
            if (activeItem && taps >= 2)
            {
                activeItem.SetInputDown();
                taps = 0;
            }
        }
    }
}
