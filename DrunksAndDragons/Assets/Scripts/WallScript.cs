using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{

    BoxCollider boxCollider;

    List<Collider> colliders;

    // Start is called before the first frame update
    void Start()
    {
        colliders = new List<Collider>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void LateUpdate()
    {
        foreach(Collider child in colliders)
        {
            child.transform.position = boxCollider.ClosestPoint(child.transform.position) + transform.forward * 0.5f;
        }
        colliders.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if(!CompareTag("Player") && !CompareTag("Enemy"))
        {
            colliders.Add(other);
        }
    }
}
