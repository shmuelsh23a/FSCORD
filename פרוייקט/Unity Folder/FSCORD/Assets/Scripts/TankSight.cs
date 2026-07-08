using UnityEngine;
using System.Collections;

public class TankSight : MonoBehaviour {
    public ItemIdentity identity;
    public ItemType type;
    public float fieldOfViewAngle;
    public bool enemyInSight;
    public Vector3 enemyLocation;
    SphereCollider coll;
    public GameObject enemy;
    Vector3 previousSighting;
    NavMeshAgent agent;
    public GameObject turret;
    public GameObject hull;
    Vector3 turretDirection;
    Quaternion turretStartRotation;
    Quaternion turretLookRotation;
    float turretRotationSpeed;
    bool rotate;
    bool isThereTarget;
	public GameObject forward;
    float reChack;
    public float tankSightRange;
    float prevTargetRange;
    bool rechackNow;
    /*Vector3[] vertices;
    Color[] colors;
    public GameObject fogOfWarTile;*/
    //bool didColorChange;

    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        coll = GetComponentInChildren<SphereCollider>();
    }
        // Use this for initialization
        void Start () {

        agent = this.GetComponent<NavMeshAgent>();
        coll = GetComponentInChildren<SphereCollider>();
        turretRotationSpeed = this.GetComponent<BasicTank>().turretRotationSpeed;
        turretStartRotation = this.transform.rotation;
        reChack = Time.timeSinceLevelLoad;
       /* fogOfWarTile = GameObject.FindGameObjectWithTag("FOW");
        vertices = fogOfWarTile.GetComponent<MeshFilter>().mesh.vertices;
        colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Color.black;
        }*/


    }
	
	// Update is called once per frame
	void Update () {
       //VertFinder();
        //RotateTurret();
        turretStartRotation = this.transform.rotation;
        if ((turret.transform.rotation != turretStartRotation) && (enemy == null))
        {
            RotateTurret();
        }

        if (enemy == null)
        {
            rotate = true;
            enemyInSight = false;
            enemyLocation = Vector3.zero;
            //enemy = null;
            isThereTarget = false;
            //Debug.Log(enemyInSight);
            if ((agent.enabled == true)&& (identity == ItemIdentity.Soviet))
            {
                agent.Resume();
            }
        }
        else
        {
            TargetInSight();
        }
	 
	}

    void OnTriggerEnter(Collider other)
    {
        AcquireTarget();
        TargetInSight();
        /*if (identity == ItemIdentity.American)
        {
            FOWTurnOFF(other);
        }*/
    }

    void OnTriggerStay(Collider other)
    {
         if (identity == ItemIdentity.American)
       {
           FOWTurnOFF(other);
       }
        AcquireTarget();
      
        
    }

    void AcquireTarget()
    {
        int layerMask;
        //for (int i = 0; i < others.Length; i++)
        //{
        // Collider other = others[i];
        if (isThereTarget == false)
        {
            if (identity == ItemIdentity.American)
            {
                layerMask = 1 << 10;
            }
            else
            {
                layerMask = 1 << 8;
            }
            Collider[] hits = Physics.OverlapSphere(this.transform.position, tankSightRange, layerMask);
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

                    if (hitObject.GetComponent<BasicTank>().identity != identity)
                    {
                        float rangeToTarget = Vector3.Distance(this.transform.position, hitObject.transform.position);
                        if (i == 0)
                        {
                            prevTargetRange = rangeToTarget;
                            enemy = hitObject;
                        }
                        //hitObject.GetComponent<BasicTank>().burning = true;
                        //hitObject.GetComponent<BasicTank>().didBurnLastTime = false;
                        if (rangeToTarget > prevTargetRange)
                        {
                            enemy = hitObject;
                        }
                    }
                }
            }
            /*             if (other.GetComponent<BasicTank>() != null)
         {
             //Debug.Log(other);
             if ((other.GetComponent<BasicTank>().identity != identity) && (other.GetComponent<BasicTank>().type == type))
             {
                 //enemyInSight = false;
                 enemy = other.gameObject;*/
        isThereTarget = true;
            reChack = Time.timeSinceLevelLoad;
            rechackNow = true;
            /*if (agent.enabled == true)
            {
                agent.Resume();
            }*/
            // }
            // }
        }
    }

    void TargetInSight()
    {
        //Debug.Log(enemy);
        int layerMask;
        //LayerMask mask;
        if (identity == ItemIdentity.American)
        {
            layerMask = 1 << 10;
           // mask = 10;
        }
        else
        {
            layerMask = 1 << 8;
            //mask = 8;
        }
        if ((isThereTarget == true) && (enemy != null) && ((Time.timeSinceLevelLoad >= reChack + 5f) || (rechackNow == true)))
        {
            Debug.Log("Rechack" + " " + this.gameObject.name);
            rechackNow = false;
            Vector3 direction = enemy.transform.position - this.transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            //if (angle < fieldOfViewAngle * 0.5f)
            //{
                RaycastHit hit;
                //Debug.Log(enemy);
                if (Physics.Raycast(this.transform.position /*+ transform.up*/, direction.normalized, out hit, tankSightRange, layerMask)) //coll.radius))
                {
                    Debug.Log(hit.collider.gameObject);
                    if (hit.collider.gameObject == enemy)
                    {
                        enemyInSight = true;
                        enemyLocation = enemy.transform.position;
                        
                    }
                    else
                    {
                       // AcquireTarget();
                    }
                }
            //}
            reChack = Time.timeSinceLevelLoad;
        }
    }

    //}


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == enemy)
        {
            rotate = true;
            enemyInSight = false;
            enemyLocation = Vector3.zero;
            enemy = null;
            isThereTarget = false;
            //Debug.Log(enemyInSight);

        }
        else
        {
            if (identity == ItemIdentity.American)
            {
                if (other.gameObject.GetComponent<FOWScript>() != null)
                {
                    if (this.gameObject.GetComponent<BasicTank>().beenkilled != true)
                    {
                        //other.GetComponent<Renderer>().enabled = true;
                        other.gameObject.GetComponent<Renderer>().material.color = Color.black;
                    }
                }
            }
        }
    
            
    }

    void RotateTurret()
    {
        turretDirection = (forward.transform.position - turret.transform.position  ).normalized;
		turretLookRotation = Quaternion.LookRotation(turretDirection);
		//turretLookRotation = forward.transform.rotation;
		turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, turretLookRotation, turretRotationSpeed * Time.deltaTime);
    }

    void FOWTurnOFF(Collider coll)
    {

        if (coll.gameObject.GetComponent<FOWScript>() != null)
        {
            //if (coll.gameObject.GetComponent<Renderer>().enabled == true)
            // {
            //coll.gameObject.GetComponent<Renderer>().enabled = false;
            if (coll.gameObject.GetComponent<Renderer>().material.color == Color.black)
            {
                coll.gameObject.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f);
            }
            //didColorChange = true;
            // }
        }
        /*int layerMask;
       
        layerMask = 1 << 2;
        
        Collider[] hits = Physics.OverlapSphere(this.transform.position, tankSightRange, layerMask);
        for (int i = 0; i < hits.Length; i++)
        {

            Collider hit = hits[i];
            

            GameObject hitObject = hit.gameObject;
            if (hitObject.GetComponent<FOWScript>() != null)
            {
                hitObject.GetComponent<Renderer>().enabled = false;
            }
        }*/
    }

    /*void VertFinder()
    {
        for (int i = 0; i<vertices.Length; i++)
        {
            if (Vector3.Distance(this.transform.position, vertices[i]) < 100)
            {
                colors[i] = new Color(0f, 0f, 0f, 0f);
                Debug.Log(colors[i]);

            }
            else
            {
                colors[i] = Color.black;
            }
        }
        fogOfWarTile.GetComponent<MeshFilter>().mesh.colors = colors;
    }*/
}
