using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollHandler : MonoBehaviour, IMixedRealityInputActionHandler
{
    [SerializeField]
    private List<string> itemsToSelectFrom;

    [SerializeField]
    private GameObject displayPrefab;

    [SerializeField]
    private GameObject selectionPrefab;

    private GameObject spawnedSelection;


    // private float spacing;


    [SerializeField]
    private float radius = 2f;

    private float capStartDegree = 45f;
    private float capEndDegree = -45f;

    // private float startDegree;
    // private float endDegree;



    //[SerializeField]
    private bool shouldRotate = true;




    public bool IsHorizontal { get { return isHorizontal; } }


    private List<GameObject> spawnedDisplays = new List<GameObject>();

    private bool hasHitDisplay = false;
    private Interactable hitDisplay;

    [SerializeField]
    private string selectionTag;




    /* UML */
    IScrollableItem[] scrollableItems;
    public IScrollableItem[] ScrollableItems { get { return scrollableItems; } }



    float spacing;
    public float Spacing { get { return spacing; } set { spacing = value; } }

    [SerializeField]
    private bool isHorizontal = true;

    float degreeSpan;

    GameObject selectionPointer;


    private float scrollableItemLength;
    public float ScrollableItemLength { get { return scrollableItemLength; } set { scrollableItemLength = value; } }

    // public float DisplayHeight { get { return displayHeight; } }

    private float displayWidth;
    public float DisplayWidth { get { return displayWidth; } }

    private List<GameObject> spawnedScrollableItems = new List<GameObject>();
    public List<GameObject> SpawnedScrollableItems { get { return spawnedScrollableItems; } }

    [SerializeField]
    private float spacingPercentage = 100f;

    // private cameraRotationFixed =

    public ScrollHandler(IScrollableItem[] scrollableItems)
    {
        this.scrollableItems = scrollableItems;

        Populate();
    }

    private void PositionCameraAndContent()
    {
        var calculations = CalculateStartEndAndIncrementDegrees();

        PositionCamera(calculations.startDegree, calculations.endDegree);
        PositionContent(calculations.startDegree, calculations.endDegree, calculations.degreeIncrement);
    }

    private (float startDegree, float endDegree, float degreeIncrement) CalculateStartEndAndIncrementDegrees()
    {
        var perimeter = 2 * Mathf.PI * radius;
        var spacingInRadian = Map(0, 1, 0, 2, Spacing / perimeter);
        var spacingInDegrees = Mathf.Rad2Deg * spacingInRadian;
        var totalSpanDegree = ScrollableItems.Length * (spacingInDegrees + ScrollableItemLength);

        var itemLengthInRadian = Map(0, 1, 0, 2, ScrollableItemLength / perimeter);
        var itemLengthInDegree = Mathf.Rad2Deg * itemLengthInRadian;

        var startDegree = 0 - totalSpanDegree / 2;
        var endDegree = totalSpanDegree / 2;
        return (startDegree, endDegree, spacingInDegrees + itemLengthInDegree);
    }

    private void PositionCamera(float startDegree, float endDegree)
    {
        var beginSegment = startDegree - capStartDegree;
        var endSegment = Mathf.Abs(endDegree - capEndDegree);

        Quaternion cameraRotFixed = GetCameraRotation(beginSegment, endSegment);

        transform.SetPositionAndRotation(Camera.main.transform.position, cameraRotFixed);
    }



    private Quaternion GetCameraRotation(float beginSegment, float endSegment)
    {
        var angleValue = isHorizontal ? Camera.main.transform.rotation.eulerAngles.y : Camera.main.transform.rotation.eulerAngles.x;

        var percentile = GetPercentile(angleValue);

        var rotation = GetRotation(angleValue, percentile, beginSegment, endSegment);

        return isHorizontal ?
            Quaternion.Euler(new Vector3(0, rotation, 0)) :
            Quaternion.Euler(new Vector3(rotation, Camera.main.transform.rotation.eulerAngles.y, 0));
    }

    private float GetPercentile(float rotation)
    {
        // with respect to rotation of camera and min+max rotation, calculate rotation percentage from min to max rotation
        return isDown(rotation) ?
                Mathf.Clamp(rotation / Mathf.Abs(capEndDegree), 0, 1)
                : Mathf.Clamp(-1 * (rotation - 360) / Mathf.Abs(capStartDegree), 0, 1);
    }

    private float GetRotation(float angleValue, float percentile, float beginSegment, float endSegment)
    {
        // with respect to given angle value, percentile, beginSegment and endSegment, compute local-space camera rotation
        return percentile * (isDown(angleValue) ? -endSegment : -beginSegment);
    }

    private void Populate()
    {
        // spawn each scrollableItem (with proper parenting within Unity)
        foreach (var scrollableItem in ScrollableItems)
        {
            scrollableItem.InstantiatedScrollableItem = Instantiate(scrollableItem.ItemPrefab);
            if (spacing == default(float)) spacing = GetSpacing(scrollableItem);
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
        return isHorizontal ?
            scrollableItem.InstantiatedScrollableItem.GetComponent<Collider>().bounds.size.x :
            scrollableItem.InstantiatedScrollableItem.GetComponent<Collider>().bounds.size.y;
    }

    private void PositionContent(float startDegree, float endDegree, float degreeIncrement)
    {
        var currentDegree = startDegree;

        foreach (var scrollableItem in ScrollableItems)
        {
            Vector3 newPos = isHorizontal ?
                new Vector3(Mathf.Sin(currentDegree) * radius, 0, Mathf.Cos(currentDegree) * radius) :
                new Vector3(0, Mathf.Sin(currentDegree) * radius, Mathf.Cos(currentDegree) * radius);

            // setting position
            scrollableItem.InstantiatedScrollableItem.transform.localPosition = newPos;

            // setting rotation
            scrollableItem.InstantiatedScrollableItem.transform.LookAt(
                scrollableItem.InstantiatedScrollableItem.transform.position + Camera.main.transform.rotation * Vector3.forward, Vector3.up
            );

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
                    if (spawnedSelection?.transform?.parent == selectionPositionGameObject.transform)
                    {

                    }
                    else
                    {

                        if (spawnedSelection)
                        {
                            Destroy(spawnedSelection);
                        }

                        spawnedSelection = Instantiate(selectionPrefab, selectionPositionGameObject.transform, false);
                        hitDisplay = selectionPositionGameObject.GetComponent<Interactable>();

                        Debug.Log(hit.transform.GetComponent<TextMeshPro>().text);

                        hasHitDisplay = true;
                        hitDisplay.HasFocus = true;


                        foreach (var display in gameObject.GetComponentsInChildren<Interactable>())
                        {
                            if (display != hitDisplay)
                            {
                                display.HasFocus = false;
                            }
                        }
                    }


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
            //Debug.Log(tag + " == " + child.tag);
            if (child.tag == tag)
            {
                //Debug.Log("found child");
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
        if (hitDisplay)
        {
            // a display has been hit (selected)
            hitDisplay.SetInputDown();
        }
        else
        {
            Debug.Log("hit display not been set");
        }
    }
}
