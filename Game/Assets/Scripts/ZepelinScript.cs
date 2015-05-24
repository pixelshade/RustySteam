using UnityEngine;
using System.Collections;

public class ZepelinScript : MonoBehaviour
{
   

   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (!Consts.IsHost) return;
        transform.Translate(Vector3.up/20);
        transform.Rotate(0,0,0.1f);
    }

    
   
}
