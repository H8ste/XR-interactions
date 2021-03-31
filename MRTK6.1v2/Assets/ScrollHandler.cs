using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ScrollHandler : MonoBehaviour, IMixedRealityInputActionHandler
{
    /* ScrollHandler Properties */
    OnClick itemSelected;
    public UnityEvent<int> ItemSelected { get { return itemSelected; } }

    IScrollableItem[] scrollableItems;
    public IScrollableItem[] ScrollableItems { get { return scrollableItems; } }

    private float scrollableItemLength;
    public float ScrollableItemLength { get { return scrollableItemLength; } set { scrollableItemLength = value; } }

    private List<GameObject> spawnedScrollableItems = new List<GameObject>();
    public List<GameObject> SpawnedScrollableItems { get { return spawnedScrollableItems; } }


    /* SelectionPointer Properties */
    // pointer used to give the user the feedback of which scrollable item is currently hovered/selected

    private GameObject selectionPrefab;
    private GameObject SelectionPrefab { get { return selectionPrefab; } set { selectionPrefab = value; } }

    private GameObject spawnedScrollablePointer;
    private GameObject SpawnedScrollablePointer { get { return spawnedScrollablePointer; } set { spawnedScrollablePointer = value; } }

    [SerializeField]
    private string selectionTag;


    /* Scrollable Items Settings */
    [SerializeField]
    private bool isHorizontal = true;
    public bool IsHorizontal { get { return isHorizontal; } }

    [SerializeField]
    private float spacingPercentage = 100f;

    [SerializeField]
    private float radius = 2f;

    private float capStartDegree = 45f;
    private float capEndDegree = -45f;

    float spacing;
    private float Spacing { get { return spacing; } set { spacing = value; } }

    /* OnClick relevant properties */
    private Interactable previousHitScrollableItem;
    private Interactable PreviousHitScrollableItem { get { return previousHitScrollableItem; } set { previousHitScrollableItem = value; } }

    [SerializeField]
    private Transform contentTransform;

    public ScrollHandler Init(IScrollableItem[] scrollableItems)
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

    private void PositionCameraAndContent()
    {
        // NOL Debug
        // Spacing = GetSpacing(scrollableItems[0]);
        // ScrollableItemLength = GetLength(scrollableItems[0]);

        var calculations = CalculateStartEndAndIncrementDegrees();

        PositionCamera(calculations.startDegree, calculations.endDegree);
        PositionContent(calculations.startDegree, calculations.endDegree, calculations.degreeIncrement);
    }

    private (float startDegree, float endDegree, float degreeIncrement) CalculateStartEndAndIncrementDegrees()
    {
        var perimeter = 2 * Mathf.PI * radius;
        var spacingInRadian = Map(0, 1, 0, 2 * Mathf.PI, Spacing / perimeter);
        var spacingInDegrees = Mathf.Rad2Deg * spacingInRadian;

        var itemLengthInRadian = Map(0, 1, 0, 2 * Mathf.PI, ScrollableItemLength / perimeter);
        var itemLengthInDegree = Mathf.Rad2Deg * itemLengthInRadian;

        var totalSpanDegree = ScrollableItems.Length * (spacingInDegrees + itemLengthInDegree);

        var startDegree = 0 - totalSpanDegree / 2;
        var endDegree = totalSpanDegree / 2;
        return (startDegree, endDegree, totalSpanDegree / ScrollableItems.Length);
    }

    private void PositionCamera(float startDegree, float endDegree)
    {
        var beginSegment = startDegree - capStartDegree;
        var endSegment = Mathf.Abs(endDegree - capEndDegree);

        Quaternion contentOffset = GetCameraRotation(beginSegment, endSegment, startDegree, endDegree);

        contentTransform.localRotation = contentOffset;

        Quaternion cameraRotFixed = IsHorizontal ?
            Quaternion.Euler(new Vector3(0, (Camera.main.transform.rotation).eulerAngles.y, 0)) :
            Quaternion.Euler(new Vector3((Camera.main.transform.rotation).eulerAngles.x, (Camera.main.transform.rotation).eulerAngles.y, 0));
        transform.SetPositionAndRotation(Camera.main.transform.position, cameraRotFixed);
    }

    private Quaternion GetCameraRotation(float beginSegment, float endSegment, float startDegree, float endDegree)
    {
        var angleValue = IsHorizontal ? Camera.main.transform.rotation.eulerAngles.y : Camera.main.transform.rotation.eulerAngles.x;
        var percentile = GetPercentile(angleValue, startDegree, endDegree);
        var rotation = GetRotation(angleValue, percentile, beginSegment, endSegment, startDegree, endDegree);

        var fixedAngleValue = rotation;
        // hide spawned objects that does not reside within anglevalue +- 45 degrees
        foreach (var scrollableItem in scrollableItems.OrderBy(item => item.SpawnedDegreeAngle))
        {
            var angleFOV = 70f;
            if ((scrollableItem.SpawnedDegreeAngle) < (fixedAngleValue + angleFOV) && scrollableItem.SpawnedDegreeAngle > (fixedAngleValue - angleFOV))
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

    private float GetPercentile(float rotation, float startDegree, float endDegree)
    {
        // returns [0 .. 1] to what degree user is "aimed" towards either start or end caps (-45* / +45*)
        return isDown(rotation) ?
                Mathf.Clamp(rotation / Mathf.Abs(capEndDegree), 0, 1)
                : Mathf.Clamp(-1 * (rotation - 360) / Mathf.Abs(capStartDegree), 0, 1);
    }

    private float GetRotation(float angleValue, float percentile, float beginSegment, float endSegment, float startDegree, float endDegree)
    {
        // print("endDegree: " + endDegree + ". capEndDegree: " + capEndDegree);
        // print("startDegree: " + startDegree + ". capStartDegree: " + capStartDegree);
        var maxEnd = Mathf.Clamp(Mathf.Abs(endSegment), 0, Mathf.Max(Mathf.Abs(capEndDegree), Mathf.Abs(endDegree)));
        var maxStart = Mathf.Clamp(Mathf.Abs(beginSegment), 0, Mathf.Max(Mathf.Abs(capStartDegree), Mathf.Abs(startDegree)));
        // with respect to given angle value, percentile, beginSegment and endSegment, compute local-space camera rotation
        return percentile * (isDown(angleValue) ? maxEnd : -maxStart);
    }

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
    /// Returns, with respect to given spacingpertange, spacing as a length in world space
    /// </summary>
    private float GetSpacing(IScrollableItem scrollableItem)
    {
        return (spacingPercentage / 100f) * GetLength(scrollableItem);
    }

    private float GetLength(IScrollableItem scrollableItem)
    {
        // returns with respect to scrollhandler-mode the world-space size of the scrollable item
        return IsHorizontal ?
            scrollableItem.InstantiatedScrollableItem.GetComponent<Collider>().bounds.size.x :
            scrollableItem.InstantiatedScrollableItem.GetComponent<Collider>().bounds.size.y;
    }

    private void PositionContent(float startDegree, float endDegree, float degreeIncrement)
    {
        var currentDegree = startDegree + degreeIncrement / 2;

        foreach (var scrollableItem in ScrollableItems)
        {

            Vector3 newPos = IsHorizontal ?
                new Vector3(Mathf.Sin(currentDegree * Mathf.Deg2Rad) * radius, 0, Mathf.Cos(currentDegree * Mathf.Deg2Rad) * radius) :
                new Vector3(0, Mathf.Sin(currentDegree * Mathf.Deg2Rad) * radius, Mathf.Cos(currentDegree * Mathf.Deg2Rad) * radius);

            // setting position
            scrollableItem.InstantiatedScrollableItem.transform.localPosition = newPos;
            scrollableItem.SpawnedDegreeAngle = currentDegree;

            // setting rotation
            var targetDirection = scrollableItem.InstantiatedScrollableItem.transform.position - Camera.main.transform.position;
            var newRotation = Vector3.RotateTowards(scrollableItem.InstantiatedScrollableItem.transform.forward, targetDirection, 90f, 0.0f);
            scrollableItem.InstantiatedScrollableItem.transform.rotation = Quaternion.LookRotation(newRotation);

            currentDegree += degreeIncrement;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PositionCameraAndContent();
    }

    bool isDown(float cameraRotation)
    {
        return (cameraRotation < 360 && cameraRotation <= 180);
    }

    private void FixedUpdate()
    {
        // Bit shift the index of the layer (10) to get a bit mask
        // This will cast rays only against colliders in layer 10.
        int layerMask = 1 << 10;

        // Does the ray intersect any objects excluding the player layer
        var directionVector = Camera.main.transform.TransformDirection(Vector3.forward);

        if (IsHorizontal)
        {
            directionVector.y = 0;
        }

        Debug.DrawRay(Camera.main.transform.position, directionVector * 1000, Color.blue);
        if (Physics.Raycast(Camera.main.transform.position, directionVector, out RaycastHit hit, radius + 1f, layerMask))
        {

            if (default(string) == selectionTag || selectionTag == null || selectionTag == "")
            {
                Debug.LogError("Selection Tag has yet been set");
            }
            else
            {
                Debug.DrawRay(Camera.main.transform.position, directionVector * 1000, Color.green);

                var selectionPositionGameObject = FindChildWithTag(selectionTag, hit.transform);

                if (selectionPositionGameObject)
                {
                    // if (!SpawnedScrollablePointer)
                    // {
                    //     SpawnedScrollablePointer = Instantiate(SelectionPrefab, selectionPositionGameObject.transform, false);
                    // }

                    // SpawnedScrollablePointer.transform.SetParent(selectionPositionGameObject.transform, false);

                    // setting all scrollable item to not have focus
                    foreach (var item in gameObject.GetComponentsInChildren<Interactable>())
                    {
                        item.HasFocus = false;
                    }

                    // setting the scrollable item that was hit to have
                    PreviousHitScrollableItem = selectionPositionGameObject.GetComponent<Interactable>();
                    PreviousHitScrollableItem.HasFocus = false;
                }
            }
        }
        else
        {
            Debug.Log("Did not Hit");
        }
    }


    public GameObject FindChildWithTag(string tag, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.tag == tag)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public float Map(float OldMin, float OldMax, float NewMin, float NewMax, float valueToMap)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float mappedValue = (((valueToMap - OldMin) * NewRange) / OldRange) + NewMin;

        return (mappedValue);
    }

    public void SwitchOrientation()
    {
        isHorizontal = !isHorizontal;

        Spacing = GetSpacing(scrollableItems[0]);
        ScrollableItemLength = GetLength(scrollableItems[0]);
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
        return;
    }

    // is triggered if all other gameobjects in scene did not respond to a Action - works as a fallback
    // only way we can register actions on gameobjects like these, where focus shouldn't be 
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (PreviousHitScrollableItem)
        {
            // a display has been hit (selected)
            PreviousHitScrollableItem.SetInputDown();

        }
        else
        {
            Debug.Log("hit display not been set");
        }
    }




    private IEnumerator<UnityEngine.WaitForSeconds> WaitFor(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ItemSelected.Invoke(previousHitScrollableItem.transform.GetComponent<OrderItem>().ScrollableID);
    }
}
