using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    List<GameObject> players = new List<GameObject>();

    Vector3 originPoint;

    Vector3 nextPos = Vector3.zero;
    float nextZoom;

    [SerializeField]
    float camMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        originPoint = transform.position;
        players = blackboard.players;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (players.Count == 0)
            players = blackboard.players;
        else
        {
            nextPos = CalcCamPos();
            nextZoom = CalcCamZoom();
        }
        transform.position = Vector3.Lerp(transform.position, (nextPos + -transform.forward * nextZoom) + originPoint, camMoveSpeed * Time.deltaTime);

        nextZoom = 0;
    }

    Vector3 CalcCamPos()
    {
        Vector3 temp = Vector3.zero;


        foreach (GameObject child in players)
            temp += child.transform.position;

        return temp / players.Count;
    }

    float CalcCamZoom()
    {
        float temp = nextZoom;

        foreach(GameObject child1 in players)
            foreach(GameObject child2 in players)
                if(!child1.Equals(child2))
                {
                    float distance = Vector3.Distance(child1.transform.position, child2.transform.position);
                    if (distance > temp)
                        temp = distance;
                }

        return temp;
    }
}
