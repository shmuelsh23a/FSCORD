using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Game : MonoBehaviour
{
    public static Game instance;
    public GameObject[] controlPoints;
    GameObject[] enemyTanks;
    public GameObject[] wayPoints;
    public GameObject[] friendly;
    public float waitforCapture;
    public VictoryType victoryType;
    public float numberOfenemyTanksperWave; // Size of each wave for CTF levels
    public float enemyWaveSize; // Size of Soviet wave for CTF levels;
    public float enemyTanksInLevel = 0; // number of Soviet tanks currently in level
    public float numberOfEnemyTanksDestroyed; // number of Soviet tanks destroyed
    public float numberOfTanksSpawned = 0; // number of Soviet tanks spwaned during the level
    public float totalNumberOfWaves; // Total number of waves for CTF levels
    public float totalNumberOfTanks; // Total number of Soviet tanks that should be destroyed to win in Elimination levels
    float numberOfFlags;
    public float numberOfAmericanFlags;
    public float numberOfSovietFlags;
    public bool spawnNow;
    public int t55Chance;
    public int t62Chance;
    public int t72Chance;
    public int naplamLeft;
    public int minesLeft;
    public int nukesLeft;
    public int daisyCutterLeft;
    public float mineDamage;
    public int concentratedLeft;
    public bool allTankStop = false;
    public float numberOfAmericanTanks = 0;
    public bool lastWPReached;
    public float naplamBurningTime;
    public GameObject startPoint;
    public float levelTimeLimit;
    public float levelTimer;
    public int WPActive;
    public bool isThereTextInTheBegeining;
    public GameObject panel;
    public Text[] story;
    public Text[] victory;
    public Text[] defeat;
    public Text[] totalDefeat;
    public Text GUIStory;
    public int currentSlide;
    public Button next;
    public Button skip;
    public Button endLevel;
    public string nextLevel;
    public string nextLevelVictory;
    public string nextLevelDefeat;
    public bool won;
    public bool isTotalDefeat;
    public bool shouldGetAmmoFromTanks;
    bool endingText;
    public AudioSource[] soundToPlay;
    public bool isThereSoundToPlay;
    public bool shouldGetNuke;
    bool didWrapUpScore = false;
    //bool lost;
    //public float burnDamage;
    //public float loadTime;
    //public enum ItemIdentity { American, Soviet, Neutral };
    void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Singleton error!");
        }
        else
        {
            instance = this;
            return;
        }
        //LoadHashtable();

    }
    // Use this for initialization
    void Start()
    {
        
        if (Application.loadedLevelName != "FirstLevel_FuldaGap")
        {
            friendly = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < Score.instance.friendlyTanks.Length; i++)
            {
                friendly[i].GetComponent<NavMeshAgent>().enabled = false;
                Vector3 instaPos = new Vector3(startPoint.transform.position.x + (i * 10), startPoint.transform.position.y, startPoint.transform.position.z);
                friendly[i].transform.position = instaPos;
                friendly[i].GetComponent<NavMeshAgent>().enabled = true;
                //GameObject friendlyTank = Instantiate(Score.instance.friendlyTanks[i], instaPos, Quaternion.identity) as GameObject;
            }
            numberOfAmericanTanks = friendly.Length;
            minesLeft += Score.instance.minesLeft;
            naplamLeft += Score.instance.naplamLeft;
            nukesLeft += Score.instance.nukesLeft;
            

        }

        UpdateWayPoints();
        enemyTanks = GameObject.FindGameObjectsWithTag("NPC");
                
        numberOfFlags = controlPoints.Length;
        if (isThereTextInTheBegeining == true)
        {
            ShowText(story);
        }

            levelTimer = Time.timeSinceLevelLoad;
        //if (enemyTanksInLevel > 0)
        // {
        // spawnNow = false;
        //}
        //numberOfEnemyTanksInLevel = enemyTanks.Length;
        /*foreach (GameObject CP in controlPoints )
        {
            Debug.Log(CP.transform.position);
        }*/
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("numberOfEnemyTanksDestroyed" + numberOfEnemyTanksDestroyed);
        EndGame();
        levelTimer = Time.timeSinceLevelLoad;

    }

    public void ZeroWayPoints()
    {
        Array.Clear(wayPoints, 0, wayPoints.Length);
    }

    void EndGame()
    {
        switch (victoryType)
        {
            case VictoryType.CaptureTheFlag:
                //WrapUpScore();
                CTFVictory();                
                break;

            case VictoryType.Elimination:
                //WrapUpScore();
                EliminationVictory();               
                break;

            case VictoryType.Points:
                //WrapUpScore();
                break;

            case VictoryType.WayPoints:
                //WrapUpScore();
                MissionAccomplished();
                
                break;
        }

        if (numberOfAmericanTanks < 1)
        {
            nextLevel = "Main_Menu";
            Debug.Log("USSR Supreme");
            won = false;
            isTotalDefeat = true;
            WrapUpScore();
            ShowText(totalDefeat);
            
           // Application.Quit();
        }

        if (levelTimer > levelTimeLimit)
        {
            nextLevel = nextLevelDefeat;
            Debug.Log("USSR Supreme");
            won = false;
            WrapUpScore();
            ShowText(defeat);
            //Application.Quit();
        }
    }

    void CTFVictory()
    {
        if (numberOfAmericanFlags == numberOfFlags)
        {
            
            nextLevel = nextLevelVictory;
            Debug.Log("America Victoria");
            won = true;
            WrapUpScore();
            ShowText(victory);
            //Application.Quit();
        }

        if (numberOfSovietFlags == numberOfFlags)
        {
            
            nextLevel = nextLevelDefeat;
            Debug.Log("USSR Supreme");
            won = false;
            WrapUpScore();
            ShowText(defeat);
            
            //Application.Quit();
        }
    }

    void EliminationVictory()
    {
        if (numberOfEnemyTanksDestroyed >= totalNumberOfTanks)
        {
            
            nextLevel = nextLevelVictory;
            Debug.Log("America Victoria");
            won = true;
            WrapUpScore();
            ShowText(victory);
            
            //Application.Quit();
        }
    }

    void MissionAccomplished()
    {
        if (lastWPReached == true)
        {
            
            nextLevel = nextLevelVictory;
            Debug.Log("Mission Acomplished");
            won = true;
            WrapUpScore();
            ShowText(victory);
            
            //Application.Quit();
        }
    }

    void WrapUpScore()
    {
        if (didWrapUpScore == false)
        {

            didWrapUpScore = true;
            Score.instance.SumThingsUp();
            endingText = true;
        }
    }

    void ShowText(Text[] text)
    {
        if (isThereSoundToPlay == true)
        {
            for (int i = 0; i < soundToPlay.Length; i++)

            {
                soundToPlay[i].Play();
            }
        }
        WPActive = -1;
        Time.timeScale = 0;
        panel.SetActive(true);
        if (endingText == true)
        {
            endLevel.gameObject.SetActive(true);
        }
        else
        {
            skip.gameObject.SetActive(true);
        }

        if (text.Length == 1)
        {
            GUIStory.text = text[0].text;
        }

        else if (text.Length > 1)
        {
            GUIStory.text = text[0].text;
            next.gameObject.SetActive(true);
        }
        currentSlide = 0;


    }

    public void NextText(Text[] text)
    {
        if (currentSlide < text.Length)
        {
            currentSlide++;
            GUIStory.text = text[currentSlide].text;
        }

        if (currentSlide == (text.Length) - 1)
        {
            next.gameObject.SetActive(false);
        }



    }

    public void UpdateWayPoints()
    {
        controlPoints = GameObject.FindGameObjectsWithTag("ControlPoint");
        //enemyTanks = GameObject.FindGameObjectsWithTag("npc");
        wayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
    }
}
