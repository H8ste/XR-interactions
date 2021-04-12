using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageHandler : MonoBehaviour
{
    [SerializeField]
    private Sprite imageOne;

    [SerializeField]
    private Sprite imageTwo;

    private Image imageComponentReference;

    // Start is called before the first frame update
    void Start()
    {
        if (imageOne == null)
        {
            Debug.LogError("ImageOne property has not been set on ImageHandler");
            throw new System.Exception("ImageOne property has not been set on ImageHandler");
        }
        if (imageTwo == null)
        {
            Debug.LogError("ImageTwo property has not been set on ImageHandler");
            throw new System.Exception("ImageTwo property has not been set on ImageHandler");
        }

        imageComponentReference = GetComponentInChildren<Image>();

        if (imageComponentReference == null)
        {
            Debug.LogError("An image component could not be found in children of gameobject this script is attached to");
            throw new System.Exception("An image component could not be found in children of gameobject this script is attached to");

        }
    }


    public void ToggleImage()
    {
        imageComponentReference.sprite = imageComponentReference.sprite == imageOne ? imageTwo : imageOne;
    }

    public void SetImage(bool setImageOne)
    {
        if (imageComponentReference == null)
        {
            imageComponentReference = GetComponentInChildren<Image>();
        }
        imageComponentReference.sprite = setImageOne ? imageOne : imageTwo;
    }

}
