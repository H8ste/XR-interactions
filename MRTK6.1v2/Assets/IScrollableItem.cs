using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IScrollableItem
{
    
    GameObject ItemPrefab { get; set; }
    OnClick OnClick { get; set; }
    int ScrollableID { get; set; }
    GameObject InstantiatedScrollableItem { get; set; }
    float SpawnedDegreeAngle {get; set;}

}