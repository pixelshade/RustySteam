using UnityEngine;
using System.Collections;

public class DPSOnCollision : MonoBehaviour
{
    public int Damage = 10;
    public float Seconds = 1000;

    private float _nextTimeDmg;
   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnCollisionEnter(Collision collision)
    {
        var player = collision.transform.GetComponent<Player>();
        if (player != null)
        {
            if (Time.time > _nextTimeDmg)
            {
                player.HP -= Damage;
                _nextTimeDmg = Time.time + Seconds;
            }

        }

    }
}
