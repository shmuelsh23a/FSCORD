using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WP : CP
{
    //public ItemIdentity identity;
   // bool wasTextShown = false;
    /*public bool areTanksComming;
    public GameObject instantPoint;
    public float numberOfTanksPerWave;
    public float numberOfEnemyWaves;
    public float timeGapBetweenTankSpawn;*/
    //public bool isCP;
    //bool was
    
    /*bool spawnTime;
    public float waveNumber;
    float numberOfTanksSpawnedThiswave;
    float lastspawnTime;
    public GameObject[] enemyTanks;
    bool pauseTime;
    public bool finalWP;
    public bool areFrienndlyTanksJoining;
    bool friendlySpawnTime;
    public GameObject[] friendlyTanks;
    float waveOfFriendlyTanksJoining;
    public GameObject freindlyInstantPoint;*/
    /*public ItemIdentity identity;
    public ItemType type;
    float captureTimer;
    public bool capturing;
    bool captureIsComplete;
    ItemIdentity otherIdentity;
    float waitforCapture;
    public bool isCapturePoint;
    
    public bool wpIsChecked;*/
    // Use this for initialization
    void Start()
    {
        wpIsChecked = false;
        waveNumber = 0;
        //isCapturePoint = isCP;

    }

    // Update is called once per frame
    void Update()
    {
        BringOnTheTanks();
        FriendlyJoining();
        DestroyWP();
        //Ressuply();
    }

    override public void EnteringTrigger(Collider other)
    {
        if (other.GetComponent<BasicTank>() != null)
        {
            
            if (other.GetComponent<BasicTank>().identity == ItemIdentity.American)
            {
                RecieveAmmo();
                Ressuply();
                ActivateWP();
                
                if ((isThereText == true) && (wasTextShown == false))
                {
                    ShowText();
                    wasTextShown = true;
                }

                
                if (areFrienndlyTanksJoining == true)
                {
                    friendlySpawnTime = true;
                }

                if (areTanksComming == true)
                {
                    spawnTime = true;
                    pauseTime = true;

                    //this.gameObject.GetComponent<Collider>().enabled = false;
                    StopAllTanks();
                }

                if ((areTanksComming != true) && (areFrienndlyTanksJoining != true))
                {
                    ResumeAllTanks();
                    //Destroy(this.gameObject);
                    canDestroy = true;

                }

                //else 
            }
        }
    }

    public void DestroyWP()
    {
        if (isThereText == false)
        {
            allTextRead = true;
        }

        if ((canDestroy == true) && (allTextRead == true))
        {
            if (finalWP == true)
            {
                Game.instance.lastWPReached = true;
                if (Game.instance.enemyTanksInLevel > 0)
                {
                    GameObject[] enemyTanks = GameObject.FindGameObjectsWithTag("NPC");
                    for (int i = 0; i < enemyTanks.Length; i++)
                    {
                        Destroy(enemyTanks[i].gameObject);
                    }
                    Game.instance.enemyTanksInLevel = 0;
                }
            }
            Destroy(this.gameObject);
        }
    }

    

   /* void BringOnTheTanks()
    {
        if (spawnTime == true)
        {
            if /*(((Game.instance.enemyTanksInLevel < 1) &&*/ /* ((waveNumber <= numberOfEnemyWaves) && (pauseTime == true)) /*)|| (spawnNow == true))*/
           /* {
                // if (spawnTime == false)
                //{
                if ((Game.instance.enemyTanksInLevel <= 0) && (pauseTime == true))
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
         /*   if (Game.instance.enemyTanksInLevel < 7)
            {
                if ((pauseTime == false) && (numberOfTanksSpawnedThiswave <= numberOfTanksPerWave) && (Time.timeSinceLevelLoad > lastspawnTime + timeGapBetweenTankSpawn))
                //for (int i =0; i <= numberOfTanksPerWave; i++) 
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

                    if (tankRandomizer <= Game.instance.t72Chance)
                    {
                        GameObject t72 = Instantiate(enemyTanks[2], instantPoint.transform.position, Quaternion.identity) as GameObject;
                        t72.GetComponent<BasicTank>().isPreSetTank = true;
                        t72.GetComponent<BasicTank>().agent.destination = this.transform.position;
                        numberOfTanksSpawnedThiswave++;
                    }
                    lastspawnTime = Time.timeSinceLevelLoad;
                    numberOfTanksSpawnedThiswave++;
                }
            }

            //}

            if (numberOfTanksSpawnedThiswave >= numberOfTanksPerWave)
            {
                if (pauseTime == false)
                {
                    pauseTime = true;
                    waveNumber ++;
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
        /*    ResumeAllTanks();
            if (finalWP == true)
            {
                Game.instance.lastWPReached = true;
            }
            Destroy(this.gameObject);
        }


    }

    void StopAllTanks()
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

    void ResumeAllTanks()
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

    void FriendlyJoining()
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
                    ResumeAllTanks();
                    if (areTanksComming != true)
                    {
                        Destroy(this.gameObject);
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
                ResumeAllTanks();
                if (areTanksComming != true)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        
    }*/
    
}



