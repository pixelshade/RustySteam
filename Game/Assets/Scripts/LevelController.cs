using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour, NetworkManager.ILoadFinish
{
    public const int NumCubes = 10;
    public const int NumDanger = 4;
    
    private NetworkManager _networkManager;
    private Rigidbody _rb;

    public GameObject ScorePanel;
    public Text TeamAScoreText;
    public Text TeamBScoreText;
    public GameObject ZepelinGameObject;

    public Consts.GameModes GameMode;
    public static TeamInfo TeamA, TeamB;
    public static TeamInfo[] TeamInfos;
    public static int VictoryScore = 30;


    public List<GameObject> PlayersGameObjects;
    public List<GameObject> MovableGameObjects;

    public List<NetworkManager.PlayerInfo> PlayerInfos;

    private int _killer, _victim = -1;
    private DeathType _deathType;
    
    private float _timeToShowKill;
    
	// Use this for initialization
	void Start () {
        _networkManager = NetworkManager.Get();
        PlayerInfos = _networkManager.PlayerList;

	    GameMode =  (Consts.GameModes) _networkManager.GameMode;
        
        TeamA = new TeamInfo("Edison", 0, new Color(146f / 255, 118f / 255, 218f / 255, 0));
        TeamB = new TeamInfo("Tesla", 1, new Color(194f / 255, 64f / 255, 63f / 255, 0));

        TeamInfos = new TeamInfo[]{TeamA,TeamB};

	    if (Consts.IsHost)
	    {
            InitWorld();
            
	    }

        SpawnPlayer(PlayerPrefs.GetString("NickName"));
	}

    public static LevelController Get()
    {
        GameObject go = GameObject.Find("Main");
        return go == null ? null : go.GetComponent<LevelController>();
    }

    public void DividePlayersToTeams()
    {
        var playerInfos = _networkManager.PlayerList;
        Debug.Log("DividePlayersToTeams");
        bool odd = false;
        var nw = GetComponent<NetworkView>();
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if(playerInfos[i]==null) continue;
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

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
            else
                Debug.Log("Successfully diconnected from the server");
    }

    public void SpawnPlayer(string name)
    {
        var player = Resources.Load("Prefabs/Player", typeof(GameObject)) as GameObject;
//        GameObject[] array1 = GameObject.FindGameObjectsWithTag("SpawnA");
//        GameObject[] array2 = GameObject.FindGameObjectsWithTag("SpawnB");
//        GameObject[] newArray = new GameObject[array1.Length + array2.Length];
//        Array.Copy(array1, newArray, 0);
//        Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
//        var spawnChoice = Random.Range(0, newArray.Length);
//        Vector3 p = new Vector3(0, 0, 0);
//        if (newArray.Length == 0)
//        {
//            Debug.LogError("No spawns, spawning player on 0,0,0");
//        }
//        else
//        {
//            p = newArray[spawnChoice].GetComponent<Transform>().position;
//        }
        GameObject pl = Network.Instantiate(player, Random.insideUnitSphere*100, Quaternion.identity, 0) as GameObject;
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
        ZepelinGameObject.SetActive(false);
    }

    public void SetUpDeathMatchMode()
    {
        Debug.Log("DM MODE");
        ZepelinGameObject.SetActive(false);

        var area = 200;
        var scrap = Resources.Load("Prefabs/Scrap", typeof(GameObject)) as GameObject;
        //var wall = Resources.Load("Prefabs/Wall", typeof(GameObject)) as GameObject;
        var danger = Resources.Load("Prefabs/DangerZone", typeof(GameObject)) as GameObject;
        for (var i = 0; i < NumCubes; i++)
        {

            var point = Random.insideUnitSphere * area;
            point.y = Math.Abs(point.y);

            var point2 = Random.insideUnitSphere * area;
            point2.y = Math.Abs(point2.y) / 5;

            GameObject s, w;
            if (Consts.IsSinglePlayer)
            {
                s = Instantiate(scrap, point, Quaternion.identity) as GameObject;
                //w = Instantiate(wall, point2, Quaternion.identity) as GameObject;
            }
            else
            {
                s = Network.Instantiate(scrap, point, Quaternion.identity, 0) as GameObject;
                //w = Network.Instantiate(wall, point2, Quaternion.identity, 0) as GameObject;
            }
            //w.transform.localScale += new Vector3(Random.value, Random.value / 3, Random.value) * 30;
        }

        /*for (var i = 0; i < NumDanger; i++)
        {
            var point = Random.insideUnitSphere * area;
            point.y = 0.07f;

            GameObject d;
            if (Consts.IsSinglePlayer)
            {
                d = Instantiate(danger, point, Quaternion.identity) as GameObject;
            }
            else
            {
                d = Network.Instantiate(danger, point, Quaternion.identity, 0) as GameObject;
            }
            d.transform.localScale += new Vector3(Random.value, 0, Random.value) * 10;
        }*/

    }

    public void SetUpKingOfTheHillMode()
    {

    }

    public void LeaveGameAndGoToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (Network.isServer || Network.isClient)
        {
            if (Network.isServer) MasterServer.UnregisterHost();
            Network.Disconnect();
            Application.LoadLevel(Consts.MainMenuScene);
        }
    }


	// Update is called once per frame
	void Update () {
	
	}

    [RPC]
    void PlayerJoinTeam(int playerIndex, int team)
    {
        Debug.Log("[RPC]PlayerJoinTeam("+playerIndex + ","+ team +")");

        if (PlayerInfos != null && PlayerInfos[playerIndex] != null)
        {
            PlayerInfos[playerIndex].Team = TeamInfos[team];
            var myPos = _networkManager.GetPosition(false);
            if (myPos == playerIndex && GetComponent<NetworkView>().isMine)
            {
                Debug.Log(PlayersGameObjects.Count + " a index" + playerIndex + " team:" + team);
                GetPlayerGameObject(playerIndex).GetComponent<Player>().Respawn(0);
            }
        }
    }

    [RPC]
    void PlayerKillEnemyWith(int killer, int victim, int deathType)
    {
        if (killer >= 0)
            if (PlayerInfos != null && PlayerInfos[killer] != null && PlayerInfos[victim] != null)
            {
			    if (killer != victim) PlayerInfos[killer].Kills++;
                if (GameMode == Consts.GameModes.DeathMatch)
                {
                    if (TeamA == PlayerInfos[victim].Team && killer != victim) TeamB.Score++;
                    if (TeamB == PlayerInfos[victim].Team && killer != victim) TeamA.Score++;
                    //PlayerInfos[killer].Team.Score++;
                }
            }
        PlayerInfos[victim].Deaths++;
        ShowKill(killer, victim, deathType);

      //  if (TeamA.Score >= VictoryScore || TeamB.Score >= VictoryScore) EndGameShowWinners();
    }

    private void ShowKill(int killer, int victim, int deathType )
    {
        _deathType = (DeathType)deathType;
        _killer = killer;
        _victim = victim;
        _timeToShowKill = Time.time + 5;

    }


    public void EndGameShowWinners()
    {
        DisableAllPlayerMovement();
        ScorePanel.GetComponent<ScorePanelScript>().ShowEndGameScore();
    }

    private void DisableAllPlayerMovement()
    {
        foreach (var playersGameObject in PlayersGameObjects)
        {
            playersGameObject.GetComponent<FPSRigidController>().Velocity = 0;
            playersGameObject.GetComponent<FPSRigidController>().enabled = false;
        }
    }


    public GameObject GetPlayerGameObject(int playerId)
    {
        foreach (var playersGameObject in PlayersGameObjects)
        {
            if (playersGameObject.GetComponent<NetworkView>().owner == PlayerInfos[playerId].Player)
            {
                return playersGameObject;
            }
        }
        return null;
    }

	public void SpawnFlagForTeam(TeamInfo team){
		GameObject flagSpawn;
        GameObject flag, flagPrefab;
        
		if (team == TeamA) {
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
        PlayersGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        DividePlayersToTeams();
        GetComponent<NetworkView>().RPC("SetupPlayers", RPCMode.AllBuffered);
        GetComponent<NetworkView>().RPC("SetupMovables", RPCMode.AllBuffered);
    }


    [RPC]
    public void SetupPlayers()
    {
        PlayersGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (var playerGO in PlayersGameObjects)
        {
            foreach (var playerInfo in PlayerInfos)
            {
                if(playerInfo==null) continue;
                
                if (playerGO.GetComponent<NetworkView>().owner == playerInfo.Player)
                {
                    Debug.Log("Setup player: "+playerInfo.NickName);
                    var playerScript = playerGO.GetComponent<Player>();
                    playerScript.Id = _networkManager.GetPosition(false, playerInfo.Player);
                    playerScript.NickName = playerInfo.NickName;
                    playerScript.SetTeamColor(playerInfo.Team.Color);
                }
            }
        }

    }

    [RPC]
    public void SetupMovables()
    {
        MovableGameObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("movable"));
        var i = -1;
        foreach (var movableGO in MovableGameObjects)
        {
            movableGO.GetComponent<Movable>().Id = i;
            i--;
        }

    }

    [RPC]
    public void UpdateScore(int teamId, int score)
    {
        if (Consts.IsHost) return;
        TeamInfos[teamId].Score = score;
    }
}



public class TeamInfo
{
    public string TeamName;
    public int Id;
    private int _score = 0;
    public Color Color;

    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            if (Consts.IsHost) LevelController.Get().GetComponent<NetworkView>().RPC("UpdateScore", RPCMode.AllBuffered, Id, value);
            if (Id == 0)
            {
                LevelController.Get().TeamAScoreText.text = value.ToString();
            }
            else
            {
                LevelController.Get().TeamBScoreText.text = value.ToString();
            }
            if (value >= LevelController.VictoryScore) LevelController.Get().EndGameShowWinners();
        }


    }

    public TeamInfo(string name, int id, Color color)
    {
        TeamName = name;
        Id = id;
        Color = color;
    }
}

enum DeathType
{
    Fall, Hit, DeathZone
}


