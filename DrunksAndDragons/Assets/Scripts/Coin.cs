using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int xAxis;
    public int yAxis;
    public int zAxis;

    // Start is called before the first frame update
    void Start()
    {
        int coinShootingX = Random.Range(-xAxis, xAxis);
        int coinShootingY = Random.Range(0, yAxis);
        int coinShootingZ = Random.Range(-zAxis, zAxis);

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
