
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Interface : MonoBehaviour
{
    public GameObject northBoundary;
    public GameObject southBoundary;
    public GameObject eastBoundary;
    public GameObject westBoundary;
    int numberOfTouches;
    Touch firstTouch;
    Touch secondTouch;
    Vector2 touchStartPosition;
    Vector2 touchEndPosition;
    Vector3 firstStartPosition;
    Vector3 firstEndPosition;
    Vector3 secondStartPosition;
    Vector3 secondEndPosition;
    Vector3 movedPosition;
    Vector3 swipeDirection;
    Vector3 acceleration;
    Vector3 deltaAcceleration;
    Vector3 lowPassValue = Vector3.zero;
    float shakeCounter;
    float accelerationDetectionThreshhold = 1.0f;
    float accelerometerUpdateInterval = 1.0f / 60.0f;
    float lowPassKernelInSeconds = 1.0f;
    float lowAcceleratonFilterFactor;
    //public ItemIdentity identity;
    public float highExplosiveDamage;
    public float naplamDamage;
    public float naplamLength;
    public float mineLength;
    Vector2 mousePos;
    Vector2 mousePos2;
    Vector3 tempMousePos;
    Vector3 tempMousePos2;
    List<Vector3> firstTouchPosList = new List<Vector3>();
    List<Vector3> secondTouchPosList = new List<Vector3>();
    float firstTouchReleaseTime;
    float firstTouchStartTime;
    float secondTouchReleaseTime;
    float secondTouchStartTime;
    public float slowMotionRate;
    public float slowMotionDuration;
    public float nukeRange;
    public float nukeDamage;
    public float daisyDamage;
    public float daisyRange;
    public float swipeSensitivity;
    float slowMotionStatTime;
    bool isSlowMotionActive;
    List<Touch> touchStorage = new List<Touch>();
    public GameObject mine;
    GameObject newMine;
    GameObject newFlame;
    GameObject newExplosion;
    GameObject newNuke;
    GameObject newDaisy;
    public GameObject nukeExplosion;
    public GameObject basicExplosion;
    public GameObject basicFlame;
    public GameObject daisyExplosion;
    bool isShaking;
    float swipeCount;
    float secondSwipeCount;
    bool minesRun = false;
    bool naplamRun = false;
    public Camera camera;
    public float cameraSpeed;
    float counter;
    bool cameraNorth = false;
    bool cameraSouth = false;
    bool cameraWest = false;
    bool cameraEast = false;
    bool ispaused = false;
    bool isSettingsActive = false;
    bool isDaisyActive = false;
    bool isNukeActive = false;
    public float HERange;
    GameObject wayPoint;
    GameObject randomTank;
    public GameObject panel;
    public GameObject settings;
    public Button skip;
    public AudioSource explosionSFX;
    public AudioSource nukeSFX;
    public AudioSource NaplamSFX;
    public AudioSource mineSFX;
    bool isTracked = true;

   
    
    //public int nukeZeroRange;
    // Use this for initialization
    void Start()
    {
        lowPassValue = Input.gyro.userAcceleration;
        firstTouchReleaseTime = Time.timeSinceLevelLoad;
        secondTouchReleaseTime = Time.timeSinceLevelLoad;
        isSlowMotionActive = false;
        accelerationDetectionThreshhold *= accelerationDetectionThreshhold;
        lowAcceleratonFilterFactor = accelerometerUpdateInterval / lowPassKernelInSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        //Paused();
        CameraCenter();
        // SloMoTime();
        CameraMover();
        acceleration = Input.acceleration;
        isShaking = IsShaking();
        //Debug.Log (isShaking);
        //MousePosition();
        numberOfTouches = Input.touchCount;
        if (numberOfTouches > 0)
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            /* Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
             RaycastHit hit;
             int layerMask = 1 << 5;

             if (Physics.Raycast(ray, out hit, layerMask) == true)
             {
                 Debug.Log(ray);
             }
             else
             {*/

            //{
            switch (numberOfTouches)
            {
                case 1:
                    firstTouch = Input.GetTouch(0);
                    if (firstTouch.phase == TouchPhase.Began) //&& (Time.timeSinceLevelLoad > firstTouchReleaseTime + 0.1f))
                    {
                        //Debug.Log ("TRUE");
                        // touchStartPosition = firstTouch.position;
                        firstStartPosition = TouchPositionFinder(firstTouch.position);
                        firstTouchPosList.Clear();
                        firstTouchStartTime = Time.timeSinceLevelLoad;
                        naplamRun = false;
                        //Debug.Log ("Start time" + firstStartPosition);

                    }

                    if (firstTouch.phase == TouchPhase.Ended)
                    {
                        //touchEndPosition = firstTouch.position;
                        firstEndPosition = TouchPositionFinder(firstTouch.position);
                        firstTouchReleaseTime = Time.timeSinceLevelLoad;



                        //Debug.Log (firstEndPosition);



                        int firstStartPositionX = (int)firstStartPosition.x;
                        int firstStartPositionZ = (int)firstStartPosition.z;
                        int endstStartPositionX = (int)firstEndPosition.x;
                        int endstStartPositionZ = (int)firstEndPosition.z;
                        if ((((firstStartPositionX >= endstStartPositionX - 1) && (firstStartPositionX <= endstStartPositionX)) || ((firstStartPositionX <= endstStartPositionX + 1) && (firstStartPositionX >= endstStartPositionX))) && (((firstStartPositionZ >= endstStartPositionZ - 1) && (firstStartPositionZ <= endstStartPositionZ)) || ((firstStartPositionZ <= endstStartPositionZ + 1) && (firstStartPositionZ >= endstStartPositionZ))) /*(firstStartPositionZ == endstStartPositionZ)*/ && (firstTouchReleaseTime - firstTouchStartTime < 1f))
                        {
                            Debug.Log("High Explosive");
                            HighExplosive();
                            firstTouchPosList.Clear();

                            //firstTouchStartTime = 0f;
                            //firstTouchReleaseTime = 0f;
                        }
                        naplamRun = false;
                        isDaisyActive = false;
                        isNukeActive = false;
                    }
                    firstTouchPosList.Add(TouchPositionFinder(firstTouch.position));
                    //Debug.Log("Sqrmag " + firstTouch.deltaPosition.sqrMagnitude );
                    if ((firstTouch.phase == TouchPhase.Moved) && (naplamRun == false) && (Game.instance.naplamLeft > 0))
                    {
                        if (firstTouch.deltaPosition.sqrMagnitude > swipeSensitivity) // was 1
                        {
                            Debug.Log(firstTouch.deltaPosition.sqrMagnitude);
                            firstTouchPosList.Add(TouchPositionFinder(firstTouch.position));
                        }

                        if (firstTouchPosList.Count > 1)
                        {
                            swipeCount = 0;
                            for (int i = 1; i < firstTouchPosList.Count; i++)
                            {
                                if ((((firstTouchPosList[i].x >= firstTouchPosList[i - 1].x - 0.5f) && (firstTouchPosList[i].x <= firstTouchPosList[i - 1].x)) || ((firstTouchPosList[i].x <= firstTouchPosList[i - 1].x + 0.5f) && (firstTouchPosList[i].x >= firstTouchPosList[i - 1].x))) || (((firstTouchPosList[i].z >= firstTouchPosList[i - 1].z - 0.5f) && (firstTouchPosList[i].z <= firstTouchPosList[i - 1].z)) || ((firstTouchPosList[i].z <= firstTouchPosList[i - 1].z + 0.5f) && (firstTouchPosList[i].z >= firstTouchPosList[i - 1].z))))
                                {
                                    //swipeCount++;
                                    NaplamStrike();
                                }
                            }
                            //if (swipeCount > 1)
                            //{
                            //int counter = (firstTouchPosList.Count) - 1;
                            //movedPosition = TouchPositionFinder(firstTouchPosList[counter]);
                            //NaplamStrike();

                            //}
                        }
                        //movedPosition = TouchPositionFinder(firstTouch.position);
                        //swipeDirection = movedPosition - startPosition;
                        //NaplamStrike();
                        //swipeDirection = TouchPositionFinder(firstTouch.deltaPosition); - CHECK AT HOME IF WORKS 

                    }

                    if ((firstTouch.phase == TouchPhase.Stationary) && (Time.timeSinceLevelLoad > firstTouchStartTime + 2f) && (Input.touchCount == 1))
                    {

                        Ray ray = Camera.main.ScreenPointToRay(firstTouch.position);
                        RaycastHit hit;
                        int layerMask = 1 << 5;

                        if (Physics.Raycast(ray, out hit, layerMask) == true)
                        {
                            Debug.Log(ray);
                        }
                        else
                        {
                            DaisyCutter();
                        }

                    }





                    if ((isShaking == true) && (shakeCounter > 4) && (Game.instance.nukesLeft > 0) && (isNukeActive == true))
                    {

                        Nuke();
                    }
                    break;

                case 2:
                    firstTouch = Input.GetTouch(0);
                    secondTouch = Input.GetTouch(1);
                    if ((firstTouch.phase == TouchPhase.Began) && (Time.timeSinceLevelLoad > firstTouchReleaseTime + 0.5f))
                    {
                        // touchStartPosition = firstTouch.position;
                        firstStartPosition = TouchPositionFinder(firstTouch.position);
                        firstTouchPosList.Clear();
                        firstTouchStartTime = Time.timeSinceLevelLoad;

                    }

                    if ((secondTouch.phase == TouchPhase.Began) && (Time.timeSinceLevelLoad > secondTouchReleaseTime + 0.5f))
                    {
                        // touchStartPosition = firstTouch.position;
                        secondStartPosition = TouchPositionFinder(firstTouch.position);
                        secondTouchPosList.Clear();
                        secondTouchStartTime = Time.timeSinceLevelLoad;
                        minesRun = false;

                    }

                    if (firstTouch.phase == TouchPhase.Ended)
                    {
                        //touchEndPosition = firstTouch.position;
                        firstEndPosition = TouchPositionFinder(firstTouch.position);
                        firstTouchReleaseTime = Time.timeSinceLevelLoad;
                        minesRun = false;

                    }

                    if (secondTouch.phase == TouchPhase.Ended)
                    {
                        //touchEndPosition = firstTouch.position;
                        secondEndPosition = TouchPositionFinder(firstTouch.position);
                        secondTouchReleaseTime = Time.timeSinceLevelLoad;

                    }
                    firstTouchPosList.Add(TouchPositionFinder(firstTouch.position));
                    secondTouchPosList.Add(TouchPositionFinder(secondTouch.position));

                    if ((secondTouch.phase == TouchPhase.Moved) && (firstTouch.phase == TouchPhase.Stationary) && (minesRun == false) && (Game.instance.minesLeft > 0))
                    {

                        if (secondTouch.deltaPosition.sqrMagnitude > swipeSensitivity)
                        {

                            secondTouchPosList.Add(TouchPositionFinder(secondTouch.position));
                        }

                        if (secondTouchPosList.Count > 2)
                        {
                            secondSwipeCount = 0;
                            for (int i = 1; i < secondTouchPosList.Count; i++)
                            {
                                if ((secondTouchPosList[i].x == secondTouchPosList[i - 1].x) || (secondTouchPosList[i].z == secondTouchPosList[i - 1].z))
                                {
                                    secondSwipeCount++;
                                }
                            }
                            if (secondSwipeCount > 1)
                            {
                                //int counter = (firstTouchPosList.Count) - 1;
                                //movedPosition = TouchPositionFinder(firstTouchPosList[counter]);
                                Mines();

                            }
                        }
                    }
                    /*if (secondTouchPosList.Count > 3)
                        {
                            for (int i = 1; i < secondTouchPosList.Count; i++)
                            {
                                if ((secondTouchPosList[i].x == secondTouchPosList[i - 1].x) || (secondTouchPosList[i].z == secondTouchPosList[i - 1].z))
                                {
                                    Mines();
                                }
                            }
                        }
                        //movedPosition = TouchPositionFinder(firstTouch.position);
                        //swipeDirection = movedPosition - startPosition;
                        //NaplamStrike();
                        //swipeDirection = TouchPositionFinder(firstTouch.deltaPosition); - CHECK AT HOME IF WORKS 

                    }*/

                    break;
            }
            //  }

        }
    }

    void HighExplosive()
    {
        Vector3 explosionLoc = TouchPositionFinder(firstTouch.position);
        newExplosion = Instantiate(basicExplosion, explosionLoc, Quaternion.identity) as GameObject;
        explosionSFX.Play();
        int layerMask = 1 << 10;
        Collider[] hits = Physics.OverlapSphere(firstStartPosition, HERange, layerMask);
        //Debug.Log("HE");
        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];
            //Debug.Log(hit.collider);
            //if (hit.collider != null) 
            //{ //&& (hit.rigidbody!= null))
            //Debug.Log("Hit by Nuke");

            GameObject hitObject = hit.gameObject;
            if (hitObject.GetComponent<BasicTank>() != null)
            {

                //Debug.Log("HIT");

                if (hitObject.GetComponent<BasicTank>().identity == ItemIdentity.Soviet)//identity)
                {
                    float rangeToTarget = Vector3.Distance(firstStartPosition, hitObject.transform.position);
                    //hitObject.GetComponent<BasicTank>().burning = true;
                    //hitObject.GetComponent<BasicTank>().didBurnLastTime = false;
                    if (rangeToTarget <= 7f)
                    {
                        Debug.Log(hitObject + " " + rangeToTarget);
                        hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - highExplosiveDamage;
                    }
                    //GameObject detectedObject = hit.collider.gameObject;
                    //if (hitObject.GetComponent<BasicTank>() != null)
                    //{

                    //if (hitObject.GetComponent<BasicTank>().identity == identity)
                    //{

                    //}
                    //};

                    else
                    {
                        hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - (highExplosiveDamage / rangeToTarget);
                    }
                    //hitObject.GetComponent<BasicTank>().burningStartTime = Time.timeSinceLevelLoad;
                }
            }
            //}
        }
        //Debug.log ("High Explosive");
        
        
        //Vector3 FP = TouchPositionFinder(firstTouch.position) ;

        //Ray ray = Camera.main.ScreenPointToRay(/*Input.mousePosition);*/ firstTouch.position);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
           // if (hit.collider != null)
            //{

                //if (hit.rigidbody != null)
                //{
                    
                //}
           // }
        
    }

    void DaisyCutter()
    {
        if ((Game.instance.daisyCutterLeft > 0) && (isDaisyActive == false))
        {
            isDaisyActive = true;
            Vector3 explosionLoc = TouchPositionFinder(firstTouch.position);
            nukeSFX.Play();
            newDaisy = Instantiate(daisyExplosion, explosionLoc, Quaternion.identity) as GameObject;
            int layerMask = 1 << 10;
            Collider[] hits = Physics.OverlapSphere(firstStartPosition, daisyRange, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider hit = hits[i];
                //Debug.Log(hit.collider);
                //if (hit.collider != null) 
                //{ //&& (hit.rigidbody!= null))
                Debug.Log("Hit by Daisy Cutter");

                GameObject hitObject = hit.gameObject;
                if (hitObject.GetComponent<BasicTank>() != null)
                {

                    //Debug.Log("HIT");

                    if (hitObject.GetComponent<BasicTank>().identity == ItemIdentity.Soviet)
                    {
                        float rangeToTarget = Vector3.Distance(firstStartPosition, hitObject.transform.position);
                        //hitObject.GetComponent<BasicTank>().burning = true;
                        //hitObject.GetComponent<BasicTank>().didBurnLastTime = false;
                        if (rangeToTarget <= 10f)
                        {
                            hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - daisyDamage;
                        }
                        else
                        {
                            hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - (daisyDamage - (rangeToTarget * 4.3f));
                        }
                        //hitObject.GetComponent<BasicTank>().burningStartTime = Time.timeSinceLevelLoad;
                    }
                }
                //}
            }
            Game.instance.daisyCutterLeft--;//isSlowMotionActive = true;
            //slowMotionStatTime = Time.realtimeSinceStartup;
            //Debug.Log("Slow Motion Began");
            //Time.timeScale = slowMotionRate;
            //int tempNumberOfTouches = Input.touchCount;
            //isDaisyActive = false;
        }

    }

    void NaplamStrike()
    {
        naplamRun = true;
        NaplamSFX.Play();
        movedPosition = TouchPositionFinder(/*Input.mousePosition);*/firstTouch.position);
        Debug.Log("Naplam");
        swipeDirection = movedPosition - firstStartPosition;
        Vector3 instatPos;// TouchPositionFinder(firstTouch.position);
        //Ray ray = Camera.main.ScreenPointToRay(/*Input.mousePosition);*/startPosition);
        int layerMask = 1 << 10;
        RaycastHit[] hits;

        Vector3 SP = firstStartPosition; //tempMousePos; new Vector3(startPosition.x, 0, startPosition.y);
        Debug.Log("SP " + SP);
        instatPos = SP;
        Debug.Log(SP);
        Vector3 SD = swipeDirection.normalized; //new Vector3 (swipeDirection.x, 0, swipeDirection.y) ;
        for (int i = 0; i < naplamLength; i++)
        {
            instatPos = instatPos + SD;
            Debug.Log("instatPos " + instatPos);
            newExplosion = Instantiate(basicExplosion, instatPos, Quaternion.identity) as GameObject;
            newFlame = Instantiate(basicFlame, instatPos, Quaternion.identity) as GameObject;
        }
        Debug.Log("SD " + SD);
        hits = Physics.RaycastAll(SP, SD, naplamLength, layerMask);
        Debug.DrawRay(SP, SD * naplamLength, Color.black, 20f);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            //Debug.Log(hit.collider);
            if (hit.collider != null) //&& (hit.rigidbody!= null))
            {
                //Debug.Log("Hit");
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.GetComponent<BasicTank>() != null)
                {

                    //Debug.Log("HIT");
                    if (hitObject.GetComponent<BasicTank>().identity == ItemIdentity.Soviet)
                    {
                        hitObject.GetComponent<BasicTank>().burning = true;
                        hitObject.GetComponent<BasicTank>().didBurnLastTime = false;
                        hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - naplamDamage;
                        hitObject.GetComponent<BasicTank>().burningStartTime = Time.timeSinceLevelLoad;
                    }
                }
            }
        }
        swipeDirection = Vector3.zero;
        firstTouchPosList.Clear();
        movedPosition = Vector3.zero;
        Game.instance.naplamLeft--;
        //naplamRun = false;
    }

    void Mines()
    {
        minesRun = true;
        mineSFX.Play();
        Debug.Log("MINE");
        movedPosition = TouchPositionFinder(secondTouch.position);
        //Debug.Log("Move Position" + movedPosition);

        //Ray ray = Camera.main.ScreenPointToRay(/*Input.mousePosition);*/startPosition);
        //int layerMask = 1 << 10;
        //RaycastHit[] hits;

        Vector3 instatPos = TouchPositionFinder(secondTouch.position); //tempMousePos; new Vector3(startPosition.x, 0, startPosition.y);
                                                                       //Debug.Log("SP " + instatPos);
        swipeDirection = movedPosition - secondStartPosition;
        //Debug.Log(SP);
        Vector3 SD = swipeDirection.normalized; //new Vector3 (swipeDirection.x, 0, swipeDirection.y) ;
        //Debug.Log("SD " + SD);
        for (int i = 0; i < mineLength; i++)
        {
            instatPos = instatPos + SD;
            Debug.Log("instatPos " + instatPos);
            if (i % 2 == 0)
            {
                newMine = Instantiate(mine, instatPos, Quaternion.identity) as GameObject;
            }
        }


        swipeDirection = Vector3.zero;
        secondTouchPosList.Clear();
        movedPosition = Vector3.zero;
        Game.instance.minesLeft--;


    }

    void Nuke()
    {
        isNukeActive = true;
        Debug.Log("NUKE");
        Vector3 explosionLoc = TouchPositionFinder(firstTouch.position);
        nukeSFX.Play();
        newNuke = Instantiate(nukeExplosion, explosionLoc, Quaternion.identity) as GameObject;
        int layerMask = 1 << 10;
        Collider[] hits = Physics.OverlapSphere(firstStartPosition, nukeRange, layerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider hit = hits[i];
            //Debug.Log(hit.collider);
            //if (hit.collider != null) 
            //{ //&& (hit.rigidbody!= null))
            Debug.Log("Hit by Nuke");

            GameObject hitObject = hit.gameObject;
            if (hitObject.GetComponent<BasicTank>() != null)
            {

                //Debug.Log("HIT");
                
                if (hitObject.GetComponent<BasicTank>().identity == ItemIdentity.Soviet)
                {
                    float rangeToTarget = Vector3.Distance(firstStartPosition, hitObject.transform.position);
                    //hitObject.GetComponent<BasicTank>().burning = true;
                    //hitObject.GetComponent<BasicTank>().didBurnLastTime = false;
                    if (rangeToTarget <= 30)
                    {
                        hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - nukeDamage;
                    }
                    else
                    {
                        hitObject.GetComponent<BasicTank>().hitPoints = hitObject.GetComponent<BasicTank>().hitPoints - (nukeDamage - (rangeToTarget*1.8f));
                    }
                    //hitObject.GetComponent<BasicTank>().burningStartTime = Time.timeSinceLevelLoad;
                }
            }
            //}
        }
        Game.instance.nukesLeft--;

    }

    /*Vector3 FindRangeToTarget(GameObject target, Vector3 FSP)
    {
        float rangeToTarget;
        float 
        Vector3 ab = new Vector3((target.transform.position.x - FSP.x), (target.transform.position.y - FSP.y), (target.transform.position.z - FSP.z));
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
    //}

    /*void MousePosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = TouchPositionFinder(Input.mousePosition);
            //touchPosList.Clear();
            firstTouchStartTime = firstTouchStartTime + 1;
            //Debug.Log(touchStartTime);
        }
        //Debug.Log(firstTouchStartTime);
        if (Input.GetMouseButton(0))//if  ((firstTouchStartTime > 20) && (isSlowMotionActive == false))
            {
                firstTouchPosList.Add(Input.mousePosition);
                //slowMotionStatTime = Time.realtimeSinceStartup;
                //GroupedHighExplosive();
            //firstTouchStartTime = 0;

            }
            //touchPosList.Add(TouchPositionFinder(Input.mousePosition));
            // Debug.Log(touchPosList.Count);
            /*int layerMask = 1 << 9;
            Vector3 hitWithGround;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 22, layerMask))
            {
                Debug.Log(hit.point);
                hitWithGround = hit.point;
            }
            else
            {
                hitWithGround = Vector3.zero;
            }
            
            mousePos = Input.mousePosition;
            Debug.Log("Scrren Point Mouse Position 1 " + mousePos);
            
            tempMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, (Camera.main.transform.position.y - hitWithGround.y)));
            mousePos = new Vector2(tempMousePos.x, tempMousePos.y);
            Debug.Log("world Point Mouse Position 1 " + tempMousePos);
            //HighExplosive();*/
    //}
    //touchPosList.Add(TouchPositionFinder(Input.mousePosition));
    //Debug.Log(touchPosList.Count);
    /*if (Input.GetMouseButtonUp(0))
    {
        endPosition = TouchPositionFinder(Input.mousePosition);
        firstTouchReleaseTime = Time.timeSinceLevelLoad;
        /*int layerMask = 1 << 9;
        Vector3 hitWithGround;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 22, layerMask))
        {
            Debug.Log(hit);
            hitWithGround = hit.point;
        }
        else
        {
            hitWithGround = Vector3.zero;
        }
        mousePos2 = Input.mousePosition;
        Debug.Log("Scrren Point Mouse Position 2 " + mousePos2);

        tempMousePos2 = Camera.main.ScreenToWorldPoint(new Vector3(mousePos2.x, mousePos2.y, (Camera.main.transform.position.y - hitWithGround.y)));
        Debug.Log(tempMousePos2);
        mousePos = new Vector2(tempMousePos2.x, tempMousePos2.y);
        Debug.Log("World Point Mouse Position 2 " + mousePos2);
        //mousePos2 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));*/



    /* if ((startPosition != endPosition)  && (firstTouchPosList.Count > 4))
     {
          for (int i = 1; i < firstTouchPosList.Count; i++)
         {
             if ((firstTouchPosList[i].x == firstTouchPosList[i - 1].x) || (firstTouchPosList[i].z == firstTouchPosList[i - 1].z))
                 {
                     //swipeDirection = endPosition - startPosition;
                     NaplamStrike();
                     firstTouchPosList.Clear();
                 }
         }
         //movedPosition = TouchPositionFinder(firstTouch.position);
         //swipeDirection = movedPosition - startPosition;
         //NaplamStrike();
         //swipeDirection = TouchPositionFinder(firstTouch.deltaPosition); - CHECK AT HOME IF WORKS 

     }
     /*if (startPosition != endPosition)
     //if ((mousePos.x != mousePos2.x) || (mousePos.y != mousePos2.y))
     {
         swipeDirection = endPosition - startPosition;
         //swipeDirection.y = tempMousePos.z - tempMousePos.z;
         Debug.Log("Swipe Direction " + swipeDirection);
         //Debug.DrawLine(mousePos, mousePos2);
         //Debug.Log("Naplam");
         //startPosition = mousePos;
         NaplamStrike();



     }*/
    /*}
    mousePos = Vector2.zero;
    mousePos2 = Vector2.zero;
    swipeDirection = Vector3.zero;
}*/

    Vector3 TouchPositionFinder(Vector2 touchPosition)
    {
        Vector3 outcom;
       // int layerMask = 1 << 9;
        //int layerMask2 = 1 << 10;
        Vector3 hitWithGround;
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity/*, layerMask*/))///*touchPosition, Vector3.down, out hit, Mathf.Infinity)) /*layerMask) || Physics.Raycast(touchPosition, Vector3.down, out hit, Mathf.Infinity, layerMask2)) (Physics.Raycast(ray, out hit,  22/*, layerMask*///))
        {
            //Debug.Log(hit.point);
            hitWithGround = hit.point;
            Debug.Log("HIT " + hit.collider.gameObject.name);
        }
        else
        {
            hitWithGround = Vector3.zero;
            Debug.Log("HIT " + hit.collider);
        }
        //startPosition = firstTouch.position;
        //mousePos = Input.mousePosition;
        //Debug.Log("Scrren Point Mouse Position 1 " + startPosition);

        outcom = Camera.main.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, (Camera.main.transform.position.y - hitWithGround.y)));
        //mousePos = new Vector2(tempMousePos.x, tempMousePos.y);
        //Debug.Log("Outcom " + outcom);
        return (outcom);
    }

    bool IsShaking()
    {
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowAcceleratonFilterFactor);
        deltaAcceleration = acceleration - lowPassValue;
        if (deltaAcceleration.sqrMagnitude >= accelerationDetectionThreshhold)
        {

            shakeCounter += 1;
            return true;
        }
        else
        {
            shakeCounter = 0;
            return false;

        }
    }

    /*void SloMoTime()
    {
        if (isSlowMotionActive == true)
        {

            if (Time.realtimeSinceStartup < slowMotionStatTime + slowMotionDuration)
            {
                int tempNumberOfTouches = Input.touchCount;
                if (/*Input.GetMouseButtonDown(0))tempNumberOfTouches == 1)
                {

                    firstTouch = Input.GetTouch(0);
                    if (firstTouch.phase == TouchPhase.Ended)
                    {
                        HighExplosive();
                        Debug.Log("BOOM");
                        counter++;

                    }
                }
            }
            else
            {
                isSlowMotionActive = false;
                Debug.Log("Slow Motion Ended");
                Time.timeScale = 1.0f;
                Game.instance.concentratedLeft--;
                Debug.Log(counter);
                counter = 0;
            }
           }
        }*/

    public void CameraStateChanger(int changer)
    {
        isTracked = false;
        switch (changer)
        {
            case 1: cameraNorth = !cameraNorth;
                break;

            case 2:
                cameraSouth = !cameraSouth;
                break;

            case 3:
                cameraWest = !cameraWest;
                break;

            case 4:
                cameraEast = !cameraEast;
                break;
        }
    }

    void CameraMover()
    {
        if (cameraNorth == true)
        {
            if (camera.transform.position.z < northBoundary.transform.position.z)
            {
                CameraUp();
            }
        }

        if (cameraSouth == true)
        {
            if (camera.transform.position.z > southBoundary.transform.position.z)
            {
                CameraDown();
            }
        }

        if (cameraWest == true)
        {
            if (camera.transform.position.x > westBoundary.transform.position.x)
            {
                CameraLeft();
            }
        }

        if (cameraEast == true)
        {
            if (camera.transform.position.x < eastBoundary.transform.position.x)
            {
                CameraRight();
            }
        }
    }

     void CameraUp()
    {
        camera.transform.position = camera.transform.position + (Vector3.forward * cameraSpeed * Time.deltaTime);
        //isTracked = false;
    }

     void CameraDown()
    {
        camera.transform.position = camera.transform.position + (Vector3.forward * -cameraSpeed * Time.deltaTime);
        //isTracked = false;
    }

     void CameraLeft()
    {
        camera.transform.position = camera.transform.position + (Vector3.right * -cameraSpeed * Time.deltaTime);
        //isTracked = false;
    }

     void CameraRight()
    {
        camera.transform.position = camera.transform.position + (Vector3.right * cameraSpeed * Time.deltaTime);
        //isTracked = false;
    }

    void CameraCenter()
    {

        if (isTracked == true)
        {
            // if ((camera.transform.position.z > southBoundary.transform.position.z) && (camera.transform.position.z < northBoundary.transform.position.z) && (camera.transform.position.x > westBoundary.transform.position.x) && (camera.transform.position.x < eastBoundary.transform.position.x))
            // {
            if (randomTank != null)
            {
                if ((randomTank.transform.position.z > southBoundary.transform.position.z) && (randomTank.transform.position.z < northBoundary.transform.position.z) && (randomTank.transform.position.x > westBoundary.transform.position.x) && (randomTank.transform.position.x < eastBoundary.transform.position.x))
                {
                    camera.transform.position = new Vector3(randomTank.transform.position.x, camera.transform.position.y, randomTank.transform.position.z);
                }

            }
            else
            {
                randomTank = GameObject.FindGameObjectWithTag("Player");
                //isTracked = true;
            }
                
           // }
        }
    }

   // void 

    public void CameraCenterButton()
    {
        isTracked = true;//!isTracked;
        //CameraCenter();
        randomTank = GameObject.FindGameObjectWithTag("Player");
    }

    public void PauseButton()
    {
        Debug.Log("Pause");
        ispaused = !ispaused;
        Paused();
    }

    void Paused()
    {
        if (ispaused == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public void QuiteButton()
    {
        Debug.Log("Quit");
        ClearLevel();
        Application.LoadLevel("Main_Menu");
    }

    public void NextButton()
    {
        if (Game.instance.WPActive != 0)
        {
            if (Game.instance.WPActive == -1)
            {
                Game.instance.NextText(Game.instance.story);
            }
            else
            {
                FindActiveWP();
                /*for (int i = 0; i < Game.instance.wayPoints.Length; i++)
                {
                    if (Game.instance.wayPoints[i].GetComponent<WP>().wayPointNum == Game.instance.WPActive)
                    {
                        wayPoint = Game.instance.wayPoints[i];
                    }
                }*/
            }
        }
        else
        {
            wayPoint.GetComponent<WP>().NextText();
        }
    }

    public void SkipButton()
    {
        if (Game.instance.WPActive > 0)
        {
            FindActiveWP();
            wayPoint.GetComponent<WP>().allTextRead = true;
            Debug.Log(wayPoint.GetComponent<WP>().allTextRead);
        }
        Game.instance.WPActive = 0;
        panel.SetActive(false);
        
        
        Time.timeScale = 1;
        skip.gameObject.SetActive(false);
    }

    public void EndLevelButton()
    {
        if (Application.loadedLevelName == "Tutorial")
        {
            /*Score.instance.Destructor();
            GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < tanks.Length; i++)
            {
                Destroy(tanks[i].gameObject);
            }
            */
            ClearLevel();
            if (Game.instance.won == true)
            {
                Application.LoadLevel(Game.instance.nextLevelVictory);
            }
            else
            {
                Application.LoadLevel(Game.instance.nextLevelDefeat);
            }
        }
        else
        {
            // DontDestroyOnLoad(Score.instance.gameObject);
            Application.LoadLevel("End_Screen");
        }
    }

    public void SettingsButton()
    {
        isSettingsActive = !isSettingsActive;
        settings.SetActive(isSettingsActive);
    }

    public void FindActiveWP()
    {
        /* if (Game.instance.WPActive != 0)
         {
             if (Game.instance.WPActive == -1)
             {
                 Game.instance.NextText(Game.instance.story);
             }
             else
             {*/
        Game.instance.UpdateWayPoints();
        for (int i = 0; i < Game.instance.wayPoints.Length; i++)
        {
            if (Game.instance.wayPoints[i].GetComponent<WP>().wayPointNum == Game.instance.WPActive)
            {
                wayPoint = Game.instance.wayPoints[i];
            }
        }
           // }
        //}
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
    //}
    //}
}
