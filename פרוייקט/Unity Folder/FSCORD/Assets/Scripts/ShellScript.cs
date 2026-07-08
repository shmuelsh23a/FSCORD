using UnityEngine;
using System.Collections;

public class ShellScript : MonoBehaviour {
    Vector3 startPosition;
    public Vector3 targetPosition;
    float startTime;
    public float duration; 
	Quaternion shellLookRotation;
	// Use this for initialization
	void Start () {
        startTime = Time.timeSinceLevelLoad;
        startPosition = this.transform.position;

		Vector3 shellDirection = (transform.position - targetPosition).normalized;
		Debug.Log (shellDirection);
		//float angle = Mathf.Atan2 (shellDirection.y, shellDirection.x) * Mathf.Rad2Deg;
		shellLookRotation = Quaternion.FromToRotation(shellDirection, Vector3.up);
		//turretLookRotation = forward.transform.rotation;
		transform.eulerAngles = new Vector3 (90, 0, 0);
		this.transform.rotation = shellLookRotation;
	}
	
	// Update is called once per frame
	void Update () {
		//this.transform.rotation = Quaternion.Slerp (this.transform.rotation, shellLookRotation, 10f * Time.deltaTime);

        transform.position = Vector3.Lerp(startPosition, targetPosition, (Time.timeSinceLevelLoad - startTime) / duration);
        if (transform.position == targetPosition)
        {
            GameObject.Destroy(gameObject);
        } 
	}
}
