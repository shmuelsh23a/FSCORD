using UnityEngine;
using System.Collections;

public class MineScript : MonoBehaviour {
    float damage;
    public GameObject explosion;
	// Use this for initialization
	void Start () {
        damage = Game.instance.mineDamage;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BasicTank>() != null)
        {
            if (other.gameObject.GetComponent<BasicTank>().identity != ItemIdentity.American)
            {
                other.gameObject.GetComponent<BasicTank>().hitPoints -= damage;
                Instantiate(explosion, this.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
}
