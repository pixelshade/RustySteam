using UnityEngine;
using System.Collections;

public class FlagDropZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision){
		var go = collision.gameObject;
		var level = GameObject.Find ("Main").GetComponent<LevelController> ();
		if (go != null) {
			var player = go.GetComponent<Player>();
			if(player!=null){
				if(player.HasFlag){
					player.HasFlag = false;
					if(level.GameMode == Consts.GameModes.CaptureTheFlag){
						level.PlayerInfos[player.Id].Team.Score++;
						level.SpawnFlagForTeam(level.PlayerInfos[player.Id].Team);
					}
				}
			}
		}
		
	}
}
