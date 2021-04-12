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
        current = new OrderItem(null, new LocPK(40, 10, 10, 10, 10, "X"), 0, 33, 1, "Pumpe", 254, false);
        next = new OrderItem(null, new LocPK(50, 20, 12, 10, 10, "L"), 1, 110, 9, "Cencor", 251, true);

        pickingHandler = gameObject.AddComponent<PickingHandler>();
        pickingHandler.BeginNewPick();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
