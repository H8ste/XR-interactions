using UnityEngine;

public class BackgroundVideoFixer : MonoBehaviour
{

    public enum FixerState
    {
        SetPosition,
        Hide,
    }

    GameObject backgroundVideo;
    MeshFilter backgroundPlane;


    [SerializeField]
    FixerState stateOfFixer;


    // Start is called before the first frame update
    void Start()
    {
        FindAndSetBackgroundVideo();
        FindAndSetBackgroundMeshFilter();
    }

    // Update is called once per frame
    void Update()
    {
        FindAndSetBackgroundVideo();
        FindAndSetBackgroundMeshFilter();

        if (backgroundVideo != null)
        {
            switch (stateOfFixer)
            {
                case FixerState.SetPosition:
                    backgroundVideo.transform.localPosition = new Vector3(0, 0, 1);
                    backgroundVideo.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
                case FixerState.Hide:
                    if (backgroundPlane != null)
                    {
                        backgroundPlane.mesh = null;
                    }
                    break;
            }
        }
    }

    private void FindAndSetBackgroundMeshFilter()
    {
        if (backgroundPlane == null && backgroundVideo != null)
        {
            backgroundPlane = backgroundVideo.GetComponent<MeshFilter>();
        }
    }

    private void FindAndSetBackgroundVideo()
    {
        if (backgroundVideo == null)
        {
            backgroundVideo = transform.GetChild(0).gameObject;
        }
    }
}
