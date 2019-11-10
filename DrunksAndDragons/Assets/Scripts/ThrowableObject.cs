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
    new Rigidbody rigidbody;
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
            int mask = 1 << LayerMask.NameToLayer("Floor");
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
            {
                float distance = Vector3.Distance(hit.point, thisCollider.ClosestPointOnBounds(hit.point));

                if(distance <= 0.1)
                {
                    rigidbody.isKinematic = true;
                    wasThrown = false;
                    hitEnemies.Clear();
                    GetComponent<NavMeshObstacle>().enabled = true;
                }
            }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (wasThrown && collision.collider.CompareTag("Enemy"))
        {

            foreach (GameObject child in hitEnemies)
                if (collision.collider.gameObject.Equals(child))
                    return;

            Vector3 colNorm = collision.contacts[0].normal;

            Vector3 hitDir = Vector3.ProjectOnPlane(rigidbody.velocity, colNorm);
            hitDir.y = 0;
            collision.collider.GetComponent<AI>().takeDamage(damage, hitDir.normalized);
            Debug.Log(rigidbody.velocity);

            hitEnemies.Add(collision.collider.gameObject);
            objectHealth--;
            if (objectHealth <= 0)
            {
                ObjectBreak();
            }
        }
    }
}
