using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour, NetworkManager.ILoadFinish
{
    public const int NumCubes = 10;
    
    private NetworkManager _networkManager;
    private Rigidbody _rb;

    public Consts.GameModes GameMode;
    public static TeamInfo _teamA, _teamB;

    public List<GameObject> PlayersGameObjects = new List<GameObject>();

    public List<NetworkManager.PlayerInfo> PlayerInfos;

    private int _killer, _victim = -1;
    private DeathType _deathType;
    
    private float _timeToShowKill;
    
	// Use this for initialization
	void Start () {
        _networkManager = NetworkManager.Get();
        PlayerInfos = _networkManager.PlayerList;

	    GameMode =  (Consts.GameModes) _networkManager.GameMode;
        
        _teamA = new TeamInfo("Edison", 1);
        _teamB = new TeamInfo("Tesla", 0);

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
                nw.RPC("PlayerJoinTeam", RPCMode.AllBuffered, i, 0);
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
     
    }

    void OnGUI()
    {
        if (_timeToShowKill > Time.time)
        {
            if (_killer != -1 && _killer != _victim)
            {
                GUI.TextArea(new Rect(Screen.width/2-100, 50, 200, 20),
                    PlayerInfos[_killer].NickName + " -[" + _deathType + "]- " + PlayerInfos[_victim].NickName);
            }
            else
            {
                GUI.TextArea(new Rect(Screen.width/2-100, 50, 200, 20),
                    "-[#cruelWorld]-" + PlayerInfos[_victim].NickName);
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

        
        switch (GameMode)
        {
                case Consts.GameModes.CaptureTheFlag: SetUpCaptureTheFlagMode();
                break;
                case Consts.GameModes.DeathMatch: SetUpDeathMatchMode();
                break;
                case Consts.GameModes.KingOfTheHill: SetUpKingOfTheHillMode();
                break;
        }
        
    }

    public void SetUpCaptureTheFlagMode()
    {

    }

    public void SetUpDeathMatchMode()
    {
        Debug.Log("DM MODE");
        var area = 500;
        var scrap = Resources.Load("Prefabs/Scrap", typeof(GameObject)) as GameObject;
        var wall = Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject;
        for (var i = 0; i < NumCubes; i++)
        {

            var point = Random.insideUnitSphere * area;
            point.y = Math.Abs(point.y);

            var point2 = Random.insideUnitSphere * area;
            point.y = Math.Abs(point2.y) / 2;

            GameObject s, w;
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

    public void SetUpKingOfTheHillMode()
    {

    }


	// Update is called once per frame
	void Update () {
	
	}

    [RPC]
    void PlayerJoinTeam(int playerIndex, int team)
    {
        Debug.Log("[RPC]DividePlayersToTeams");
       
        if (PlayerInfos != null && PlayerInfos[playerIndex] != null)
        PlayerInfos[playerIndex].Team = (team == 1) ? _teamA : _teamB;
    }

    [RPC]
    void PlayerKillEnemyWith(int killer, int victim, int deathType)
    {
        if (PlayerInfos != null && PlayerInfos[killer] != null && PlayerInfos[victim] != null)
        {
			if(killer != victim)  PlayerInfos[killer].Kills++;
            PlayerInfos[victim].Deaths++;
            if (GameMode == Consts.GameModes.DeathMatch && PlayerInfos[killer].Team != PlayerInfos[victim].Team)
            {
                PlayerInfos[killer].Team.Score++;
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


  

	public void SpawnFlagForTeam(TeamInfo team){
		GameObject flagSpawn;
        GameObject flag, flagPrefab;
        
		if (team == _teamA) {
			flagSpawn = GameObject.Find ("FlagSpawnZoneA") as GameObject;
            flagPrefab = Resources.Load("Prefabs/FlagA", typeof(GameObject)) as GameObject;
		} else {
			flagSpawn = GameObject.Find ("FlagSpawnZoneB") as GameObject;
            flagPrefab = Resources.Load("Prefabs/FlagB", typeof(GameObject)) as GameObject;
		}

		if (flagSpawn == null) {
			Debug.LogError("there is no spawn point that belongs to specified team");
			return;
		}
		
		if (Consts.IsSinglePlayer)
		{
			flag = Instantiate(flagPrefab, flagSpawn.transform.position, Quaternion.identity) as GameObject;
		}
		else
		{
			flag = Network.Instantiate(flagPrefab, flagSpawn.transform.position, Quaternion.identity, 0) as GameObject;
		}

	}


    public void LoadFinished()
    {
    }

    public void AllLoadFinished()
    {
        
        DividePlayersToTeams();
        GetComponent<NetworkView>().RPC("SetupPlayers",RPCMode.AllBuffered);
    }


    [RPC]
    public void SetupPlayers()
    {
        var playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var playerGO in playersGameObjects)
        {
            foreach (var playerInfo in PlayerInfos)
            {
                if(playerInfo==null) continue;
                
                if (playerGO.GetComponent<NetworkView>().owner == playerInfo.Player)
                {
                    Debug.Log(playerInfo.NickName);
                    var playerScript = playerGO.GetComponent<Player>();
                    playerScript.Id = _networkManager.GetPosition(false, playerInfo.Player);
                    playerScript.NickName = playerInfo.NickName;

                }
            }
        }

    }
}



public class TeamInfo
{
    public string TeamName;
    public int Id;
    public int Score = 0;

    public TeamInfo(string name, int id)
    {
        TeamName = name;
        Id = id;
    }
}

enum DeathType
{
    Fall, Hit, DeathZone
}


