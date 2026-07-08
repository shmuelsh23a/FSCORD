using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
    float startTime;
    public float endTime;
    // Use this for initialization
    void Start () {
        startTime = Time.timeSinceLevelLoad;

    }
	
	// Update is called once per frame
	void Update () {

        if (Time.timeSinceLevelLoad >= startTime + endTime)
        {
            Destroy(this.gameObject);
        }
    }
}
