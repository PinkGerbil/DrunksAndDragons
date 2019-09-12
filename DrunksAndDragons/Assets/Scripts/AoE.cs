using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoE : MonoBehaviour
{
    public float aoeDuration;
    public float timeBetweenHits;
    private float aoeTickRate;
    // Start is called before the first frame update
    void Start()
    {
        aoeTickRate = timeBetweenHits;
    }

    // Update is called once per frame
    void Update()
    {
        aoeDuration -= Time.deltaTime;
        if(aoeDuration <= 0)
        {
            Destroy(this.gameObject);
        }
        aoeTickRate -= Time.deltaTime;
        if(aoeTickRate <= 0)
        {
            this.GetComponent<SphereCollider>().enabled = true;
            aoeTickRate = timeBetweenHits;
        }
        else
        {
            this.GetComponent<SphereCollider>().enabled = false;
        }
    }
}
