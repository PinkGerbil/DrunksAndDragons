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
    float camMoveSpeed = 1;

    [SerializeField]
    [Range(-1, 1000)]
    float maxZoom;
    [SerializeField]
    [Range(-1000, 1)]
    float minZoom;

    public float PlayerScale = 1;

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
        

        transform.localPosition = Vector3.Lerp(transform.localPosition, nextPos + -transform.forward * nextZoom, camMoveSpeed * Time.deltaTime);

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
                    float distance = Vector3.Distance(child1.transform.position, child2.transform.position) / PlayerScale;
                    if (distance > temp)
                        temp = distance;
                }
        if(!(maxZoom < 0 && minZoom > 0))
            temp = Mathf.Clamp(temp, minZoom, maxZoom);
        return temp;
    }
}
