
using UnityEngine;

public class PromptOption : IScrollableItem
{
    private string optionText;
    public string OptionText { get { return optionText; } set { optionText = value; } }


    private GameObject itemPrefab;
    private OnClick onclick;
    private int scrollableID;
    private GameObject instantiatedScrollableItem;
    private float spawnedDegreeAngle;

    public GameObject ItemPrefab { get { return itemPrefab; } set { itemPrefab = value; } }
    public OnClick OnClick { get { return onclick; } set { onclick = value; } }
    public int ScrollableID { get { return scrollableID; } set { scrollableID = value; } }
    public GameObject InstantiatedScrollableItem { get { return instantiatedScrollableItem; } set { instantiatedScrollableItem = value; } }

    public float SpawnedDegreeAngle { get { return spawnedDegreeAngle; } set { spawnedDegreeAngle = value; } }

    public PromptOption(string optionText, GameObject itemPrefab, OnClick onclick, int scrollableID)
    {
        this.optionText = optionText;
        this.itemPrefab = itemPrefab;
        this.onclick = onclick;
        this.scrollableID = scrollableID;
    }
    public void SwitchPrefab(GameObject newPrefab)
    {
        itemPrefab = newPrefab;
    }
}

