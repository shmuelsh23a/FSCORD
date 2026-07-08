using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour {

    public Button nextLevel;
    public Button newGame;
    public Text gameOutcome;
    public Text naplamLeft;
    public Text minesLeft;
    public Text nukesLeft;
    public Text friendlyTanks;
    //public string nextLevel;
    //public bool won;
    public Text totalNumberOfEnemyTanksDestroyed;
    public Text numberOfEnemyTanksDestroyedThisLevel;
    public Image canvas;
    public Sprite victoryTexture;
    public Sprite defeatTexture;


    // Use this for initialization
    void Start()
    {

        if (Score.instance.totalDefeat != true)
        {
            nextLevel.gameObject.SetActive(true);
        }
        else
        //if (Score.instance.totalDefeat == true)
        {
            newGame.gameObject.SetActive(true);
        }


        switch (Score.instance.won)
        {
            case true:
                gameOutcome.text = "Victory";
                canvas.sprite = victoryTexture;
                break;

            case false:
                gameOutcome.text = "Defeat";
                canvas.sprite = defeatTexture;
                break;
        }
        naplamLeft.text = "Number of naplam strikes left: " + Score.instance.naplamLeft.ToString();
        minesLeft.text = "Number of mine strikes left: " + Score.instance.minesLeft.ToString();
        nukesLeft.text = "Number of nukes left: " + Score.instance.nukesLeft.ToString();
        friendlyTanks.text = "Number of freindly tanks left: " + Score.instance.friendlyTanks.Length.ToString();
        totalNumberOfEnemyTanksDestroyed.text = "Total number of enemy tanks destroyed: " + Score.instance.totalNumberOfEnemyTanksDestroyed.ToString();
        numberOfEnemyTanksDestroyedThisLevel.text = "Number of enemy tanks destroyed in this level: " + Score.instance.numberOfEnemyTanksDestroyedThisLevel.ToString();
    }




	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextLevel()
    {
        Application.LoadLevel(Score.instance.nextLevel);
    }

    public void NewGame()
    {
        ClearLevel();
        Application.LoadLevel("FirstLevel_FuldaGap");
    }

    public void MainMenu()
    {
        ClearLevel();
        Application.LoadLevel("Main_Menu");
    }

    void ClearLevel()
    {

        Score.instance.Destructor();
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < tanks.Length; i++)
        {
            Destroy(tanks[i].gameObject);
        }
    }
}
