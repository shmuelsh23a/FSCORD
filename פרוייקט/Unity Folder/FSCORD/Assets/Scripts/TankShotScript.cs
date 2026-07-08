using UnityEngine;
using System.Collections;

public class TankShotScript : MonoBehaviour
{
    float startTime;
    float endTime = 1.5f;
    // Use this for initialization
    void Start()
    {

        startTime = Time.timeSinceLevelLoad;
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad >= startTime + endTime)
        {
            Destroy(this.gameObject);
        }


    }

}