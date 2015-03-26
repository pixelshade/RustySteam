using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DPSOnCollision : MonoBehaviour
{
    public int Damage = 10;
    public float Seconds = 1000;

    private List<Player> collidingPlayers = new List<Player>();
    private List<float> times = new List<float>();
  
   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    for (int i = 0; i < collidingPlayers.Count; i++)
	    {
	        if (Time.time > times[i])
	        {
                collidingPlayers[i].HP -= Damage;
                times[i] = Time.time + Seconds;
	        }
	    }
	}

    void OnCollisionEnter(Collision collision)
    {
        var player = collision.transform.GetComponent<Player>();
        if (player != null && !collidingPlayers.Contains(player))
        {
			collidingPlayers.Add(player);
            Debug.Log(player.name);
            times.Add(Time.time);

        }

    }

	void OnCollisionExit(Collision collision) {
		var player = collision.transform.GetComponent<Player>();
		if (player != null)
		{
            var pos = collidingPlayers.IndexOf(player);
		    if (pos < 0) return;
		    collidingPlayers.RemoveAt(pos);
		    times.RemoveAt(pos);
		}

	}
}
