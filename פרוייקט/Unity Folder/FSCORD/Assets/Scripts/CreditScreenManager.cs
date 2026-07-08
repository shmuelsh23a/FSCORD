using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditScreenManager : MonoBehaviour {

    public Button mainMenu;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MainMenu()
    {
        Application.LoadLevel("Main_Menu");
    }

    public void MoreCredits()
    {
        Application.LoadLevel("More Credits");
    }
}
