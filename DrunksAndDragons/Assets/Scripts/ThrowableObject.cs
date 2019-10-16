using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(NavMeshObstacle))]
public class ThrowableObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How much damage the thrown item does when it hits an enemy")]
    int damage = 1;

    [HideInInspector]
    public bool wasThrown = false;

    Rigidbody rigidbody;
    Collider thisCollider;

    float maxTimer = 1;
    float timer = 0;

    [SerializeField]
    [Tooltip("How many times the thrown object can hit an enemy before breaking")]
    [Range(1, 10)]
    int objectHealth = 3;

    List<GameObject> hitEnemies = new List<GameObject>();



    void Start()
    {
        
        rigidbody = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if(wasThrown && rigidbody.velocity == Vector3.zero)
        {
            rigidbody.isKinematic = true;
            wasThrown = false;
            hitEnemies.Clear();
        }
    }
    public void setThrown()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        timer = maxTimer;
        wasThrown = true;
    }

    /// <summary>
    /// once the object runs out of health (or some other condition is met) call this function to break the object.
    /// </summary>
    void ObjectBreak()
    {
        // instantiate broken stool gameobject and destroy this.
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(wasThrown && other.CompareTag("Enemy"))
        {
            foreach (GameObject child in hitEnemies)
                if (other.gameObject.Equals(child))
                    return;
            Vector3 hitDir = (other.ClosestPointOnBounds(transform.position) - transform.position).normalized;
            hitDir.y = 0;
            other.GetComponent<AI>().takeDamage(damage, hitDir.normalized);
            Debug.Log(rigidbody.velocity);
            rigidbody.velocity = (rigidbody.velocity.normalized - hitDir * 2).normalized * rigidbody.velocity.magnitude * 2;

            hitEnemies.Add(other.gameObject);
            objectHealth--;
            if (objectHealth <= 0)
            {
                ObjectBreak();
            }
        }
    }
}
