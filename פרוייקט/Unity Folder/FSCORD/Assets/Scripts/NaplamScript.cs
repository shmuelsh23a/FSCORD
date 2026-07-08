using UnityEngine;
using System.Collections;

public class NaplamScript : MonoBehaviour {
    float startTime;
    float endTime;

	// Use this for initialization
	void Start () {

        startTime = Time.timeSinceLevelLoad;
        endTime = Game.instance.naplamBurningTime;
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.timeSinceLevelLoad >= startTime+endTime)
        {
            Destroy(this.gameObject);
        }

	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BasicTank>() != null)
        {
            //Debug.Log(other);
            if ((other.GetComponent<BasicTank>().identity != ItemIdentity.American) && (other.GetComponent<BasicTank>().type == ItemType.Tank))
            {
                other.GetComponent<BasicTank>().burning = true;
            }
        }
    }
}
