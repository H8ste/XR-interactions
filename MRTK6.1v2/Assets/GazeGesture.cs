using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GazeGesture : MonoBehaviour
{
    [SerializeField]
    private List<string> itemsToSelectFrom;

    [SerializeField]
    private GameObject displayPrefab;

    [SerializeField]
    private float spacing = 0.2f;

    [SerializeField]
    private float radius = 2f;

    private float capStartDegree = 45f;
    private float capEndDegree = -45f;

    private float startDegree;
    private float endDegree;



    //[SerializeField]
    private bool shouldRotate = true;





    private List<GameObject> spawnedDisplays = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        var cameraRotFixed = Quaternion.Euler(new Vector3(0, (Camera.main.transform.rotation).eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z));
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
        });

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

        var percentile = isDown(Camera.main.transform.rotation.eulerAngles.x) ?
            Mathf.Clamp(Camera.main.transform.rotation.eulerAngles.x / Mathf.Abs(capEndDegree), 0, 1)
            : Mathf.Clamp(-1 * (Camera.main.transform.rotation.eulerAngles.x - 360) / Mathf.Abs(capStartDegree), 0, 1);

        //Debug.Log(Camera.main.transform.rotation.eulerAngles.x < 0 ? "positive" + percentile : "negative" + percentile);

        var rotation = isDown(Camera.main.transform.rotation.eulerAngles.x) ?
                -endSegment * percentile :
                -beginSegment * percentile;

        var cameraRotFixed = Quaternion.Euler(new Vector3(rotation, (Camera.main.transform.rotation).eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z));




        transform.SetPositionAndRotation(Camera.main.transform.position, cameraRotFixed);

        PositionDisplays();
    }

    bool isDown(float cameraRotation)
    {
        return (cameraRotation < 360 && cameraRotation <= 180);
    }

    void PositionDisplays(bool setPosition = false)
    {
        var perimeter = 2 * Mathf.PI * radius;


        var radians = Map(0, 1, 0, 2, spacing / perimeter);
        var degrees = Mathf.Rad2Deg * radians;


        var totalSpanDegree = degrees * spawnedDisplays.Count;
        startDegree = 0 - totalSpanDegree / 2;
        endDegree = totalSpanDegree / 2;


        var currentDegree = startDegree;

        spawnedDisplays.ForEach(display =>
        {
            var newPos = Camera.main.transform.position + new Vector3(0, Mathf.Sin(currentDegree) * radius, Mathf.Cos(currentDegree) * radius);

            if (setPosition)
                display.transform.position = newPos;

            if (shouldRotate)
                display.transform.LookAt(display.transform.position + Camera.main.transform.rotation * Vector3.forward, Vector3.up);

            currentDegree += degrees;

        });
    }

    public float Map(float OldMin, float OldMax, float NewMin, float NewMax, float valueToMap)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float mappedValue = (((valueToMap - OldMin) * NewRange) / OldRange) + NewMin;

        return (mappedValue);
    }

}
