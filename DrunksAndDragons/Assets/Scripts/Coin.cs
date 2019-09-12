using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int randomRot = Random.Range(0, 50);
        int coinShootingX = Random.Range(-25, 25);
        int coinShootingY = Random.Range(0, 50);
        int coinShootingZ = Random.Range(-25, 25);

        this.GetComponent<Rigidbody>().AddForce(new Vector3(coinShootingX, coinShootingY, coinShootingZ));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerPoints>().AddPoints(1);
            Destroy(this.gameObject);
        }
    }

}
