using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableFlip : MonoBehaviour
{
    bool flipped { get { return resetCountdown > 0 || flipCountdown > 0; } }

    [SerializeField]
    [Tooltip("How long after the table is flipped before it goes back to normal")]
    [Range(1, 100)]
    float ResetTime = 20;
    float resetCountdown = 0;

    float flipTime = 0.5f;
    float flipCountdown = 0;

    Vector3 nextPosition = Vector3.zero;
    Vector3 originalPosition = Vector3.zero;
    Quaternion newRotation = Quaternion.identity;
    Quaternion originalRotation = Quaternion.identity;

    void Start()
    {
        
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(flipCountdown > 0)
        {
            flipCountdown -= Time.deltaTime;
            float perc = (flipTime - flipCountdown) * (1 / flipTime);
            transform.position = Vector3.Lerp(originalPosition, nextPosition, perc);
            transform.rotation = Quaternion.Lerp(originalRotation, newRotation, perc);
            if (perc >= 0.5f)
                nextPosition.y = originalPosition.y + 3;
        }
        else if(resetCountdown > 0)
        {
            resetCountdown -= Time.deltaTime;
            if(resetCountdown <= 0)
            {
                transform.SetPositionAndRotation(originalPosition, originalRotation);
            }
        }
    }

    public void flip()
    {
        if (!flipped)
        {
            Debug.Log("Table Flipped");
            resetCountdown = ResetTime;
            flipCountdown = flipTime;
            nextPosition = transform.position + -transform.forward * 8.5f + Vector3.up * 7.0f;
            newRotation = transform.rotation;
            newRotation = Quaternion.Euler(180, 90, 0);
        }
    }
}
