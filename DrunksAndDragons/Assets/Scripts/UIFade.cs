using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D), typeof(CanvasGroup))]
public class UIFade : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;
    List<GameObject> players = new List<GameObject>();
    

    Collider2D collider;
    CanvasGroup cvGroup;

    // Start is called before the first frame update
    void Start()
    {
        cvGroup = GetComponent<CanvasGroup>();
        collider = GetComponent<Collider2D>();

        if (!blackboard)
            Debug.Log("No reference to blackboard");
        else
            players = blackboard.players;

        if (players.Count == 0)
            Debug.Log("players was not filled");
    }

    // Update is called once per frame
    void Update()
    {
        bool overlap = false;
        foreach(GameObject player in players)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);
            
            if(collider.OverlapPoint(screenPoint))
            {
                overlap = true;
            }
        }

        if(overlap)
        {
            cvGroup.alpha -= Time.deltaTime;
            if (cvGroup.alpha < 0.25f)
                cvGroup.alpha = 0.25f;
        }
        else
        {
            cvGroup.alpha += Time.deltaTime;
        }
    }
}
