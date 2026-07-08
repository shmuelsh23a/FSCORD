using UnityEngine;
using System.Collections;

public class FOWScript : MonoBehaviour
{
    /*bool isOn = true;
    public Renderer rend;
    float fowRange = 90f;*/
    // Use this for initialization
    void Start()

    {
        // rend = this.gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //TurnFOWOff();
        //TurnFOW(isOn);
    }
}

   /* void OnTriggerEnter(Collider other)
    {
        /*int layerMask;
        layerMask = 1 << 8;
        Collider[] hits = Physics.OverlapSphere(this.transform.position, fowRange, layerMask);
        for (int i = 0; i < hits.Length; i++)
        {

            Collider hit = hits[i];
            //Debug.Log(hit.collider);
            //if (hit.collider != null) 
            //{ //&& (hit.rigidbody!= null))
            //Debug.Log("Hit by Nuke");

            GameObject hitObject = hit.gameObject;
            if (hitObject.GetComponentInParent<BasicTank>() != null)
            {*/

        //Debug.Log("HIT");

        /* if (hitObject.GetComponentInParent<BasicTank>().identity == ItemIdentity.American)
         {
             isOn = false;
         }*/
       /* if (isOn == true)
        {
            Debug.Log(other.gameObject.name);
            if (other.gameObject.GetComponentInParent<BasicTank>() != null)
            {
                if (other.gameObject.GetComponentInParent<BasicTank>().identity == ItemIdentity.American)
                {
                    isOn = false;
                    //rend.enabled = isOn;
                    Debug.Log(isOn);
                    //TurnFOW(isOn);
                }
            }

        }
    }
    //  }
    //}

    void OnTriggerExit(Collider other)
    //void TurnFOWOff()
    {

        if (isOn == false)
        {
            /*int layerMask;
            layerMask = 1 << 8;
            Collider[] hits = Physics.OverlapSphere(this.transform.position, fowRange, layerMask);
            Debug.Log(hits.Length);
            /*for (int i = 0; i < hits.Length; i++)
            {*/

            //Collider hit = hits[i];
            // Debug.Log(hits.Length);
            //Debug.Log(hit.collider);
            //if (hit.collider != null) 
            //{ //&& (hit.rigidbody!= null))
            //Debug.Log("Hit by Nuke");
            /* if (hits.Length > 0)
             {
                 isOn = false;
             }
             else
             {
                 isOn = true;
             }*/
            /*GameObject hitObject = hit.gameObject;
            if (hitObject.GetComponentInParent<BasicTank>() != null)
            {

                //Debug.Log("HIT");

                if (hitObject.GetComponentInParent<BasicTank>().identity == ItemIdentity.American)
                {
                    isOn = false;
                }
                else
                {
                    isOn = false;
                }*/
           /* Debug.Log(other.gameObject.name);
            if (other.gameObject.GetComponentInParent<BasicTank>() != null)
            {

                if (other.gameObject.GetComponentInParent<BasicTank>().identity == ItemIdentity.American)
                {
                    if (other.gameObject.GetComponentInParent<BasicTank>().beenkilled != true)
                    {
                        isOn = true;
                        Debug.Log(isOn);
                    }
                    //TurnFOW(isOn);
                }
            }

        }
    }
    //}

   // }

    void TurnFOW (bool switcher)
    {
        rend.enabled = switcher;
    }
}*/
