using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class GuiMenu : MonoBehaviour {
    public enum MenuState
    {
        MainMenu, Lobby, Join, Host
    }

//    public Texture Logo;

    public float AutoServerListRefreshTimeDelay = 5;
    public bool AutoRefreshServersList = true;
    private float _lastAutoRefresh;

    private HostData[] _hostList;
    private MenuState _state;
    private string _nick;
    private int _campParty;
    private readonly Rect _centRect = new Rect(Screen.width/2 - 100, 100, 200, 400);


    private bool _disconnect;
    private NetworkManager _networkManager;

    private readonly string[] _selModesStrings = Enum.GetNames(typeof(Consts.GameModes));

    private int _selModeInt = 0;
    private int _prevSelModeInt = 0;

    // panels
    private GameObject _mainMenuPanel;

    public void SetState(MenuState newState)
    {
        _state = newState;
    }


	void Start ()
	{
	   
        _networkManager = NetworkManager.Get();
        Debug.Log(_networkManager);
        _nick = "Player" + Random.Range(0, 20);
        if (PlayerPrefs.HasKey("NickName"))
        {
            _nick = PlayerPrefs.GetString("NickName");
        }
        SetState(MenuState.MainMenu);
	    _mainMenuPanel = GameObject.Find("MainMenuPanel") as GameObject;
        
        

	}

    void Awake()
    {
//        GetComponent<NetworkView>().group = 0;
    }

    private void OnGUI()
    {
//        GUI.DrawTexture(new Rect(Screen.width / 2 - 100, 0, 200, 100), Logo,ScaleMode.ScaleToFit);
        GUILayout.BeginArea(_centRect);
        if (_state == MenuState.MainMenu)
        {
            MainMenu();
        }
        if (_state == MenuState.Join)
        {
            JoinGame();
        }
        if (_state == MenuState.Host)
        {
            HostGame();
        }
        if (_state == MenuState.Lobby)
        {
            GameLobby();
        }
        GUILayout.EndArea();
    }

    // Update is called once per frame
	void Update () {
	
	}


    void MainMenu()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name:");
        _nick = GUILayout.TextField(_nick,20);
        PlayerPrefs.SetString("NickName", _nick);
        GUILayout.EndHorizontal();
        


        if (GUILayout.Button("Create game"))
        {
            SetState(MenuState.Host);
        }


        if (GUILayout.Button("Join game"))
        {
            SetState(MenuState.Join);
        }

        if (GUILayout.Button("Exit"))
        {
            Application.Quit();
        }


       

    }

    void JoinGame()
    {
                 if (AutoRefreshServersList)
                 {
                     if (Time.time - _lastAutoRefresh > AutoServerListRefreshTimeDelay)
                     {
                         _lastAutoRefresh = Time.time;
                         Debug.Log("Refresh server list"+Time.time);
                         MasterServer.RequestHostList(Consts.GameName);
                     }
                 }
		        GUILayout.Label("Join Game");
		        GUILayout.Label("Servers:");
		        if (_hostList != null){
		            foreach (var t in _hostList)
		            {
		                if (GUILayout.Button(t.gameName)){
		                    _disconnect = false;
		                    var e = Network.Connect(t);
		                    Debug.Log(e);
		                }
		            }
		        }
		        GUILayout.BeginHorizontal();
		        if(GUILayout.Button("Back")){
		            SetState(MenuState.MainMenu);
		        }
		        if(GUILayout.Button("Refresh")){
		            MasterServer.RequestHostList(Consts.GameName);
		        }
		        GUILayout.EndHorizontal();
		
    }

    void HostGame()
    {
 
//		        GUILayout.Label("Host Game");
//		        
//		        GUILayout.BeginHorizontal();
//		        if(GUILayout.Button("Back")){
//		            SetState(MenuState.MainMenu);
//		        }
//		        if(GUILayout.Button("Start Server")){
		            //Network.InitializeSecurity();
		            _disconnect = false;
		            Network.InitializeServer(Consts.maxPlayers, Consts.Port, !Network.HavePublicAddress());
		            MasterServer.RegisterHost(Consts.GameName, _nick + "'s Game");
//		        }
//		        GUILayout.EndHorizontal();
		   
    }



    private void GameLobby()
    {
        GUILayout.Label("Lobby");
        int count = 0;
        for (int i = 0; i < (Consts.maxPlayers); i++)
        {
            if (_networkManager.PlayerList[i] != null) count++;
        }
        GUILayout.Label("Players (" + count + "/" + (Consts.maxPlayers) + "):");
        for (int i = 0; i < Consts.maxPlayers; i++)
        {
            if (i%2 == 0) GUILayout.BeginHorizontal();
            if (_networkManager.PlayerList[i] == null)
            {
                GUILayout.Label("<Empty>");
            }
            else
            {
                if (Consts.IsHost)
                {
                    if (GUILayout.Button(_networkManager.PlayerList[i].NickName))
                    {
                        GetComponent<NetworkView>()
                            .RPC("KickPlayer", RPCMode.AllBuffered, _networkManager.PlayerList[i].Player);
                    }
                }
                else
                {
                    GUILayout.Label(_networkManager.PlayerList[i].NickName);
                }
            }
            if (i%2 == 1) GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        if (Consts.IsHost)
        {
            _selModeInt = GUILayout.SelectionGrid(_selModeInt, _selModesStrings, 1);
            if (_selModeInt != _prevSelModeInt)
            {
                GetComponent<NetworkView>().RPC("SetGameModeInMenu", RPCMode.OthersBuffered, _selModeInt);
                _prevSelModeInt = _selModeInt;
            }

        }
        else
        {
            GUILayout.Label("Game mode:"+(Consts.GameModes)_selModeInt);
           
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Leave Lobby"))
        {
            StartCoroutine(LeaveLobby());
        }
        if (Consts.IsHost)
        {
            if (GUILayout.Button("Start Game"))
            {
                MasterServer.UnregisterHost();

                _networkManager.LoadLevel(Consts.GameScene, _selModeInt);
            }
            
            
           
        }
        else
        {
            GUILayout.Label("Wait for Host to Start the Game");
        }
        GUILayout.EndHorizontal();

    }

    void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			_hostList = MasterServer.PollHostList();
	}

	void OnConnectedToServer()
	{
        Debug.Log("OnConnectedToServer");
		_networkManager.PlayerList.Clear();
        for (var i = 0; i < Consts.maxPlayers; i++)
        {
			_networkManager.PlayerList.Add(null);
		}
		SetState(MenuState.Lobby);
		GetComponent<NetworkView>().RPC("PlayerConnect", RPCMode.Server, Network.player, _nick);
	}

	void OnServerInitialized()
	{
        Debug.Log("OnServerInitialized");
		_networkManager.PlayerList.Clear();
        for (var i = 0; i < Consts.maxPlayers; i++)
        {
			_networkManager.PlayerList.Add(null);
		}
		SetState(MenuState.Lobby);
		GetComponent<NetworkView>().RPC("AddPlayer", RPCMode.AllBuffered, Network.player, _nick, 0);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		GetComponent<NetworkView>().RPC("PlayerLeft", RPCMode.All, player);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Disconnected from server: " + info);
		if(!_disconnect)
			StartCoroutine(LeaveLobby());
	}

	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
	}

	[RPC] 
	void AddPlayer(NetworkPlayer player, string username, int i) {
		Debug.Log("got addplayer " + username);
	    _networkManager.PlayerList[i] = new NetworkManager.PlayerInfo {Player = player, NickName = username};
	}

	[RPC] 
	void KickPlayer(NetworkPlayer player) {
		if(player == Network.player){
			StartCoroutine(LeaveLobby());
		}
	}

	[RPC]
	void PlayerConnect(NetworkPlayer player, string username) {
		var x = -1;
        for (var i = 0; i < Consts.maxPlayers; i++)
        {
		    if (_networkManager.PlayerList[i] != null) continue;
		    x = i;
		    break;
		}
		if(x != -1){
			GetComponent<NetworkView>().RPC("AddPlayer", RPCMode.AllBuffered, player, username, x);
		}
	}

	[RPC]
	void PlayerLeft(NetworkPlayer player) {
		var x = -1;
		for (var i = 0; i <  _networkManager.PlayerList.Count; i++) {
		    if (_networkManager.PlayerList[i] == null || player != _networkManager.PlayerList[i].Player) continue;
		    x = i;
		    break;
		}
		if(x != -1){
			_networkManager.PlayerList[x] = null;
		}
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}


    [RPC]
    void SetGameModeInMenu(int gameMode)
    {
        _selModeInt = gameMode;
    }

	IEnumerator LeaveLobby() {
        SetState(MenuState.MainMenu);
		if (Network.isServer || Network.isClient) {
			_disconnect = true;
			if (Network.isServer) MasterServer.UnregisterHost();
			Network.Disconnect();
			yield return new WaitForSeconds(.3f);
		}
	}
}
