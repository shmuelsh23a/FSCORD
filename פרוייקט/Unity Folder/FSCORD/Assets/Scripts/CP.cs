using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CP : MonoBehaviour
{
    //public enum ItemIdentity { American, Soviet, Neutral };
    public ItemIdentity identity;
    public ItemType type;
    public bool capturing;
    public bool wpIsChecked;
    public bool spawnTime;
    public bool pauseTime;
    public bool finalWP;
    public bool areFrienndlyTanksJoining;
    public bool friendlySpawnTime;
    public bool areTanksComming;
    public bool shouldGetAmmo;
    public bool isCapturePoint;
    public bool isThereResupply;
    public bool isThereText;
    public bool shouldActivateWP;
    public bool isThereSoundToPlay;
    public bool wasTextShown;
    public bool canDestroy = false;
    public bool allTextRead = false;

    public float waveNumber;
    public float numberOfTanksSpawnedThiswave;
    public float lastspawnTime;
    public float waveOfFriendlyTanksJoining;
    public float numberOfTanksPerWave;
    public float numberOfEnemyWaves;
    public float timeGapBetweenTankSpawn;

    public GameObject capturingTank;
    public GameObject panel;
    public GameObject freindlyInstantPoint;
    public GameObject WPtoActivate;
    public GameObject[] enemyTanks;
    public GameObject[] friendlyTanks;
    public GameObject[] instantPoint;

    

    public Text[] story;
    public Text GUIStory;

    public Button next;
    public Button skip;

    public int getNaplam;
    public int getMines;
    public int getNukes;
    //public int getDaisyCutters;
    public int getDaisyCutters;
    public int wayPointNum;

    public AudioSource soundToPlay;

    float captureTimer;   
    bool captureIsComplete;
    
    bool wasResupplyGiven = false;
    bool wasAmmoGiven = false;
    
    float waitforCapture;
    int currentSlide;
    //public int wayPointNum;
    ItemIdentity otherIdentity;

    void Awake ()
    {
        
    }
    void Start()
    {
        waitforCapture = Game.instance.waitforCapture;

    }

    void Update()
    {
        TanksAreOnTarget();
    }

    void OnTriggerEnter(Collider other)
    {
        //GameObject otherTank = other.gameObject;
        EnteringTrigger(other);
    }

    void OnTriggerExit(Collider other)
    {
        ExitingTrigger(other);
    }

    virtual public void EnteringTrigger (Collider other)
    {
        //if (isCapturePoint == true)
        //{
        if (other.GetComponent<BasicTank>() != null)
        {
            if (capturingTank == null)
            {
                capturingTank = other.gameObject;
                if ((capturingTank.GetComponent<BasicTank>().identity != identity) && (capturingTank.GetComponent<BasicTank>().type != type))
                {
                    if ((isThereText == true)&& (wasTextShown == false) && (capturingTank.GetComponent<BasicTank>().identity == ItemIdentity.American))
                    {
                        ShowText();
                        wasTextShown = true;
                    }
                    captureTimer = Time.timeSinceLevelLoad;
                    capturing = true;
                    otherIdentity = capturingTank.GetComponent<BasicTank>().identity;
                     
                    //captureIsComplete = false;
                }
            }
        }
        //}
        //else
        //{ }
    }

    virtual public void ExitingTrigger(Collider other)
    {
        if (capturingTank != null)
        {
            if (other.gameObject == capturingTank)
            {
                // Debug.Log(other.GetComponent<BasicTank>().identity);
                if ((capturingTank.GetComponent<BasicTank>().identity == identity) && (capturingTank.GetComponent<BasicTank>().type != type))
                {
                    //Debug.Log(capturing);
                    capturing = false;
                    //identity = otherIdentity;
                    //Debug.Log(capturing);
                }
                capturingTank = null;
            }
        }
    }

    virtual public void TanksAreOnTarget()
    {
        if (capturing == true)
        {
            if (Time.timeSinceLevelLoad > (captureTimer + waitforCapture))
            {
                FlagChnagerMinus();
                identity = otherIdentity;
                FlagChnagerPlus();
                capturing = false;
                capturingTank = null;
                Ressuply();
                RecieveAmmo();
                ActivateWP();

            }
        }
    }

    virtual public void FlagChnagerMinus()
    {
        switch (identity)
        {
            case ItemIdentity.American:
                Game.instance.numberOfAmericanFlags--;
                break;

            case ItemIdentity.Soviet:
                Game.instance.numberOfSovietFlags--;
                break;
        }
    }

    virtual public void FlagChnagerPlus()
    {
        switch (identity)
        {
            case ItemIdentity.American:
                Game.instance.numberOfAmericanFlags++;
                break;

            case ItemIdentity.Soviet:
                Game.instance.numberOfSovietFlags++;
                break;
        }
    }
    virtual public void BringOnTheTanks()
    {
        if (spawnTime == true)
        {
            if /*(((Game.instance.enemyTanksInLevel < 1) &&*/ ((waveNumber <= numberOfEnemyWaves) && (pauseTime == true)) /*)|| (spawnNow == true))*/
            {
                // if (spawnTime == false)
                //{
                if (((Game.instance.enemyTanksInLevel <= 0) || (waveNumber == 0)) && (pauseTime == true))
                {
                    numberOfTanksSpawnedThiswave = 0;
                    pauseTime = false;
                }
                // spawnTime = true;
                //Game.instance.spawnNow = false;
                // }
            }



            /*else
            {
                spawnTime = false;
            }*/

            // if (spawnTime == true)
            //{
            //if (Game.instance.enemyTanksInLevel < 7)
            //{
            if ((pauseTime == false) && (numberOfTanksSpawnedThiswave <= numberOfTanksPerWave) && (Time.timeSinceLevelLoad > lastspawnTime + timeGapBetweenTankSpawn) && (waveNumber <= numberOfEnemyWaves))
            //for (int i =0; i <= numberOfTanksPerWave; i++) 
            {

                //if (instantPoint.Length > 1)
                //{
                for (int i = 0; i < instantPoint.Length; i++)
                {
                    Instantiator(instantPoint[i]);
                }
                //}
                // }
            }

            //}

            if (numberOfTanksSpawnedThiswave >= numberOfTanksPerWave)
            {
                if (pauseTime == false)
                {
                    pauseTime = true;
                    waveNumber++;
                }
            }
        }

        if (waveNumber > numberOfEnemyWaves)
        {
            wpIsChecked = true;
            /*GameObject[] tempWayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
            //GameObject[] Game.instance.wayPoints = new GameObject[];
            Game.instance.ZeroWayPoints();
            int n = 0;
            for (int i = 0; i < tempWayPoints.Length; i++)
            {
                if (tempWayPoints[i].GetComponent<WP>().wpIsChecked == false)
                {
                    Game.instance.wayPoints[n] = tempWayPoints[i];
                    n++;
                }



            }*/
            ResumeAllTanks();
            if (finalWP == true)
            {
                Game.instance.lastWPReached = true;
            }
            //Destroy(this.gameObject);
            canDestroy = true;
        }


    }

    virtual public void StopAllTanks()
    {
        Game.instance.allTankStop = true;
        GameObject[] friendlyTanks = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < friendlyTanks.Length; i++)
        {
            Debug.Log(i);
            GameObject friendlyTank = friendlyTanks[i];
            friendlyTank.GetComponent<BasicTank>().shouldStopMoving = true;
            friendlyTank.GetComponent<BasicTank>().agent.destination = friendlyTank.transform.position;
            friendlyTank.GetComponent<BasicTank>().agent.Stop();

        }
    }

    virtual public void ResumeAllTanks()
    {
        Game.instance.allTankStop = false;
        GameObject[] friendlyTanks = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < friendlyTanks.Length; i++)
        {
            //friendlyTanks[i].GetComponent<BasicTank>().agent.Resume();
            GameObject friendlyTank = friendlyTanks[i];
            friendlyTank.GetComponent<BasicTank>().shouldStopMoving = false;
            friendlyTank.GetComponent<BasicTank>().IdentifyCP();
            friendlyTank.GetComponent<BasicTank>().agent.Resume();


        }
    }

    virtual public void FriendlyJoining()
    {
        if (friendlySpawnTime == true)
        {
            StopAllTanks();
            if (numberOfEnemyWaves > 0)
            {
                if (waveNumber == waveOfFriendlyTanksJoining)
                {
                    for (int i = 0; i < friendlyTanks.Length; i++)
                    {
                        GameObject newFriendly = Instantiate(friendlyTanks[i], freindlyInstantPoint.transform.position, Quaternion.identity) as GameObject;

                    }
                    friendlySpawnTime = false;
                    wpIsChecked = true;
                    //();
                    if (areTanksComming != true)
                    {
                        ResumeAllTanks();
                            //Destroy(this.gameObject);
                        canDestroy = true;
                        
                    }
                }
            }

            else
            {
                for (int i = 0; i < friendlyTanks.Length; i++)
                {
                    GameObject newFriendly = Instantiate(friendlyTanks[i], freindlyInstantPoint.transform.position, Quaternion.identity) as GameObject;

                }
                friendlySpawnTime = false;
                wpIsChecked = true;
                
                if (areTanksComming != true)
                {
                    ResumeAllTanks();
                    //Destroy(this.gameObject);
                    canDestroy = true;
                }
            }
        }

    }

    virtual public void RecieveAmmo()
    {
        if ((shouldGetAmmo == true) && (wasAmmoGiven == false))
        {
            Game.instance.nukesLeft += getNukes;
            Game.instance.minesLeft += getMines;
            Game.instance.naplamLeft += getNaplam;
            Game.instance.daisyCutterLeft += getDaisyCutters;
            wasAmmoGiven = true;
        }

    }

    virtual public void Instantiator(GameObject instantPoint)
    {
        int tankRandomizer = Random.Range(1, 101);
        if (tankRandomizer < Game.instance.t55Chance)
        {
            GameObject t55 = Instantiate(enemyTanks[0], instantPoint.transform.position, Quaternion.identity) as GameObject;
            t55.GetComponent<BasicTank>().isPreSetTank = true;
            t55.GetComponent<BasicTank>().agent.destination = this.transform.position;
            numberOfTanksSpawnedThiswave++;
        }

        if ((tankRandomizer > Game.instance.t55Chance) && (tankRandomizer <= Game.instance.t62Chance))
        {
            GameObject t62 = Instantiate(enemyTanks[1], instantPoint.transform.position, Quaternion.identity) as GameObject;
            t62.GetComponent<BasicTank>().isPreSetTank = true;
            t62.GetComponent<BasicTank>().agent.destination = this.transform.position;
            numberOfTanksSpawnedThiswave++;
        }

        if ((tankRandomizer > Game.instance.t62Chance) && (tankRandomizer <= Game.instance.t72Chance))
        {
            GameObject t72 = Instantiate(enemyTanks[2], instantPoint.transform.position, Quaternion.identity) as GameObject;
            t72.GetComponent<BasicTank>().isPreSetTank = true;
            t72.GetComponent<BasicTank>().agent.destination = this.transform.position;
            numberOfTanksSpawnedThiswave++;
        }
        lastspawnTime = Time.timeSinceLevelLoad;
        //numberOfTanksSpawnedThiswave++;
    }

    virtual public void Ressuply()
    {
        if ((isThereResupply == true) && (wasResupplyGiven == false))
        {
            GameObject[] friendlyTanks = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < friendlyTanks.Length; i++)
            {
                friendlyTanks[i].GetComponent<BasicTank>().hitPoints = friendlyTanks[i].GetComponent<BasicTank>().initialHitPoints;
                Debug.Log("tank " + friendlyTanks[i] + " HP " + friendlyTanks[i].GetComponent<BasicTank>().hitPoints);
            }
            wasResupplyGiven = true;
        }
    }

    virtual public  void ShowText()
    {
        if (isThereSoundToPlay == true)
        {
            soundToPlay.Play();
        }
        Game.instance.WPActive = wayPointNum;
        Time.timeScale = 0;
        panel.SetActive(true);
        skip.gameObject.SetActive(true);
        GUIStory.text = story[0].text;
        if (story.Length > 1)
        {
            next.gameObject.SetActive(true);
        }
        currentSlide = 0;


    }

    virtual public void NextText()
    {
        if (currentSlide < story.Length)
        {
            currentSlide++;
            GUIStory.text = story[currentSlide].text;
        }

        if (currentSlide == (story.Length) - 1)
        {
            next.gameObject.SetActive(false);
        }

    }

    virtual public void ActivateWP()
    {
        if (shouldActivateWP == true)
        {
            WPtoActivate.SetActive(true);
            Game.instance.UpdateWayPoints();
        }
    }
}
