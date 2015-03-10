using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NetworkManager : MonoBehaviour {


	public class PlayerInfo {
		public string Username;
		public NetworkPlayer Player;
	}

	public interface ILoadFinish {
		void LoadFinished();
		void AllLoadFinished();
	}

    
    private int _lastLevelPrefix = 1;
	public List<PlayerInfo> PlayerList = new List<PlayerInfo>();
	public List<ILoadFinish> LoadFinish = new List<ILoadFinish>();
	private int _loaded;

    void Awake ()
	{
		DontDestroyOnLoad(this);
        //Application.LoadLevel("MainMenu");
	}

	public void LoadLevel(string level){
		Network.RemoveRPCsInGroup(0);
		GetComponent<NetworkView>().RPC("LoadLevelRPC", RPCMode.AllBuffered, level, _lastLevelPrefix + 1);
	}



	[RPC] 
	IEnumerator LoadLevelRPC (string level, int levelPrefix){
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

	void OnDisconnectedFromServer (){
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