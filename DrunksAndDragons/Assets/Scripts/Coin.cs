using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int xAxis;
    public int yAxis;
    public int zAxis;

    List<PlayerPoints> players;

    [SerializeField]
    [Tooltip("how fast the coin moves towards the player when the player is close enough")]
    float acceleration = 1;
    float velocity = 0;

    [SerializeField]
    [Tooltip("How close a player needs to be for the coin to be drawn to them")]
    float AttractDistance = 3;

    private float coinMagnetTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        //launches the coin in a random direction when spawned
        players = GameObject.Find("Game Manager").GetComponent<ScoreManager>().players;
        int coinShootingX = Random.Range(-xAxis, xAxis);
        int coinShootingY = Random.Range(0, yAxis);
        int coinShootingZ = Random.Range(-zAxis, zAxis);

        this.GetComponent<Rigidbody>().AddForce(new Vector3(coinShootingX, coinShootingY, coinShootingZ));
    }

    // Update is called once per frame
    void Update()
    {
        coinMagnetTimer -= Time.deltaTime;
        Vector3 closest = Vector3.positiveInfinity;

        //start to move towards the closest player
        if (coinMagnetTimer < 0)
        {
            foreach (PlayerPoints child in players)
            {
                if (Vector3.Distance(child.transform.position, transform.position) < Vector3.Distance(closest, transform.position))
                    closest = child.transform.position;
            }
            //closest.y = transform.position.y;
            if (Vector3.Distance(transform.position, closest) < AttractDistance)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                velocity += acceleration * Time.deltaTime;
                transform.position += (closest - transform.position).normalized * velocity * Time.deltaTime;
            }
            else
            {
                GetComponent<Rigidbody>().isKinematic = false;
                velocity = 0;
            }
        }
    }

    //getting picked up by an alive player
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PlayerDamageHandler>().Alive)
        {
            other.GetComponent<PlayerPoints>().AddPoints(1);
            Destroy(this.gameObject);
        }
    }

}
