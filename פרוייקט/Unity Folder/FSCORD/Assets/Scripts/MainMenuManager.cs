using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        /*GameObject[] friendlyTanks = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i<friendlyTanks.Length; i++)
        {
            Destroy(friendlyTanks[i].gameObject);

        }

        if (Score.instance.gameObject != null)
        {
            Score.instance.Destructor();
        }*/

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {
        Application.LoadLevel("FirstLevel_FuldaGap");
    }

    public void Credits()
    {
        Application.LoadLevel("Credits");
    }

    public void Tutorial()
    {
        Application.LoadLevel("Tutorial");
    }
}
