using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ThrowableObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How much damage the thrown item does when it hits an enemy")]
    int damage = 1;

    [HideInInspector]
    public bool wasThrown = false;

    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if(wasThrown && rigidbody.velocity == Vector3.zero)
        {
            rigidbody.isKinematic = true;
            wasThrown = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(wasThrown && collision.collider.CompareTag("Enemy"))
        {
            Vector3 hitDir = collision.collider.transform.position - transform.position;
            hitDir.y = 0;
            collision.collider.GetComponent<AI>().takeDamage(damage, hitDir.normalized);
        }
    }
}
