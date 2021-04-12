using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickinHandlerTester : MonoBehaviour
{
    OrderItem current;
    OrderItem next;
    PickingHandler pickingHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        
        pickingHandler = gameObject.AddComponent<PickingHandler>();
        pickingHandler.BeginNewPick();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
