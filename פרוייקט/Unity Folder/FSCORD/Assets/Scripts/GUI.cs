using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI : MonoBehaviour {
    public Text naplam;
    public Text mines;
    public Text nukes;
    public Text daisyCutter;
    public Text timer;
    int minutesLeft;
    int secondsLeft;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        minutesLeft = (int) (Game.instance.levelTimeLimit - Game.instance.levelTimer) / 60;
        secondsLeft = (int)(Game.instance.levelTimeLimit - Game.instance.levelTimer) % 60;
        naplam.text = "Naplam Left: " + Game.instance.naplamLeft.ToString();
        mines.text = "Mines Left: " + Game.instance.minesLeft.ToString();
        nukes.text = "Nukes Left: " + Game.instance.nukesLeft.ToString();
        daisyCutter.text = "Daisy Cutters Left: " + Game.instance.daisyCutterLeft.ToString();
        timer.text = "Time Left: " + minutesLeft.ToString() + " : " + secondsLeft.ToString();

    }
}
