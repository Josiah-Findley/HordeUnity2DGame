using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScript : MonoBehaviour
{

    //miniMaps
    public GameObject MiniMap1;
    public GameObject MiniMap2;

    public bool showing;
    public bool showing2;

    public void Awake()
    {
        showing = false;
        showing2 = true;
        Hide(MiniMap1);
        Hide(MiniMap2);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("P1_Minimap"))
        {
            if (showing)
            {
                Hide(MiniMap1);
                showing = false;
            }
            else {
                Show(MiniMap1);
                showing = true;
            }
            
        }

        if (Input.GetButtonDown("P2_Minimap"))
        {
            if (showing2)
            {
                Hide(MiniMap2);
                showing2 = false;
            }
            else
            {
                Show(MiniMap2);
                showing2 = true;
            }

        }
    }

    //MiniMap HIDE AND SHOW
    public void Show(GameObject MiniMap)
    {
        MiniMap.SetActive(true);
    }

    public void Hide(GameObject MiniMap)
    {
        MiniMap.SetActive(false);
    }
}
