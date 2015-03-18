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



    private Consts.GameModes _gameMode;
    
	// Use this for initialization
	void Start () {
	    if (Consts.IsHost)
	    {
	        InitWorld();
	    }
        SpawnPlayer(PlayerPrefs.GetString("NickName"));

	}


    public void SpawnPlayer(string name)
    {
        var player = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
//        var pos = Random.insideUnitSphere*100;
         Vector3 pos = Consts.IsHost ? new Vector3(0, 5, 0) : new Vector3(50, 5, 50);
//        pos.y = 5;
        
        GameObject p = Network.Instantiate(player, pos, Quaternion.identity, 0) as GameObject;
        p.GetComponent<Player>().Name = name;
        _rb = p.GetComponent<Rigidbody>();
        Debug.Log("wecolvome:"+name);
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

        var scrap = Resources.Load("Prefabs/Scrap", typeof(GameObject)) as GameObject;
        var wall = Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject;
        for (var i = 0; i < NUM_CUBES; i++)
        {

            var point = Random.insideUnitSphere * 500;
            point.y = Math.Abs(point.y);

            var point2 = Random.insideUnitSphere * 500;
            point.y = Math.Abs(point2.y)/2;

            GameObject s,w;
            if (Consts.IsSinglePlayer)
            {
                s = Instantiate(scrap, point, Quaternion.identity) as GameObject;
                w = Instantiate(wall, point2, Quaternion.identity) as GameObject;
            }
            else
            {
                s = Network.Instantiate(scrap, point, Quaternion.identity, 0) as GameObject;
                w = Network.Instantiate(wall, point2, Quaternion.identity, 0) as GameObject;
            }
//            s.transform.localScale += new Vector3(Random.value, Random.value, Random.value) * 10;
          
            w.transform.localScale += new Vector3(Random.value, Random.value, Random.value) * 50;
//            c.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);

           
            
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
