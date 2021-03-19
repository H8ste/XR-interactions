using UnityEngine;

public class PositionSelectionPosition : MonoBehaviour
{

    private GameObject selectionPositionReference;
    private GazeGesture gazeScriptReference;


    [SerializeField]
    private Transform horizontalPos;
    [SerializeField]
    private Transform verticalPos;

    private bool oldIsHorizontal;


    // Start is called before the first frame update
    void Start()
    {
        selectionPositionReference = FindChildWithTag("SelectionPosition", transform);
        gazeScriptReference = transform.GetComponentInParent<GazeGesture>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (gazeScriptReference.IsHorizontal != oldIsHorizontal)
        //{
            switch (gazeScriptReference.IsHorizontal)
            {
                case true:
                    selectionPositionReference.transform.localPosition = horizontalPos.localPosition;
                    selectionPositionReference.transform.localRotation = horizontalPos.localRotation;
                    break;

                case false:
                    selectionPositionReference.transform.localPosition = verticalPos.localPosition;
                    selectionPositionReference.transform.localRotation = verticalPos.localRotation;
                    break;
            }
        //}


        //oldIsHorizontal = gazeScriptReference.IsHorizontal;


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

}
