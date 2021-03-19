using UnityEngine;

public class SwipeGestureCollider : MonoBehaviour
{
    private BoxCollider collider;
    private SwipeGesture sG;
    private void OnTriggerExit(Collider fingerCollider)
    {
        if (sG != null && fingerCollider.tag == "IndexFingerCollider")
        {
            sG.Collision(collider.center);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<BoxCollider>();
        sG = gameObject.GetComponentInParent<SwipeGesture>();
    }

}
