using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ScrollHandler : MonoBehaviour, IMixedRealityInputActionHandler
{
    /* ScrollHandler Properties */

    // UnityEvent that is invoked when a scrollableItem has been selected by a user
    // returns the scrollableItem.ID
    OnClick itemSelected;
    public UnityEvent<int> ItemSelected { get { return itemSelected; } }

    // reference to the scrollableitems to render - is passed by whatever entity begins a scrollHandler
    IScrollableItem[] scrollableItems;
    public IScrollableItem[] ScrollableItems { get { return scrollableItems; } }


    /* SelectionPointer Properties */

    // pointer-prefab used to give the user the feedback of which scrollable item is currently hovered/selected
    private GameObject selectionPrefab;
    private GameObject SelectionPrefab { get { return selectionPrefab; } set { selectionPrefab = value; } }

    // reference to the instantiated pointer-prefab - used to relocate its gameobject to a different gameobject 
    //(when a different scrollable item is hit)
    private GameObject instantiatedScrollablePointer;
    private GameObject InstantiatedScrollablePointer { get { return instantiatedScrollablePointer; } set { instantiatedScrollablePointer = value; } }

    // ref to the transform that acts as parent to all spawned scrollable items
    [SerializeField]
    private Transform contentTransform;


    /* Scrollable Items Settings */

    // presents content horizontally or vertically
    [SerializeField]
    private bool isHorizontal = true;
    public bool IsHorizontal { get { return isHorizontal; } }

    // spacing between each presented item 
    // 100% = full size of a scrollableItem
    // 33% = 1/3 size of a scrollable item
    [SerializeField]
    private float spacingPercentage = 100f;

    // the world-space spacing between each spawned elements with respect to spacingPercentage
    float spacing;
    private float Spacing { get { return spacing; } set { spacing = value; } }

    // the world-space length of scrollable items  
    // if presenting content horizontal:    uses width.
    // if present content vertical:         uses height.
    private float scrollableItemLength;
    public float ScrollableItemLength { get { return scrollableItemLength; } set { scrollableItemLength = value; } }

    // how far away from the player the scrollable items are spawned 
    [SerializeField]
    private float radius = 2f;

    public float Radius { get { return radius; } }

    // how far, in a viewing angle starting from 0*, can the camera rotate [capStartDegree .. capEndDegree]
    private float capStartDegree = 45f;
    private float capEndDegree = -45f;


    /* OnClick relevant properties */

    // reference to the most previous hit scrollable item
    private Interactable previousHitScrollableItem;
    public Interactable PreviousHitScrollableItem { get { return previousHitScrollableItem; } private set { previousHitScrollableItem = value; } }


    /* Unity Methods */

    /// <summary>
    /// Update runs once per frame
    /// </summary>
    private void Update()
    {
        PositionContentAndOffset();
    }

    /// <summary>
    /// FixedUpdate can run once, zero, or several times per frame, depending on how many physics frames per second are set in the time settings, and how fast/slow the framerate is.
    /// </summary>
    private void FixedUpdate()
    {
        // Bit shift the index of the layer (10) to get a bit mask
        // This will cast rays only against colliders in layer 10.
        int layerMask = 1 << 10;

        var directionVector = Camera.main.transform.TransformDirection(Vector3.forward);

        if (IsHorizontal)
        {
            directionVector.y = 0;
        }

        // instatiate and shoot a ray from camera out towards scrollable items
        if (Physics.Raycast(Camera.main.transform.position, directionVector, out RaycastHit hit, radius + 1f, layerMask))
        {
            Debug.DrawRay(Camera.main.transform.position, directionVector * 1000, Color.green);
            // place ScrollablePointer at the hit object's transform as its child (also changes world-space coordinates of ScrollablePointer)
            switch (InstantiatedScrollablePointer)
            {
                case null:
                    InstantiatedScrollablePointer = Instantiate(SelectionPrefab, hit.transform, false);
                    break;
                default:
                    InstantiatedScrollablePointer.transform.SetParent(hit.transform, false);
                    break;
            }

            // give the user feedback by setting the state of the hit component to "focused"
            // this triggers a different color to render on the hovered scrollable item
            PreviousHitScrollableItem = hit.transform.GetComponent<Interactable>();
            foreach (var item in gameObject.GetComponentsInChildren<Interactable>())
            {
                if (PreviousHitScrollableItem == item) PreviousHitScrollableItem.HasFocus = false;
                else item.HasFocus = false;
            }
        }
        else
        {
            Debug.DrawRay(Camera.main.transform.position, directionVector * 1000, Color.red);
        }
    }


    /* Public Methods */

    /// <summary>
    /// Instantiates the scrollHandler with passed scrollableItems
    /// </summary>
    /// <param name="scrollableItems">A set of scrollableItems to be handled/rendered by the initialised ScrollHandler</param>
    public ScrollHandler Instantiate(IScrollableItem[] scrollableItems)
    {
        SelectionPrefab = Resources.Load("Prefabs/SelectionPointer", typeof(GameObject)) as GameObject;
        if (!SelectionPrefab)
        {
            throw new System.Exception("Prefab SelectionPointer does not exist");
        }

        itemSelected = new OnClick();

        this.scrollableItems = scrollableItems;

        Populate();

        return this;
    }

    /// <summary>
    /// Switches (toggles) the rendered orientation of the scrollable items (vertical/horizontal)
    /// </summary>
    /// <param name="scrollableItems">A set of scrollableItems to be handled/rendered by the initialised ScrollHandler</param>
    public void SwitchOrientation()
    {
        isHorizontal = !isHorizontal;

        Spacing = GetSpacing(scrollableItems[0]);
        ScrollableItemLength = GetLength(scrollableItems[0]);
    }

    /* Private Methods */

    /// <summary>
    /// Positions the content in local space aswell as applying an offset (based on user's viewing angle left/right) 
    /// </summary>
    private void PositionContentAndOffset()
    {
        var calculations = CalculateStartEndAndIncrementDegrees();

        PositionContent(calculations.startDegree, calculations.endDegree, calculations.degreeIncrement);
        PositionOffset(calculations.startDegree, calculations.endDegree);
    }

    /// <summary>
    /// Calculates required values for placing scrollable items in the world. Based on radius, spacing between scrollable items, and size/length of scrollable items:
    /// A starting degree and end degree is calulcated and returned along with a respective degree increment
    /// </summary>
    private (float startDegree, float endDegree, float degreeIncrement) CalculateStartEndAndIncrementDegrees()
    {
        var perimeter = 2 * Mathf.PI * radius;
        var spacingInRadian = Helper.Map(0, 1, 0, 2 * Mathf.PI, Spacing / perimeter);
        var spacingInDegrees = Mathf.Rad2Deg * spacingInRadian;

        var itemLengthInRadian = Helper.Map(0, 1, 0, 2 * Mathf.PI, ScrollableItemLength / perimeter);
        var itemLengthInDegree = Mathf.Rad2Deg * itemLengthInRadian;

        var totalSpanDegree = ScrollableItems.Length * (spacingInDegrees + itemLengthInDegree);

        var startDegree = 0 - totalSpanDegree / 2;
        var endDegree = totalSpanDegree / 2;

        return (startDegree, endDegree, totalSpanDegree / ScrollableItems.Length);
    }

    /// <summary>
    /// Positions the offset with respect to the user's rotation
    /// enables an interaction where a user can rotate their head 45* degrees to the left
    /// but be shown a rendered element that was placed at e.g., 250* degrees
    /// </summary>
    private void PositionOffset(float startDegree, float endDegree)
    {
        var beginSegment = startDegree - capStartDegree;
        var endSegment = Mathf.Abs(endDegree - capEndDegree);

        Quaternion contentOffset = GetOffsetAndHideNonVisibleItems(beginSegment, endSegment, startDegree, endDegree);

        contentTransform.localRotation = contentOffset;

        Quaternion cameraRotFixed = IsHorizontal ?
            Quaternion.Euler(new Vector3(0, (Camera.main.transform.rotation).eulerAngles.y, 0)) :
            Quaternion.Euler(new Vector3((Camera.main.transform.rotation).eulerAngles.x, (Camera.main.transform.rotation).eulerAngles.y, 0));
        transform.SetPositionAndRotation(Camera.main.transform.position, cameraRotFixed);
    }

    /// <summary>
    /// Returns a calculated offset with respect to the player's viewing angle as a quaternion.
    /// With respect to the player's viewing angle, hides any rendered items that reside outside the player fov 
    /// </summary>
    /// <param name="beginSegment">the span in degrees towards the start of the list</param>
    /// <param name="endSegment">the span in degrees towards the end of the list</param>
    /// <param name="startDegree">the degree at which the start of the list was spawned (can be below or higher than beginSegment)</param>
    /// <param name="endDegree">the degree at which the end of the list was spawned (can be below or higher than endSegment)</param>
    private Quaternion GetOffsetAndHideNonVisibleItems(float beginSegment, float endSegment, float startDegree, float endDegree)
    {
        var angleValue = IsHorizontal ? Camera.main.transform.rotation.eulerAngles.y : Camera.main.transform.rotation.eulerAngles.x;
        var percentile = GetPercentile(angleValue, startDegree, endDegree);
        var rotation = GetOffset(angleValue, percentile, beginSegment, endSegment, startDegree, endDegree);

        var fixedAngleValue = rotation;

        // as the scrollHandler can spawn overlapping items
        // hide spawned scrollable items that does not reside within the player's anglevalue +- 45* degrees
        foreach (var scrollableItem in scrollableItems.OrderBy(item => item.SpawnedDegreeAngle))
        {
            var angleFOV = 70f;
            if (IsWithinView(scrollableItem, fixedAngleValue, angleFOV))
            {
                scrollableItem.InstantiatedScrollableItem.SetActive(true);
            }
            else
            {
                scrollableItem.InstantiatedScrollableItem.SetActive(false);
            }
        }

        return IsHorizontal ?
            Quaternion.Euler(new Vector3(0, -rotation, 0)) :
            Quaternion.Euler(new Vector3(-rotation, 0, 0));
    }

    /// <summary>
    /// Computes whether the item passed is within the current angle with respect to some angleFOV
    /// </summary>
    /// <param name="scrollableItem">a scrollable item to check for</param>
    /// <param name="currentAngle">the current viewing angle of the player</param>
    /// <param name="angleFOV">the angle to live within</param>
    private bool IsWithinView(IScrollableItem scrollableItem, float currentAngle, float angleFOV)
    {
        return (
            (scrollableItem.SpawnedDegreeAngle) < (currentAngle + angleFOV) &&
             scrollableItem.SpawnedDegreeAngle > (currentAngle - angleFOV));
    }

    /// <summary>
    /// Calculates to what degree [0%, 100%] the user has fully turned their head towards the start/end of the scrollable items
    /// 100% = fully reached the start/end degree*
    /// </summary>
    private float GetPercentile(float rotation, float startDegree, float endDegree)
    {
        // returns [0 .. 1] to what degree user is "aimed" towards either start or end caps (-45* / +45*)
        return GazingTowardsEnd(rotation) ?
                Mathf.Clamp(rotation / Mathf.Abs(capEndDegree), 0, 1)
                : Mathf.Clamp(-1 * (rotation - 360) / Mathf.Abs(capStartDegree), 0, 1);
    }

    /// <summary>
    /// Calculates the rotational offset based on the player's viewing angle.
    /// </summary>
    /// <param name="angleValue">the player's current angle--></param>
    /// <param name="percentile">to what degree [0%, 100%] the user has fully turned their head towards the start/end of the scrollable items</param>
    /// <param name="beginSegment">the span in degrees towards the start of the list</param>
    /// <param name="endSegment">the span in degrees towards the end of the list</param>
    /// <param name="startDegree">the degree at which the start of the list was spawned (can be below or higher than beginSegment)</param>
    /// <param name="endDegree">the degree at which the end of the list was spawned (can be below or higher than endSegment)</param>

    private float GetOffset(float angleValue, float percentile, float beginSegment, float endSegment, float startDegree, float endDegree)
    {
        var maxEnd = Mathf.Clamp(Mathf.Abs(endSegment), 0, Mathf.Max(Mathf.Abs(capEndDegree), Mathf.Abs(endDegree)));
        var maxStart = Mathf.Clamp(Mathf.Abs(beginSegment), 0, Mathf.Max(Mathf.Abs(capStartDegree), Mathf.Abs(startDegree)));
        // with respect to given angle value, percentile, beginSegment and endSegment, compute local-space camera rotation
        return percentile * (GazingTowardsEnd(angleValue) ? maxEnd : -maxStart);
    }

    /// <summary>
    /// For each provided scrollable item, a gameobject is instantiated and referenced within the scrollableitem itself for future use
    /// </summary>
    private void Populate()
    {
        // spawn each scrollableItem (with proper parenting within Unity)
        foreach (var scrollableItem in ScrollableItems)
        {
            scrollableItem.InstantiatedScrollableItem = Instantiate(scrollableItem.ItemPrefab, contentTransform);
            if (Spacing == default(float)) Spacing = GetSpacing(scrollableItem);
            if (ScrollableItemLength == default(float)) ScrollableItemLength = GetLength(scrollableItem);
        }
    }

    /// <summary>
    /// Returns, with respect to spacingpercentage, spacing as a length in world space
    /// </summary>
    /// <param name="scrollableItem">the scrollable item used to calculate spacing</param>
    private float GetSpacing(IScrollableItem scrollableItem)
    {
        return (spacingPercentage / 100f) * GetLength(scrollableItem);
    }

    /// <summary>
    /// Returns the world space length (height if rendering vertical | length if rendering horizontally) of the passed scrollableItem
    /// </summary>
    /// <param name="scrollableItem">the scrollable item used to calculate length</param>
    private float GetLength(IScrollableItem scrollableItem)
    {
        // returns with respect to scrollhandler-mode the world-space size of the scrollable item
        return IsHorizontal ?
            scrollableItem.InstantiatedScrollableItem.GetComponent<Collider>().bounds.size.x :
            scrollableItem.InstantiatedScrollableItem.GetComponent<Collider>().bounds.size.y;
    }

    /// <summary>
    /// With respect to passed starting degree, ending degree and degree increment, place instantiated scrollable items into the world
    /// </summary>
    /// <param name="startDegree">the degree at which the start of the list was spawned</param>
    /// <param name="endDegree">the degree at which the end of the list was spawned</param>
    /// <param name="degreeIncrement">the degree increment at which scrollable items are spawned at</param>

    private void PositionContent(float startDegree, float endDegree, float degreeIncrement)
    {
        var currentDegree = startDegree + degreeIncrement / 2;

        foreach (var scrollableItem in ScrollableItems)
        {
            Vector3 newPos = IsHorizontal ?
                new Vector3(Mathf.Sin(currentDegree * Mathf.Deg2Rad) * radius, 0, Mathf.Cos(currentDegree * Mathf.Deg2Rad) * radius) :
                new Vector3(0, Mathf.Sin(currentDegree * Mathf.Deg2Rad) * radius, Mathf.Cos(currentDegree * Mathf.Deg2Rad) * radius);

            // setting position and spawned angle
            scrollableItem.InstantiatedScrollableItem.transform.localPosition = newPos;
            scrollableItem.SpawnedDegreeAngle = currentDegree;

            // setting rotation
            var targetDirection = scrollableItem.InstantiatedScrollableItem.transform.position - Camera.main.transform.position;
            var newRotation = Vector3.RotateTowards(scrollableItem.InstantiatedScrollableItem.transform.forward, targetDirection, 90f, 0.0f);
            scrollableItem.InstantiatedScrollableItem.transform.rotation = Quaternion.LookRotation(newRotation);

            currentDegree += degreeIncrement;
        }
    }

    /// <summary>
    /// With respect to the player's viewing-rotation, computes and returns wether they're looking towards end of the list.
    /// </summary>
    /// <param name="cameraRotation">the player's viewing-rotation [horisontal: y] [vertical: x]</param>
    private bool GazingTowardsEnd(float cameraRotation)
    {
        return (cameraRotation < 360 && cameraRotation <= 180);
    }

    /* MRTK Relevant Methods */

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

    // is triggered if all other gameobjects in scene did not respond to an Action - works as a fallback
    // only way we can register actions on gameobjects like these, where focus shouldn't be 
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (PreviousHitScrollableItem)
        {
            // a display has been hit (selected)
            PreviousHitScrollableItem.SetInputDown();
            StartCoroutine(WaitFor(1f));
        }
        else
        {
            Debug.Log("hit display not been set");
        }
    }

    private IEnumerator<UnityEngine.WaitForSeconds> WaitFor(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // finds the scrollable item and its ID which instantiatedscrollableitem is equal to previous hit scrollable item
        var clickedID = ScrollableItems.Where(scrollableItem => scrollableItem.InstantiatedScrollableItem == PreviousHitScrollableItem.gameObject).SingleOrDefault()?.ScrollableID;
        // if no scrollable item could be found, a -1 is returned and expected to be handled in what started the scrollhandler
        ItemSelected.Invoke(clickedID ?? -1);
    }
}
