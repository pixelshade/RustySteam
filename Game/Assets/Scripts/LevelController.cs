using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour, NetworkManager.ILoadFinish
{
    private const int NUM_CUBES = 10;
    private List<GameObject> cubes = new List<GameObject>();
    private NetworkManager _networkManager;
    private Rigidbody _rb;
	// Use this for initialization
	void Start () {
	    if (Consts.IsHost)
	    {
	        InitWorld();
	    }
	    SpawnPlayer();

	}


    public void SpawnPlayer()
    {
        var player = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
//        var pos = Random.insideUnitSphere*100;
         Vector3 pos = Consts.IsHost ? new Vector3(0, 5, 0) : new Vector3(50, 5, 50);
//        pos.y = 5;
        
        GameObject p = Network.Instantiate(player, pos, Quaternion.identity, 0) as GameObject;
        _rb = p.GetComponent<Rigidbody>();
//        Debug.Log("[NETWORK]playerName has joined the game.");
//        playerCount += 1;
    }

	void OnGUI(){
		GUI.Label(new Rect(120,0,200,100),"velocity "+_rb.velocity+"\n pos: "+_rb.position);
	}


    public void InitWorld()
    {
        _networkManager = NetworkManager.Get();
        if (_networkManager != null)
        {
            _networkManager.LoadFinish.Add(this);
        }
        else
        {
            Debug.LogWarning("NetworkManager is null! - Debugging enviroment");
        }

        var cube = Resources.Load("Prefabs/Cube", typeof(GameObject)) as GameObject;
        for (var i = 0; i < NUM_CUBES; i++)
        {

            var point = Random.insideUnitSphere * 500;
            point.y = Math.Abs(point.y);

            GameObject c;
//            if (Consts.IsSinglePlayer)
//            {
//                c = Instantiate(cube, point, Quaternion.identity) as GameObject;
//            }
//            else
//            {
             
                c = Network.Instantiate(cube, point, Quaternion.identity, 0) as GameObject;
//            }
            c.transform.localScale += new Vector3(Random.value, Random.value, Random.value) * 10;
            c.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);

            // We will create some stable blocks 
            if (Random.value > 0.5)
            {
                c.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f);
                c.GetComponent<Rigidbody>().isKinematic = true;
                c.transform.localScale += c.transform.localScale * 10;
                c.transform.position = new Vector3(c.transform.position.x, 0, c.transform.position.z);
                Destroy(c.GetComponent<Movable>());
            }
            cubes.Add(c);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    public void LoadFinished()
    {
    }

    public void AllLoadFinished()
    {
    }
}
