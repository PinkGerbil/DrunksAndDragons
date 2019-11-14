using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class joinMenu : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;


    public GameObject p1AtoJoin;
    public GameObject p2AtoJoin;
    public GameObject p3AtoJoin;
    public GameObject p4AtoJoin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player1.activeInHierarchy && p1AtoJoin.activeInHierarchy)
        {
            p1AtoJoin.SetActive(false);
        }
        if (player2.activeInHierarchy && p2AtoJoin.activeInHierarchy)
        {
            p2AtoJoin.SetActive(false);
        }
        if (player3.activeInHierarchy && p3AtoJoin.activeInHierarchy)
        {
            p3AtoJoin.SetActive(false);
        }
        if (player4.activeInHierarchy && p4AtoJoin.activeInHierarchy)
        {
            p4AtoJoin.SetActive(false);
        }
    }
}
