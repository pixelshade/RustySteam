using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class NetworkManager : MonoBehaviour {


	public class PlayerInfo {
		public string NickName;
		public NetworkPlayer Player;
	    public TeamInfo Team;
	    public int Score;
	    public int Kills;
	    public int Deaths;
	}

	public interface ILoadFinish {
		void LoadFinished();
		void AllLoadFinished();
	}

    
    private int _lastLevelPrefix = 1;
	public List<PlayerInfo> PlayerList = new List<PlayerInfo>();
	public List<ILoadFinish> LoadFinish = new List<ILoadFinish>();
	private int _loaded;

    public int GameMode;
    


    void Awake ()
	{
		DontDestroyOnLoad(this);
        //Application.LoadLevel("MainMenu");
	}

	public void LoadLevel(string level, int gameMode){
		Network.RemoveRPCsInGroup(0);
        GetComponent<NetworkView>().RPC("LoadLevelRPC", RPCMode.AllBuffered, level, _lastLevelPrefix, gameMode);
	}


	[RPC] 
	IEnumerator LoadLevelRPC (string level, int levelPrefix, int gameMode){
        GameMode = gameMode;
        Debug.Log("GameMode je");
        Debug.Log(gameMode);
        _loaded = 0;
		_lastLevelPrefix = levelPrefix;
		Network.SetSendingEnabled(0, false);    
		Network.isMessageQueueRunning = false;
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		yield return null;
		yield return null;
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);
	    
		if(Network.isServer){
			TellLoaded();
		} else {
			GetComponent<NetworkView>().RPC("TellLoaded", RPCMode.Server);
		}
		foreach (var t in LoadFinish){
		    t.LoadFinished();
		}
	}

	[RPC]
	private void TellLoaded(){
		_loaded++;
	    if (_loaded < GetCount()) return;
	    foreach (var t in LoadFinish)
	    {
	        t.AllLoadFinished();
	    }
	}

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

	void OnDisconnectedFromServer (){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Destroy(gameObject);
        Application.LoadLevel(Consts.MainMenuScene);
        _lastLevelPrefix = 1;
        PlayerList = new List<PlayerInfo>();
        LoadFinish = new List<ILoadFinish>();
        _loaded = 0;
        GameMode = 0;

	}

	public int GetCount(){
		if(Consts.IsSinglePlayer) return 1;
	    return PlayerList.Count(t => t != null);
	}

	public int GetPosition(bool skipSpaces, NetworkPlayer? p = null){
		if(Consts.IsSinglePlayer) return 0;
	    var pl = p.HasValue ? p.GetValueOrDefault() : Network.player;
		var x = 0;
		for(var i = 0; i < PlayerList.Count; i++){
			if(PlayerList[i].Player == pl){
				return skipSpaces ? i : x;
			}
			if(PlayerList[i] != null) x++;
		}
		return -1;
	}

	public static NetworkManager Get(){
		GameObject go = GameObject.Find("NetworkManager");
	    return go == null ? null : go.GetComponent<NetworkManager>();
	}
}