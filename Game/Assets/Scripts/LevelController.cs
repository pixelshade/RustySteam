using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour, NetworkManager.ILoadFinish
{
    public const int NumCubes = 10;
    
    private NetworkManager _networkManager;
    private Rigidbody _rb;

    private Consts.GameModes _gameMode;
    private TeamInfo _teamA, _teamB;

    public List<GameObject> PlayersGameObjects = new List<GameObject>();

    private List<NetworkManager.PlayerInfo> _playerInfos;

    private int _killer, _victim = -1;
    private DeathType _deathType;
    
    private float _timeToShowKill;
    
	// Use this for initialization
	void Start () {
        _networkManager = NetworkManager.Get();
        _playerInfos = _networkManager.PlayerList;

        _gameMode = Consts.GameModes.DeathMatch;

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
        
        GameObject p = Network.Instantiate(player, pos, Quaternion.identity, 0) as GameObject;
        p.GetComponent<Player>().NickName = name;
    }

    void OnGUI()
    {
        if (_timeToShowKill > Time.time)
        {
            if (_killer != -1)
            {
                GUI.TextArea(new Rect(Screen.width/2-100, 50, 200, 20),
                    _playerInfos[_killer].NickName + " -[" + _deathType + "]- " + _playerInfos[_victim].NickName);
            }
            else
            {
                GUI.TextArea(new Rect(Screen.width/2-100, 50, 200, 20),
                    "-[#cruelWorld]-" + _playerInfos[_victim].NickName);
            }
        }
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
        for (var i = 0; i < NumCubes; i++)
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
            w.transform.localScale += new Vector3(Random.value, Random.value, Random.value) * 50;
        }
    }

	// Update is called once per frame
	void Update () {
	
	}

    [RPC]
    void PlayerJoinTeam(int playerIndex, int team)
    {
        Debug.Log("[RPC]DividePlayersToTeams");
       
        if (_playerInfos != null && _playerInfos[playerIndex] != null)
        _playerInfos[playerIndex].Team = (team == 1) ? _teamA : _teamB;
    }

    [RPC]
    void PlayerKillEnemyWith(int killer, int victim, int deathType)
    {
        if (_playerInfos != null && _playerInfos[killer] != null && _playerInfos[victim] != null)
        {
            _playerInfos[killer].Kills++;
            _playerInfos[victim].Deaths++;
            if (_gameMode == Consts.GameModes.DeathMatch && _playerInfos[killer].Team != _playerInfos[victim].Team)
            {
                _playerInfos[killer].Team.Score++;
            }
            ShowKill(killer, victim, deathType);
        }
    }

    private void ShowKill(int killer, int victim, int deathType )
    {
        _deathType = (DeathType)deathType;
        _killer = killer;
        _victim = victim;
        _timeToShowKill = Time.time + 5;

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
    public int Score = 0;

    public TeamInfo(string name)
    {
        TeamName = name;
    }
}

enum DeathType
{
    Fall, Hit, DeathZone
}


