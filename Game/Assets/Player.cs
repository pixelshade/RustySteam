using System;
using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public int HP = 100;
    


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("sila" + collision.relativeVelocity.magnitude);
//        Debug.Log("moja" + GetComponent<Rigidbody>().velocity.magnitude);
        if(collision.relativeVelocity.magnitude > 50)
        HP -= 10;

    }
}
