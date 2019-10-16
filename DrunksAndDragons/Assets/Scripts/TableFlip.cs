using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class TableFlip : MonoBehaviour
{
    bool isFlipping { get { return GetComponent<Rigidbody>().velocity.magnitude > velocityMinMag && resetCountdown > 0; } }

    [SerializeField]
    [Tooltip("How much damage is done when the table collides with an enemy")]
    [Range(0, 10)]
    int damage = 1;

    [SerializeField]
    [Tooltip("How long after the table is flipped before it goes back to normal")]
    [Range(1, 100)]
    float ResetTime = 20;
    float resetCountdown = 0;

    [SerializeField]
    [Tooltip("how slow the table has to be moving before it is considered to be stopped")]
    float velocityMinMag = 0.1f;
    

    Vector3 nextPosition = Vector3.zero;
    Vector3 unflippedPosition = Vector3.zero;
    Quaternion newRotation = Quaternion.identity;
    Quaternion unflippedRotation = Quaternion.identity;

    Vector3    resetPos = Vector3.zero;
    Quaternion resetRot = Quaternion.identity;

    Vector3 flipDir = Vector3.zero;

    Collider collider;

    List<GameObject> hitEnemies = new List<GameObject>();

    Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        resetPos = transform.position;
        resetRot = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!isFlipping)
        {
            rigidbody.velocity = Vector3.zero;
            GetComponent<NavMeshObstacle>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (resetCountdown > 0)
        {
            resetCountdown -= Time.deltaTime;
            if (resetCountdown <= 0)
            {
                transform.SetPositionAndRotation(resetPos, resetRot);
                rigidbody.velocity = Vector3.zero;
                GetComponent<NavMeshObstacle>().enabled = true;
            }
        }
        Debug.DrawLine(transform.position, transform.position + rigidbody.velocity * Time.deltaTime);
    }

    public void flip(Vector3 flipperPos)
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        if(resetCountdown <= 0)
            resetCountdown = ResetTime;
        rigidbody.AddForceAtPosition(((transform.position - flipperPos).normalized + Vector3.up).normalized * 500, GetComponent<Collider>().ClosestPointOnBounds(flipperPos));
            
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (isFlipping && other.CompareTag("Enemy"))
        {
            foreach (GameObject child in hitEnemies)
                if (other.gameObject.Equals(child))
                {
                    Debug.Log("already hit");
                    return;
                }

            other.GetComponent<AI>().takeDamage(damage, flipDir);
            hitEnemies.Add(other.gameObject);
        }
    }
    

}
