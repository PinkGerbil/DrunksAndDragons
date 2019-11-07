using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private bool activated;
    public GameObject shopUI;

    private float lerpInProgress;
    private float lerpOutProgress;

    public float lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            if (lerpInProgress < 1)
            {
                lerpOutProgress = 0;
                lerpInProgress += Time.deltaTime * lerpSpeed;
                shopUI.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(shopUI.GetComponent<CanvasGroup>().alpha, 1, lerpInProgress);
            }
        }
        else
        {
            if (lerpOutProgress < 1)
            {
                lerpInProgress = 0;
                lerpOutProgress += Time.deltaTime * lerpSpeed;
                shopUI.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(shopUI.GetComponent<CanvasGroup>().alpha, 0, lerpOutProgress);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !activated)
        {
            activated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && activated)
        {
            activated = false;
        }
    }
}
