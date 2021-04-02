using UnityEngine;

public class PositionSelectionPosition : MonoBehaviour
{
    // reference to scrollHandler that contains selected scrollableItem
    private ScrollHandler scrollHandlerReference;

    // a reference to the position of the pointer in its horizontal state - set in unityinspector
    [SerializeField]
    private Transform horizontalPos;

    // a reference to the position of the pointer in its vetical state - set in unityinspector
    [SerializeField]
    private Transform verticalPos;


    /* Unity Methods */

    private void Start()
    {
        // find scrollHandler in parent
        scrollHandlerReference = transform.GetComponentInParent<ScrollHandler>();
    }
    
    private void Update()
    {
        var child = Helper.FindChildWithTag(scrollHandlerReference?.PreviousHitScrollableItem?.gameObject, "SelectionPosition");
        if (child)
        {
            switch (scrollHandlerReference.IsHorizontal)
            {
                // based on scrollHandlerState, position pointer horizontally/vertically
                case true:
                    child.transform.localPosition = horizontalPos.localPosition;
                    child.transform.localRotation = horizontalPos.localRotation;
                    break;

                case false:
                    child.transform.localPosition = verticalPos.localPosition;
                    child.transform.localRotation = verticalPos.localRotation;
                    break;
            }
        }
    }
}
