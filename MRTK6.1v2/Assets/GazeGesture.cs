using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GazeGesture : MonoBehaviour, IMixedRealityInputActionHandler
{
    [SerializeField]
    private List<string> itemsToSelectFrom;

    [SerializeField]
    private GameObject displayPrefab;

    [SerializeField]
    private GameObject selectionPrefab;

    private GameObject spawnedSelection;

    [SerializeField]
    private float spacingPercentage = 100f;
    private float spacing;


    [SerializeField]
    private float radius = 2f;

    private float capStartDegree = 45f;
    private float capEndDegree = -45f;

    private float startDegree;
    private float endDegree;



    //[SerializeField]
    private bool shouldRotate = true;

    private float displayHeight;
    private float displayWidth;

    [SerializeField]
    private bool isHorizontal = true;

    public bool IsHorizontal { get { return isHorizontal; } }


    private List<GameObject> spawnedDisplays = new List<GameObject>();

    private bool hasHitDisplay = false;
    private Interactable hitDisplay;

    [SerializeField]
    private string selectionTag;

    // Start is called before the first frame update
    void Start()
    {



        var cameraRotFixed = Quaternion.Euler(new Vector3(0, (Camera.main.transform.rotation).eulerAngles.y, 0));
        transform.SetPositionAndRotation(Camera.main.transform.position, cameraRotFixed);

        if (!displayPrefab || itemsToSelectFrom == null)
        {
            Debug.LogError("all necessary properties not set");
        }

        itemsToSelectFrom.ForEach((item) =>
        {
            var display = Instantiate(displayPrefab, Camera.main.transform.position, cameraRotFixed, gameObject.transform);
            display.GetComponent<TextMeshPro>().text = item;
            spawnedDisplays.Add(display);

            if (default(float) == displayHeight || default(float) == displayWidth)
            {
                displayHeight = display.GetComponent<Collider>().bounds.size.y;
                displayWidth = display.GetComponent<Collider>().bounds.size.x;
            }
        });

        if (isHorizontal)
        {
            spacing = displayWidth * spacingPercentage;
        }
        else
        {
            spacing = displayHeight * spacingPercentage;
        }

        Debug.Log("H: " + displayHeight);
        Debug.Log("W: " + displayWidth);


        PositionDisplays(true);


    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //PositionDisplays();

        //find x rotation

        // get



        var beginSegment = startDegree - capStartDegree;
        var endSegment = Mathf.Abs(endDegree - capEndDegree);

        //Debug.Log("isDown: " + isDown(Camera.main.transform.rotation.eulerAngles.x));

        //Debug.Log(Camera.main.transform.rotation.eulerAngles.x);
        //Debug.Log("startDegree: " + startDegree);
        //Debug.Log("endDegree: " + endDegree);
        //Debug.Log("beginSegment: " + beginSegment);
        //Debug.Log("endSegment: " + beginSegment);
        //Debug.Log("up %:" + Mathf.Clamp(-1 * (Camera.main.transform.rotation.eulerAngles.x - 360) / capStartDegree, 0, 1));
        //Debug.Log("down %:" + Mathf.Clamp(Camera.main.transform.rotation.eulerAngles.x / Mathf.Abs(capEndDegree), 0, 1));


        Quaternion cameraRotFixed;

        if (isHorizontal)
        {

            var percentile = isDown(Camera.main.transform.rotation.eulerAngles.y) ?
                Mathf.Clamp(Camera.main.transform.rotation.eulerAngles.y / Mathf.Abs(capEndDegree), 0, 1)
                : Mathf.Clamp(-1 * (Camera.main.transform.rotation.eulerAngles.y - 360) / Mathf.Abs(capStartDegree), 0, 1);

            //Debug.Log(Camera.main.transform.rotation.eulerAngles.x < 0 ? "positive" + percentile : "negative" + percentile);

            var rotation = isDown(Camera.main.transform.rotation.eulerAngles.y) ?
                    -endSegment * percentile :
                    -beginSegment * percentile;


            cameraRotFixed = Quaternion.Euler(new Vector3(0, rotation, 0));

        }
        else
        {
            var percentile = isDown(Camera.main.transform.rotation.eulerAngles.x) ?
                Mathf.Clamp(Camera.main.transform.rotation.eulerAngles.x / Mathf.Abs(capEndDegree), 0, 1)
                : Mathf.Clamp(-1 * (Camera.main.transform.rotation.eulerAngles.x - 360) / Mathf.Abs(capStartDegree), 0, 1);

            //Debug.Log(Camera.main.transform.rotation.eulerAngles.x < 0 ? "positive" + percentile : "negative" + percentile);

            var rotation = isDown(Camera.main.transform.rotation.eulerAngles.x) ?
                    -endSegment * percentile :
                    -beginSegment * percentile;

            cameraRotFixed = Quaternion.Euler(new Vector3(rotation, (Camera.main.transform.rotation).eulerAngles.y, 0));

        }




        transform.SetPositionAndRotation(Camera.main.transform.position, cameraRotFixed);

        PositionDisplays(true);
    }

    bool isDown(float cameraRotation)
    {
        return (cameraRotation < 360 && cameraRotation <= 180);
    }

    void PositionDisplays(bool setPosition = false)
    {
        if (isHorizontal)
        {
            spacing = displayWidth * spacingPercentage / 100;
        }
        else
        {
            spacing = displayHeight * spacingPercentage / 100;
        }


        var perimeter = 2 * Mathf.PI * radius;

        //Debug.Log("Perimeter = " + perimeter + ". Spacing = " + spacing);
        var radians = Map(0, 1, 0, 2, spacing / perimeter);
        var degrees = Mathf.Rad2Deg * radians;


        var totalSpanDegree = degrees * spawnedDisplays.Count;
        startDegree = 0 - totalSpanDegree / 2;
        endDegree = totalSpanDegree / 2;


        var currentDegree = startDegree;

        spawnedDisplays.ForEach(display =>
        {
            Vector3 newPos;

            if (isHorizontal)
            {
                newPos = /*gameObject.transform.localPosition +*/ new Vector3(Mathf.Sin(currentDegree) * radius, 0, Mathf.Cos(currentDegree) * radius);
                //newPos.y = 0;
            }
            else
            {
                newPos = /*gameObject.transform.localPosition +*/ new Vector3(0, Mathf.Sin(currentDegree) * radius, Mathf.Cos(currentDegree) * radius);
                //newPos.x = 0;
            }


            if (setPosition)
                display.transform.localPosition = newPos;

            if (shouldRotate)
                display.transform.LookAt(display.transform.position + Camera.main.transform.rotation * Vector3.forward, Vector3.up);

            currentDegree += degrees;

        });



    }

    private void FixedUpdate()
    {
        // Bit shift the index of the layer (10) to get a bit mask
        // This will cast rays only against colliders in layer 10.
        int layerMask = 1 << 10;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        var directionVector = Camera.main.transform.TransformDirection(Vector3.forward);

        switch (IsHorizontal)
        {
            case true:
                directionVector.y = 0;
                break;

            case false:
                //directionVector.x = 0;
                break;
        }

        Debug.DrawRay(Camera.main.transform.position, directionVector * 1000, Color.blue);
        if (Physics.Raycast(Camera.main.transform.position, directionVector, out hit, radius + 1f, layerMask))
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
        } else
        {
            Debug.Log("hit display not been set");
        }
    }
}
