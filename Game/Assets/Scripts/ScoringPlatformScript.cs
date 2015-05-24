using UnityEngine;
using System.Collections;

public class ScoringPlatformScript : MonoBehaviour {
    private TeamInfo _teamOwningZepelin;
    public GameObject TeamColoredGameObject;
    public float GiveScorePointEverySec = 10;
    private float _timeOverTaken; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (_teamOwningZepelin == null) return;
	    if ((_timeOverTaken + GiveScorePointEverySec) < Time.time)
	    {
	        _teamOwningZepelin.Score++;
	        _timeOverTaken = Time.time;
	    }
	}

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Huraaaaa koliziaaa");
        var player = collision.gameObject.GetComponent<Player>();
        if (player == null) return;
        _teamOwningZepelin = player.Team;
        TeamColoredGameObject.transform.GetComponent<Renderer>().material.color = _teamOwningZepelin.Color;
        _timeOverTaken = Time.time;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        _teamOwningZepelin = null;
        TeamColoredGameObject.transform.GetComponent<Renderer>().material.color = Color.white;
    }

}
