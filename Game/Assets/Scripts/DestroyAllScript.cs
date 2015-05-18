using UnityEngine;
using System.Collections;
using System;

public class DestroyAllScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamage(1000);
        }
        else
        {
            var trans = other.GetComponent<Transform>();
            if (trans.tag == "movable")
            {
                var point = UnityEngine.Random.insideUnitSphere * 150;
                point.y = Math.Abs(point.y);
                trans.position = point;
            }
        }
    }
}
