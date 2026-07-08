using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankHUDScript : MonoBehaviour {

    public Slider health;
    public Text level;

	// Use this for initialization
	void Start () {
        health.maxValue = this.gameObject.GetComponent<BasicTank>().hitPoints;
	}
	
	// Update is called once per frame
	void Update () {
        health.value = this.gameObject.GetComponent<BasicTank>().hitPoints;
        level.text = this.gameObject.GetComponent<BasicTank>().currentLevel.ToString();
    }
}
