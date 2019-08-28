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

    public float PlayerScale = 1;
    
    [Header("Position Bounds")]
    [Tooltip("Enable the fields below to be applied to the camera position.")]
    [SerializeField]
    bool useBounds = false;
    [SerializeField]
    [Range(0, 40)]
    float maxZoom = 20;
    [SerializeField]
    [Range(0, 40)]
    float minZoom = 0;
    [SerializeField]
    [Range(-10, 0)]
    float xMinBounds = -5;
    [SerializeField]
    [Range(0, 10)]
    float xMaxBounds = 5;
    [SerializeField]
    [Range(-20, 0)]
    float zMinBounds = -10;
    [SerializeField]
    [Range(0, 20)]
    float zMaxBounds = 10;

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
        if(useBounds)
        {
            Vector3 temp = transform.localPosition;
            if (transform.localPosition.x < xMinBounds)
                temp.x = xMinBounds;
            if (transform.localPosition.x > xMaxBounds)
                temp.x = xMaxBounds;

            if (transform.localPosition.z < zMinBounds)
                temp.z = zMinBounds;
            if (transform.localPosition.z > zMaxBounds)
                temp.z = zMaxBounds;

            transform.localPosition = temp;
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
        if(useBounds)
            temp = Mathf.Clamp(temp, minZoom, maxZoom);
        return temp;
    }
}
