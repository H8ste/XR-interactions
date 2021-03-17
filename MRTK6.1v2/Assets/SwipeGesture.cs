using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SwipeGesture : MonoBehaviour
{
    List<int> previousCollisions = new List<int>();
    public delegate void SwipeEvent(int direction);
    public static event SwipeEvent OnSwiped;
    private void Start()
    {
        EndGestureRecognition();
    }
    private int GetColliderIndex(Vector3 position)
    {
        switch (position.x)
        {
            case var _position when position.x < 0:
                return 0;
                
            case 0:
                return 1;
                
            case var _position when position.x > 0:
                return 2;
                
            
        }
        return -1;
    }

  
    public void Collision(Vector3 colliderCenter)
    {
        var index = GetColliderIndex(colliderCenter);
        if (index != -1)
        {
            RecordCollision(index);
            if (HasSwiped(out var direction))
            {
                OnSwiped.Invoke(direction);
                Debug.Log("direction:" + direction);
                //EndGestureRecognition();
            }
            
        }
    }
    private void RecordCollision(int collisionIndex)
    {
        previousCollisions.Add(collisionIndex);
        if(previousCollisions.Count > 3)
        {
            previousCollisions.RemoveAt(0);
        }
    }
    private bool HasSwiped(out int direction)
    {
        direction = -1;
        if(previousCollisions.Count == 3)
        {
            List<int> leftSwipe = new List<int> { 0, 1, 2 };
            List<int> rightSwipe = new List<int> { 2, 1, 0 };
         
            if(Enumerable.SequenceEqual(previousCollisions, leftSwipe))
            {
                direction = 0;
                return true;
            }

            if (Enumerable.SequenceEqual(previousCollisions, rightSwipe))
            {
                direction = 1;
                return true;
            }


        }
        return false;
    }
    public void BeginGestureRecognition()
    {
        IterateOverColliders((collider) => collider.SetActive(true));
        
    }
    public void EndGestureRecognition()
    {
        IterateOverColliders((collider) => collider.SetActive(false));
    }
    private void IterateOverColliders(Action<GameObject> action)
    {
        
        var colliders = gameObject.GetComponentsInChildren<BoxCollider>(includeInactive: true).Select(x => x.transform).ToArray();
        foreach (var item in colliders)
        {
            action(item.gameObject);
        }
    }
}
