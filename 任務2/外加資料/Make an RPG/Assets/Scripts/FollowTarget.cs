using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public Transform target;
    public Vector3 offset;
    public float followTime;
    public bool lookAtTarget;
    public float stopAtRange;
    public Vector3 axisNulifier = Vector3.one;

    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null) return;
        if (lookAtTarget)
        {
            transform.LookAt(new Vector3(target.position.x, transform.position.y, transform.position.z));
        }
        if (stopAtRange != 0 && Vector3.Distance(transform.position, target.position) < stopAtRange) return;
        iTween.MoveUpdate(this.gameObject, iTween.Hash("position", Math3D.SideMultiplication(target.position, axisNulifier) + offset, "time", followTime, "easetype", iTween.EaseType.easeInOutSine));

    }
}
