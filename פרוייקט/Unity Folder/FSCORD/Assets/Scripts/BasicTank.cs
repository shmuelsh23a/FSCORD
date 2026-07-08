using UnityEngine;
using System.Collections;

public class BasicTank : MonoBehaviour {

    public bool isPreSetTank;
    public ItemIdentity identity;
    public ItemType type;
    public NavMeshAgent agent;
    GameObject chosenCP;
    GameObject interimCP;
    GameObject target;
    public GameObject muzzel;
    public GameObject shell;
    public GameObject turret;
    float rangeToTarget;
    Vector3 oldLocation;
    float distanceA;
    float distanceB;
    float captureTimer;
    bool justMoved;
    public bool capturing;
    float timeSinceLastCPCheck;
    Vector3 one;
    bool firstTimeGo = true;
    float waitforCapture;
    //public float loadTime;
    public float tankRange;
    public bool beenkilled;
    float firingTime;
    float prevFiringTime = 0;
    public float tankLoadTime;
    public float hitPoints;
    public float initialHitPoints;
    public float hitChance;
    float enemyHitPoints;
    public float beenKilledTime;
    public bool burning;
    float naplamBurnTime;
    public float burningStartTime;
    public float burningDamage;
    float burnIntervalTimer;
    public bool didBurnLastTime;
    public float armorPoints;
    public float damage;
    Vector3 turretDirection;
    Quaternion turretLookRotation;
    public float turretRotationSpeed;
    //Vector3 eP;
    bool isFirstPass;
    int oldWPNum;
    int newWPNum;
    public GameObject tankShot;
    public bool shouldStopMoving = false;
    public int XPWorth;
    public float currentXP;
    public int currentLevel;
    public int levelThreshold;
    TankSight tankSight;
    float bonus;

    //public bool isCapturePoint;

    //TankHealth targetHealth;

    // Use this for initialization
    void Awake () {
        agent = this.GetComponent<NavMeshAgent>();
        tankSight = this.GetComponent<TankSight>();
        if (identity == ItemIdentity.American)
        {
            DontDestroyOnLoad(this.transform.gameObject);
           /* if (Application.loadedLevelName != "FirstLevel_FuldaGap")
            {
                Vector3 instaPos = new Vector3(Game.instance.startPoint.transform.position.x + (Random.Range(1,10)), Game.instance.startPoint.transform.position.y, Game.instance.startPoint.transform.position.z);
                this.transform.position = instaPos;
                Game.instance.numberOfAmericanTanks++;
            }*/
        }
        
        //IdentifyCP();
        //MovetoCP();

        //agent.destination = goal.position;
        /* for (int i = 0; i < Game.instance.controlPoints.Length; i++)
         {
             if (i == 0)
             {
                 oldLocation = Game.instance.controlPoints[i].transform.position;
                 interimCP = Game.instance.controlPoints[i];
             }
             else
             {
                 distanceA = Vector3.Distance(this.transform.position, Game.instance.controlPoints[i].transform.position);
                 distanceB = Vector3.Distance(this.transform.position, oldLocation);
                 if (distanceA < distanceB)
                 {
                     oldLocation = Game.instance.controlPoints[i].transform.position;
                     interimCP = Game.instance.controlPoints[i];
                 }
             }
         }
         chosenCP = interimCP;
         //Debug.Log(chosenCP);
         agent.destination = chosenCP.transform.position; */
    }

    // Update is called once per frame
    void Start ()
    {

        agent = this.GetComponent<NavMeshAgent>();
        tankSight = this.GetComponent<TankSight>();
        naplamBurnTime = Game.instance.naplamBurningTime;
        timeSinceLastCPCheck = Time.timeSinceLevelLoad;
        waitforCapture = Game.instance.waitforCapture;
        beenkilled = false;
        switch (identity)
        {
            case ItemIdentity.Soviet:
                Game.instance.enemyTanksInLevel++;
                Game.instance.numberOfTanksSpawned++;
                break;

            case ItemIdentity.American:
                Game.instance.numberOfAmericanTanks++;
                break;
        }
       
        //loadTime = Game.instance.loadTime;
    }
    void FixedUpdate()
    {
        BeenKilled();
        /* if (this.identity == ItemIdentity.Soviet)
        {
            //Debug.Log(agent.destination);
        }*/
        if (burning == true)
        {
            Burning();
        }

        /*if ((hitPoints <= 0) && (beenkilled != true))
        {
            BeenKilled();
        }*/

       /* if (beenkilled == true) 
		{ 
			if (Time.timeSinceLevelLoad > beenKilledTime + 3f)
			{
				if (identity == ItemIdentity.Soviet)
				{
					Game.instance.enemyTanksInLevel --;
					Game.instance.numberOfEnemyTanksDestroyed ++;
				}
				Destroy (this.gameObject);
			}
		} */
		//else 
		//{
			if (tankSight.enemyInSight == true)
        	{
            	Shoot();
        	}
            /*else
            {
                agent.Resume();
            }*/
            /*if ((isPreSetTank == false) &&*/ if (Game.instance.allTankStop == false)// (Game.instance.controlPoints.Length>0))
            {
                if (firstTimeGo == true)
                {
                    IdentifyCP();
                    firstTimeGo = false;
                }

                if ((justMoved != false) && (capturing != true))
                {
                    IdentifyCP();
                    // MovetoCP();
                }
                /*if (hitPoints < 0)
                {
                    Destroy(this.gameObject);
                }*/
                //if (beenkilled == true)

                
                //if (Game.instance.allTankStop == false)
                //{
                    OccasionalCPRechack();
               // }
           // }

            if ((shouldStopMoving == true) && (target != null))
            {

            }
            
        }
        LevelUp();
    }

    void OnTriggerEnter(Collider other)
    {
        //GameObject ControlP = 
        if (other.GetComponent<CP>() != null)
        {
            if ((other.GetComponent<CP>().identity != identity) && (other.GetComponent<CP>().type != type))
            {
                captureTimer = Time.timeSinceLevelLoad;
                capturing = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        capturing = false;
    }

    void OnTriggerStay(Collider other)
    {
        if ((other.GetComponent<CP>() != null) && (Game.instance.controlPoints.Length > 0))// || (other.GetComponent<WP>() != null))
        {
            if (Time.timeSinceLevelLoad > (captureTimer + waitforCapture) || (other.GetComponent<CP>().identity == identity))
            {
                capturing = false;
                IdentifyCP();
            }
        }
    }

        /*for (int i = 0; i < Game.instance.controlPoints.Length; i++)
        {
            if (i == 0)
            {
                oldLocation = Game.instance.controlPoints[i].transform.position;
                interimCP = Game.instance.controlPoints[i];
            }
            else
            {
                distanceA = Vector3.Distance(this.transform.position, Game.instance.controlPoints[i].transform.position);
                distanceB = Vector3.Distance(this.transform.position, oldLocation);
                if (distanceA > distanceB)
                {
                    oldLocation = Game.instance.controlPoints[i].transform.position;
                    interimCP = Game.instance.controlPoints[i];
                }
            }
        }
        chosenCP = interimCP;
        //Debug.Log(chosenCP);
        agent.destination = chosenCP.transform.position;*/



 

   public void IdentifyCP ()
    {
        if (agent.enabled == true)
		{
		 isFirstPass = true;
            //Debug.Log(Game.instance.controlPoints.Length);
            if (Game.instance.controlPoints.Length > 0)
            {
                InCaseItIsCP();
            }
            else if (Game.instance.wayPoints.Length>0)
            {
                InCaseItIsWP();
            }
        }
        
        chosenCP = interimCP;
        one = this.transform.position;
        //Debug.Log(chosenCP);
        if (chosenCP != null)
        { if ((chosenCP.transform.position != this.transform.position) && (agent.enabled == true)) //this.transform.position)
            {
                agent.destination = chosenCP.transform.position;
                justMoved = true;
            }
            else
            {
                justMoved = false;
            }
        }
        //Debug.Log(chosenCP);
        //agent.destination = chosenCP.transform.position;
    }

    /*void MovetoCP ()
    {
        if (chosenCP.transform.position != this.transform.position)
        {
            agent.destination = chosenCP.transform.position;
            justMoved = true;
        }
    }*/
    void InCaseItIsCP()
    {
            for (int i = 0; i < Game.instance.controlPoints.Length; i++)
            {
                //Debug.Log(Game.instance.controlPoints.Length);
                ItemIdentity currentCP = Game.instance.controlPoints[i].GetComponent<CP>().identity;

                if ((currentCP != identity) && (Game.instance.controlPoints[i].GetComponent<CP>().capturing != true))
                {
                    if (i == 0)
                    {
                        oldLocation = Game.instance.controlPoints[i].transform.position;
                        //Debug.Log(oldLocation);
                        interimCP = Game.instance.controlPoints[i];
                        //Debug.Log(interimCP);
                        isFirstPass = false;
                        // Debug.Log(identity, interimCP);
                    }
                    else
                    {
                        if (isFirstPass == true)
                        {
                            oldLocation = Game.instance.controlPoints[i].transform.position;
                            interimCP = Game.instance.controlPoints[i];
                            isFirstPass = false;
                        }
                        distanceA = Vector3.Distance(this.transform.position, Game.instance.controlPoints[i].transform.position);
                        distanceB = Vector3.Distance(this.transform.position, oldLocation);
                        if (distanceA < distanceB)
                        {
                            oldLocation = Game.instance.controlPoints[i].transform.position;
                            interimCP = Game.instance.controlPoints[i];
                        }
                    }
                }
            }
        
    }

    void InCaseItIsWP()
    {
        if (identity == ItemIdentity.American)

        {
            for (int i = 0; i < Game.instance.wayPoints.Length; i++)
            {
                //Debug.Log(Game.instance.controlPoints.Length);
                //ItemIdentity currentCP = Game.instance.wayPoints[i].GetComponent<WP>().identity;

                if (Game.instance.wayPoints[i] != null && (Game.instance.wayPoints[i].GetComponent<WP>() != null)) //(currentCP != identity) && (Game.instance.controlPoints[i].GetComponent<CP>().capturing != true))
                {
                    if (i == 0)
                    {
                        //oldLocation 
                        oldWPNum = Game.instance.wayPoints[i].GetComponent<WP>().wayPointNum;
                        //Debug.Log(oldLocation);
                        interimCP = Game.instance.wayPoints[i];
                        //Debug.Log(interimCP);
                        isFirstPass = false;
                        // Debug.Log(identity, interimCP);
                    }
                    else
                    {
                        if (isFirstPass == true)
                        {
                            oldWPNum = Game.instance.wayPoints[i].GetComponent<WP>().wayPointNum;
                            interimCP = Game.instance.wayPoints[i];
                            isFirstPass = false;
                        }
                        //distanceA = Vector3.Distance(this.transform.position, Game.instance.controlPoints[i].transform.position);
                        //distanceB = Vector3.Distance(this.transform.position, oldLocation);
                        if ((oldWPNum >= Game.instance.wayPoints[i].GetComponent<WP>().wayPointNum) && (Game.instance.wayPoints[i].GetComponent<CP>().wpIsChecked == false))
                        {
                            oldLocation = Game.instance.wayPoints[i].transform.position;
                            interimCP = Game.instance.wayPoints[i];
                        }
                    }
                }
            }

        }
        else
        {
            GameObject[] americanTanks = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < americanTanks.Length; i++)
            {
                //Debug.Log(Game.instance.controlPoints.Length);
                //ItemIdentity currentCP = Game.instance.wayPoints[i].GetComponent<WP>().identity;


                if (i == 0)
                {
                    oldLocation = americanTanks[i].transform.position;
                    //Debug.Log(oldLocation);
                    interimCP = americanTanks[i];
                    //Debug.Log(interimCP);
                    isFirstPass = false;
                    // Debug.Log(identity, interimCP);
                }
                else
                {
                    if (isFirstPass == true)
                    {
                        oldLocation = americanTanks[i].transform.position;
                        interimCP = americanTanks[i];
                        isFirstPass = false;
                    }
                    distanceA = Vector3.Distance(this.transform.position, americanTanks[i].transform.position);
                    distanceB = Vector3.Distance(this.transform.position, oldLocation);
                    if (distanceA < distanceB)
                    {
                        oldLocation = americanTanks[i].transform.position;
                        interimCP = americanTanks[i];
                    }
                }
            
            }
        }
    }

    
    void OccasionalCPRechack()
    {
        if (Time.timeSinceLevelLoad> (timeSinceLastCPCheck +5))
        {
            IdentifyCP();
            timeSinceLastCPCheck = Time.timeSinceLevelLoad;
        }
    }

    void Shoot()
    {
        //TankSight tankSight = this.GetComponent<TankSight>();
        if (tankSight.enemyInSight == true)
        {
            target = tankSight.enemy;
            //Debug.Log("This tank " + this + " enemy tank" + target);
            //if (target.GetComponent<BasicTank>() != null)
            //{
            BasicTank enemyScript = target.GetComponent<BasicTank>();
            enemyHitPoints = enemyScript.hitPoints;
            //}
            //else
            //{
            //    return;
            //  }
            rangeToTarget = Vector3.Distance(this.transform.position, target.transform.position);
            
            if (rangeToTarget >= tankRange)
            {
                //Vector3 endPoint = (rangeToTarget - (rangeToTarget - tankRange)) * Vector3.Normalize(target.transform.position - this.transform.position) + this.transform.position;
                //Vector3 endPoint = Vector3.Lerp(this.transform.position, target.transform.position, ratio);
                //Debug.Log(this.name + midWay + endPoint + FindRangeToTarget());
                agent.destination = FindRangeToTarget() + ((target.transform.position - this.transform.position).normalized *  (Random.Range(20, 30))) ;//endPoint;
                //Debug.Log(rangeToTarget );
            }
            else
            {
                //agent.Stop();
                //shouldStopMoving = true;
                RotateTurret();
                //turret.transform.rotation = target.transform.position.x;
            }

            if (Time.timeSinceLevelLoad > (prevFiringTime + (tankLoadTime + Random.Range(0.1f, 1f))))
            {
                Debug.Log("firing" + Time.timeSinceLevelLoad);
                
                prevFiringTime = Time.timeSinceLevelLoad;
                float checkIfHit = Random.Range(1, 101);
                if (checkIfHit < hitChance)
                {
                    /*GameObject tankShotExp =*/ Instantiate(tankShot, muzzel.transform.position, muzzel.transform.rotation);
                    //GameObject shella = Instantiate(shell, muzzel.transform.position, Quaternion.identity) as GameObject;
					//Vector3 shellDirection = (target.transform.position - muzzel.transform.position).normalized;
					//shella.transform.LookAt (target.transform, Vector3.up);
                    //shella.GetComponent<ShellScript>().targetPosition = target.transform.position;
                    Debug.Log("Tank " + this + " firing on "+ target);
                    float tempArmor = (10* enemyScript.armorPoints)/100;
                    enemyScript.hitPoints = enemyScript.hitPoints - (damage + Random.Range(0, (10*damage)/100) - (enemyScript.armorPoints + Random.Range(0, tempArmor))); //enemyHitPoints - 1;
                    if (target.GetComponent<BasicTank>().hitPoints <= 0)
                    {
                        //target.GetComponent<Renderer>().enabled = false;
                        //target.transform.position = new Vector3(0, 6, 0);
                        enemyScript.BeenKilled();
                        currentXP += enemyScript.XPWorth;

                        //Destroy(target.gameObject);
                        //target.GetComponent<BasicTank>().beenKilledTime = Time.timeSinceLevelLoad;
                        //target.GetComponent<BasicTank>().beenkilled = true;
                        target = null;
                        shouldStopMoving = false;
                    }
                    Debug.Log("Hit " + enemyHitPoints);
                }
                else
                {
                    //GameObject shella = Instantiate(shell, muzzel.transform.position, Quaternion.identity) as GameObject;
                    //shella.GetComponent<ShellScript>().targetPosition = new Vector3(Random.Range(target.transform.position.x, (target.transform.position.x + 1)), target.transform.position.y, Random.Range(target.transform.position.z, (target.transform.position.z + 1)));
                }
            }

            if (tankSight.enemyInSight == false)
            {
                if (Game.instance.controlPoints.Length > 0)
                {
                    IdentifyCP();
                }
            }
            //}
            // else
            //  {
            //     return;
            //  }
        }
        }

    public void BeenKilled()
    {

        if ((hitPoints <= 0) && (beenkilled != true))
        {
            beenkilled = true;
            agent.enabled = false;
            agent.SetDestination(this.transform.position);
            agent.Stop();
            //this.gameObject.GetComponent<Renderer>().enabled = false;
            this.transform.position = this.transform.position + Vector3.up * -100f;
            beenKilledTime = Time.timeSinceLevelLoad;
            switch (identity)
            {
                case ItemIdentity.Soviet:
                    Game.instance.numberOfEnemyTanksDestroyed++;
                    Game.instance.enemyTanksInLevel--;
                    GetBonuses();
                    break;

                case ItemIdentity.American:
                    Game.instance.numberOfAmericanTanks--;
                    break;
            }
        }
        if (beenkilled == true)
        {
            if (Time.timeSinceLevelLoad > beenKilledTime + 3f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void Burning()
    {
        if (Time.timeSinceLevelLoad < burningStartTime + naplamBurnTime)
        {
            if (didBurnLastTime == false)
            {
                hitPoints = hitPoints - burningDamage;
                Debug.Log(hitPoints);
                didBurnLastTime = true;
                burnIntervalTimer = Time.timeSinceLevelLoad;
            }
            else
            {
                if (Time.timeSinceLevelLoad > burnIntervalTimer + 3f)
                {
                    didBurnLastTime = false;
                }
            }
        }
        else
        {
            burning = false;
        }
    }

    void GetBonuses ()
    {
        if (Game.instance.shouldGetAmmoFromTanks == true)
        {
            if (Game.instance.shouldGetNuke == true)
            {
                bonus = Random.Range(0, 101);
            }
            else
            {
                bonus = Random.Range(0, 100);
            }
            if (bonus == 100)
            {
                Game.instance.nukesLeft++;
                Debug.Log(Game.instance.nukesLeft);
            }
            if ((bonus < 100) && (bonus >= 95))
            {
                Game.instance.daisyCutterLeft++;
                Debug.Log(Game.instance.daisyCutterLeft);
            }

            if ((bonus < 95) && (bonus >= 85))
            {
                Game.instance.minesLeft++;
                Debug.Log(Game.instance.minesLeft);
            }
            if ((bonus < 85) && (bonus >= 70))
            {
                Game.instance.naplamLeft++;
                Debug.Log(Game.instance.naplamLeft);
            }
        }

    }

    void RotateTurret()
    {
        //Debug.Log(turret.transform.position);
        turretDirection = (target.transform.position - turret.transform.position).normalized;
        //Debug.Log("Rot" + turretDirection);
        turretLookRotation = Quaternion.LookRotation(turretDirection);
        //Quaternion tempi = Quaternion.FromToRotation(turretDirection, Vector3.up);
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, turretLookRotation, turretRotationSpeed * Time.deltaTime);
    }

    void LevelUp()
    {
        if (currentXP >= ((currentLevel*currentLevel) * levelThreshold))
        {
            currentLevel++;
            if (hitChance < 95)
            {
                hitChance = hitChance++;
            }
            hitPoints = hitPoints + ((10 * initialHitPoints) / 100);
            initialHitPoints = initialHitPoints + ((10 * initialHitPoints) / 100);
            damage = damage + ((10 * damage) / 100);
        }
    }

    //void RotateTank

    Vector3 FindRangeToTarget()
    {
        Vector3 ab = new Vector3((target.transform.position.x - this.transform.position.x), (target.transform.position.y - this.transform.position.y), (target.transform.position.z - this.transform.position.z));
        float vAB = Mathf.Sqrt((ab.x * ab.x) + (ab.y * ab.y) + (ab.z * ab.z));
        Vector3 vec = new Vector3((ab.x / vAB), (ab.y / vAB), (ab.z / vAB));
        Vector3 vT = new Vector3(vec.x * (rangeToTarget - tankRange), vec.y * (rangeToTarget - tankRange), vec.z * (rangeToTarget - tankRange));
        Vector3 eP = vT + this.transform.position;
        return (eP);
        /*float distanceToFire = rangeToTarget - (rangeToTarget - tankRange);
        float ratio = rangeToTarget / distanceToFire;
        float x3 = ratio * target.transform.position.x + (1 - ratio) * this.transform.position.x;
        float z3 = ratio * target.transform.position.z + (1 - ratio) * this.transform.position.z;
        Vector3 midWay = new Vector3(x3, this.transform.position.y, z3);*/
    }
}
