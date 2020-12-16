using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{

    public float startTime;
    public float timePassed;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed = Time.time - startTime;

        if ((int)timePassed >= 30)
        {
            gameObject.SetActive(false);
        }
    }
}
