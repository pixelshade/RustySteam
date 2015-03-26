using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour, NetworkManager.ILoadFinish
{
    private const int NUM_CUBES = 10;
    private List<GameObject> cubes = new List<GameObject>();
    private NetworkManager _networkManager;
    private Rigidbody _rb;

    private TeamInfo _teamA, _teamB;

    public List<GameObject> PlayersGameObjects = new List<GameObject>();
//    private List<NetworkManager.PlayerInfo> _playerInfos;

    private Consts.GameModes _gameMode;
    
	// Use this for initialization
	void Start () {
        _networkManager = NetworkManager.Get();
	   
        
        _teamA = new TeamInfo("Edison");
        _teamB = new TeamInfo("Tesla");

	    if (Consts.IsHost)
	    {
            InitWorld();
            
	    }

        SpawnPlayer(PlayerPrefs.GetString("NickName"));
	}


    public void DividePlayersToTeams()
    {
        var playerInfos = _networkManager.PlayerList;
        Debug.Log("DividePlayersToTeams");
        bool odd = false;
        var nw = GetComponent<NetworkView>();
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if (odd)
            {
                nw.RPC("PlayerJoinTeam", RPCMode.AllBuffered, i, 1);
            }
            else
            {
                nw.RPC("PlayerJoinTeam", RPCMode.AllBuffered, i, 2);
            }
            odd = !odd;
        }
    }


    public void SpawnPlayer(string name)
    {
        var player = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
        var pos = Random.insideUnitSphere*300;
        pos.y = 2;
//        pos.y = 5;
        
        GameObject p = Network.Instantiate(player, pos, Quaternion.identity, 0) as GameObject;
        p.GetComponent<Player>().NickName = name;
        _rb = p.GetComponent<Rigidbody>();
        Debug.Log("wecolvome:"+name);
//        Debug.Log("[NETWORK]playerName has joined the game.");
//        playerCount += 1;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(120, 0, 200, 100), "velocity " + _rb.velocity + "\n pos: " + _rb.position);
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

    [RPC]
    void PlayerJoinTeam(int playerIndex, int team)
    {
        Debug.Log("[RPC]DividePlayersToTeams");
        var playerInfos = _networkManager.PlayerList;
        if (playerInfos != null && playerInfos[playerIndex] != null)
        playerInfos[playerIndex].Team = (team == 1) ? _teamA : _teamB;
    }


    public void LoadFinished()
    {
    }

    public void AllLoadFinished()
    {
        DividePlayersToTeams();
     
    }
}



public class TeamInfo
{
    public string TeamName;
//    public List<NetworkManager.PlayerInfo> Players= new List<NetworkManager.PlayerInfo>();
    public int Score = 0;

    public TeamInfo(string Name)
    {
        TeamName = Name;
    }
}
