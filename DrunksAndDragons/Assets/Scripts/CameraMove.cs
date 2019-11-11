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
    [Tooltip("Enable the bounds limiting the camera's X and Z positions. (when disabled, x/zMinBounds and x/zMaxBounds do nothing)")]
    [SerializeField]
    bool usePositionBounds = true;
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
    [Header("Zoom Limit")]
    [Tooltip("Enable the limiters for zooming. (when disabled, maxZoom and minZoom do nothing)")]
    [SerializeField]
    bool useZoomBounds = true;
    [SerializeField]
    [Range(0, 40)]
    float maxZoom = 20;
    [SerializeField]
    [Range(0, 40)]
    float minZoom = 0;

    // Start is called before the first frame update
    void Start()
    {
        originPoint = transform.position;

        if (!blackboard)
            blackboard = GameObject.Find("Game Manager").GetComponent<Blackboard>();

        players = blackboard.players;
        

    }
    

    // Update is called once per frame
    void Update()
    {
        
        if (players.Count == 0)
        {
            players = blackboard.players;
        }
        else
        {
            nextPos = CalcCamPos();
            nextZoom = CalcCamZoom();
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, nextPos + -transform.forward * nextZoom, camMoveSpeed * Time.deltaTime);

        if(usePositionBounds)
        {
            Vector3 temp = transform.localPosition;
            if (temp.x < xMinBounds)
                temp.x = xMinBounds;
            if (temp.x > xMaxBounds)
                temp.x = xMaxBounds;

            if (temp.z < zMinBounds)
                temp.z = zMinBounds;
            if (temp.z > zMaxBounds)
                temp.z = zMaxBounds;

            transform.localPosition = temp;
        }

        nextZoom = 0;
    }

    Vector3 CalcCamPos()
    {
        Vector3 temp = Vector3.zero;


        foreach (GameObject child in players)
            temp += child.transform.position;
        temp.z += 20;
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
        if(useZoomBounds)
            temp = Mathf.Clamp(temp, minZoom, maxZoom);
        return temp / PlayerScale;
    }
}
