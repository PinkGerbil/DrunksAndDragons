using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFlip : MonoBehaviour
{
    bool flipped { get { return flipCountdown > 0; } }

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
    [Tooltip("How long it takes for the table to reach flipped state")]
    [Range(0, 10)]
    float flipTime = 0.1f;
    float flipCountdown = 0;

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
        if (flipTime <= 0)
            flipTime = 0.0000001f;

        resetPos = transform.position;
        resetRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(flipCountdown > 0)
        {
            flipCountdown -= Time.deltaTime;
            float perc = (flipTime - flipCountdown) * (1 / flipTime);
            transform.position = Vector3.Lerp(unflippedPosition, nextPosition, perc);
            transform.rotation = Quaternion.Lerp(unflippedRotation, newRotation, perc);
            if (flipCountdown <= 0)
                hitEnemies.Clear();
        }
        else if(resetCountdown > 0)
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
        if (!flipped)
        {
            Debug.Log("Table Flipped");
            resetCountdown = ResetTime;
            flipCountdown = flipTime;

            flipDir = (GetComponent<Collider>().ClosestPointOnBounds(flipperPos) - flipperPos);
            flipDir.y = 0;
            flipDir.Normalize();
            Debug.Log(flipDir);

            findNextPos();

        }
    }

    void findNextPos()
    {
        unflippedPosition = transform.position;
        unflippedRotation = transform.rotation;
        int distance = (int)(2.0f * (flipDir.z + flipDir.x) + 2.25f * flipDir.z);
        if (distance < 0) distance *= -1;
        int height = (int)(2.0f * (flipDir.z + flipDir.x) + 2.0f * flipDir.z);
        if (height < 0) height *= -1;
        
        nextPosition = transform.position + flipDir * distance + Vector3.up * height;
        if (transform.position != resetPos || transform.rotation != resetRot)
            nextPosition.y = resetPos.y + 2.5f;
        newRotation = transform.rotation;

        Debug.Log(distance + " : " + height);

        float x = 90 * flipDir.x;
        float z = 90 * flipDir.z;

        newRotation *= Quaternion.Euler(z, 0, -x);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (flipped && other.CompareTag("Enemy"))
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
