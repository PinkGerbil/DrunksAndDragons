using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFlip : MonoBehaviour
{
    bool isFlipping { get { return GetComponent<Rigidbody>().velocity != Vector3.zero; } }

    [SerializeField]
    [Tooltip("How much damage is done when the table collides with an enemy")]
    [Range(0, 10)]
    int damage = 1;

    [SerializeField]
    [Tooltip("How long after the table is flipped before it goes back to normal")]
    [Range(1, 100)]
    float ResetTime = 20;
    float resetCountdown = 0;
    

    Vector3 nextPosition = Vector3.zero;
    Vector3 unflippedPosition = Vector3.zero;
    Quaternion newRotation = Quaternion.identity;
    Quaternion unflippedRotation = Quaternion.identity;

    Vector3    resetPos = Vector3.zero;
    Quaternion resetRot = Quaternion.identity;

    Vector3 flipDir = Vector3.zero;

    Collider collider;

    List<GameObject> hitEnemies = new List<GameObject>();

    void Start()
    {
        collider = GetComponent<Collider>();

        resetPos = transform.position;
        resetRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (resetCountdown > 0)
        {
            resetCountdown -= Time.deltaTime;
            if(resetCountdown <= 0)
            {
                transform.SetPositionAndRotation(resetPos, resetRot);
            }
        }

        
    }

    public void flip(Vector3 flipperPos)
    {
        //if (!isFlipping && resetCountdown <= 0)
        //{
            Debug.Log("Table Flipped");
            resetCountdown = ResetTime;
            GetComponent<Rigidbody>().AddForceAtPosition(((transform.position - flipperPos).normalized + Vector3.up).normalized * 500, GetComponent<Collider>().ClosestPointOnBounds(flipperPos));
            

        //}
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
            Debug.Log("hit");
            other.GetComponent<AI>().takeDamage(damage, flipDir);
            hitEnemies.Add(other.gameObject);
        }
    }
    

}
