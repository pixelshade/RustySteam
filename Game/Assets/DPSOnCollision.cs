using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DPSOnCollision : MonoBehaviour
{
    public int Damage = 10;
    public float Seconds = 1000;

	private Dictionary<Player, float>  collidingPlayers = new Dictionary<Player, float> ();
  
   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		var temp = new Dictionary<Player, float> ();

		foreach (var t in collidingPlayers) {
//			Debug.Log("next:"+t.Value);
//			Debug.Log ("time:"+Time.time);
			if (Time.time > t.Value) {

				t.Key.HP -= Damage;
				collidingPlayers[t.Key] = Time.time + Seconds;
				var c = Time.time + Seconds;
//				temp.Add(t.Key,c);
//				Debug.Log(c);
			}
		}
//		collidingPlayers = temp;

//		Debug.Log (collidingPlayers);
	}

    void OnCollisionEnter(Collision collision)
    {
        var player = collision.transform.GetComponent<Player>();
        if (player != null)
        {
			collidingPlayers.Add(player, Time.time);

        }

    }

	void OnCollisionExit(Collision collision) {
		var player = collision.transform.GetComponent<Player>();
		if (player != null)
		{
			collidingPlayers.Remove(player);			
		}

	}
}
