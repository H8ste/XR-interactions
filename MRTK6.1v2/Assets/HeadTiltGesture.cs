using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HeadTiltGesture : MonoBehaviour
{
    //private Camera cameraRef;
    private Quaternion FixedRotation { get { return Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, Camera.main.transform.rotation.eulerAngles.z)); } }
    private Vector3 FixedPosition { get { return Camera.main.transform.position; } }




    [SerializeField]
    private GameObject ColliderGO;
    [SerializeField]
    private GameObject RotateGO;

    [SerializeField]
    private GameObject ButtonGO;

    [SerializeField]
    private GameObject FeedForwardGO;

    [SerializeField]
    UnityEvent OnHeadTilt;

    [SerializeField]
    private AudioClip confirmClip;
    [SerializeField]
    private AudioClip cancelClip;


    private AudioSource audioSource;

    private Quaternion lockedRotation;

    private bool gestureListener;
    private bool doingConfirmGesture = false;
    private bool doingCancelGesture = false;
    private int lastCollisionID = -1;
    private bool hovering = false;







    // Start is called before the first frame update
    void Start()
    {
        //cameraRef = Camera.main;
        audioSource = GetComponent<AudioSource>();


        if (!ColliderGO && !RotateGO && !confirmClip && !cancelClip)
        {
            Debug.LogError("All references have not been set on this script");
        }
        else
        {
            ColliderGO.SetActive(false);
            FeedForwardGO.SetActive(false);
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (!gestureListener)
        {
            transform.SetPositionAndRotation(FixedPosition, FixedRotation);
        }
        else
        {
            transform.SetPositionAndRotation(FixedPosition, lockedRotation);
            RotateGO.transform.SetPositionAndRotation(FixedPosition, Camera.main.transform.rotation);
        }
    }

    private void FixedUpdate()
    {
        if (gestureListener)
        {
            // send out raycast from rotateGO
            SendRayCast();
        }
    }

    public void SendRayCast()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        // This would cast rays only against colliders in layer 9.
        int layerMask = 1 << 9;



        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(RotateGO.transform.position, RotateGO.transform.TransformDirection(Vector3.forward), out hit, 30f, layerMask))
        {
            // onEnter
            if (!hovering)
            {
                CollideInput(GetCollideID(hit.collider as BoxCollider));
            }

            hovering = true;
        }
        else
        {
            hovering = false;
        }
    }




    public void ActiveGestureListener()
    {
        Debug.Log("activated gesture listener");

        gestureListener = true;

        lockedRotation = gameObject.transform.rotation;
        ColliderGO.SetActive(true);
        FeedForwardGO.SetActive(true);
    }

    private int GetCollideID(BoxCollider boxCollider)
    {
        var centerPos = boxCollider.center;
        int returnID = -1;
        switch (Mathf.Abs(centerPos.x))
        {
            case 0f:
                // is positive
                switch (centerPos.y > 0)
                {
                    case true:
                        returnID = 0;
                        break;

                    case false:
                        returnID = 2;
                        break;
                }

                break;

            case 1.6f:
                // is positive
                switch (centerPos.x > 0)
                {
                    case true:
                        returnID = 1;
                        break;

                    case false:
                        returnID = 3;
                        break;
                }

                break;
        }

        return returnID;
    }


    private void CollideInput(int colliderID)
    {
        switch (colliderID)
        {
            case 0:
            case 2:
                RegisterConfirmInput(colliderID == lastCollisionID);
                break;
            case 1:
            case 3:
                RegisterCancelInput(colliderID == lastCollisionID);
                break;
        }

        lastCollisionID = colliderID;
    }

    public void RegisterConfirmInput(bool falseInput)
    {
        if (falseInput) return;

        //if (doingConfirmGesture)
        //{
        //    Debug.Log("confirmed");
        //    audioSource.clip = confirmClip;
        //    audioSource.Play();
        //    InvokeGesture();
        //} else
        //{
        //    doingCancelGesture = false;
        //    doingConfirmGesture = true;
        //}

        Debug.Log("confirmed");
        audioSource.clip = confirmClip;
        audioSource.Play();
        InvokeGesture();

        doingCancelGesture = false;
        doingConfirmGesture = true;

        Debug.Log("Registered Confirm Input");
    }

    public void RegisterCancelInput(bool falseInput)
    {
        if (falseInput) return;

        if (doingCancelGesture)
        {
            // did cancel gesture
            Debug.Log("cancelled");
            audioSource.clip = cancelClip;
            audioSource.Play();
            InvokeGesture();
        } else
        {
            doingCancelGesture = true;
            doingConfirmGesture = false;
        }

        Debug.Log("Registered Cancel Input");
    }

    public IEnumerator OnAudioStopped()
    {
        float clipLength = audioSource.clip.length;

        doingCancelGesture = false;
        doingConfirmGesture = false;
        gestureListener = false;
        hovering = false;
        ColliderGO.SetActive(false);
        FeedForwardGO.SetActive(false);


        yield return new WaitForSeconds(clipLength);



        //buttonActive = true
        ButtonGO.SetActive(true);
 

        //collidersActive = false


        lastCollisionID = -1;

        //print("")
    }


    public void EndGesture()
    {
        StartCoroutine(OnAudioStopped());
    }


    private void InvokeGesture()
    {
        if (OnHeadTilt != null)
        {
            OnHeadTilt.Invoke();
        }
    }
}
