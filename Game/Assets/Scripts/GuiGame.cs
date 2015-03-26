using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuiGame : MonoBehaviour
{
    public Texture CrosshairTexture;

    private Player _player;
    private PullController _pull;
    

    private bool _crosshairSelected;
    private List<NetworkManager.PlayerInfo> _playerInfos;
    
    private float _respawnTime;

    private GUIStyle _hugeGuiStyle;

	// Use this for initialization
    void Awake()
    {
        _player = GetComponent<Player>();
        _pull = GetComponent<PullController>();
    }

	void Start ()
	{
	    _hugeGuiStyle = new GUIStyle();
	    _hugeGuiStyle.fontSize = 72;
	    _playerInfos = NetworkManager.Get().PlayerList;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        
        
        GUI.TextArea(new Rect(0, 0, 100, 50),"HP:" + _player.HP+ " \n Power:"+ _pull.Power + "\n Range: "+ _pull.Range);

//        if (_pull.CdLeft()>0)
//        {
            GUI.TextArea(new Rect(Screen.width/2 - 50, Screen.height-50, 100, 50),  " CD ready in "+ _pull.CdLeft() +"s");
//        }
//        GUI.Box(new Rect(Screen.width / 2 - 100, 100, 200, 150), "Menu");

        CrossHairGUI();

        RespawnGUI();

        ScoreGUI();
    }

    private void CrossHairGUI()
    {
        if (_crosshairSelected)
            GUI.DrawTexture(new Rect(Screen.width / 2 - 16, Screen.height / 2 - 16, 32, 32), CrosshairTexture, ScaleMode.ScaleToFit, true);
        else
            GUI.DrawTexture(new Rect(Screen.width / 2 - 12, Screen.height / 2 - 12, 24, 24), CrosshairTexture, ScaleMode.ScaleToFit, true);

    }


    private void RespawnGUI()
    {
        var res = _respawnTime - Time.time;
        
        if (res > 0)
        {
            GUI.TextArea(new Rect(Screen.width / 2 - 12, Screen.height / 2 - 100, 200, 24), ((int)res).ToString(), _hugeGuiStyle);
        }
        else if (res > -1)
        {
            GUI.TextArea(new Rect(Screen.width / 2 - 12, Screen.height / 2 - 100, 200, 24), "GO!", _hugeGuiStyle);
        }
        

    }


    private void ScoreGUI()
    {
        string plrScore = "";
        string strTeam1 = "", strTeam2 = "";
        TeamInfo team1 = null;
        foreach (var player in _playerInfos)
        {

            if (player == null || player.Team == null) continue;
            if (team1 == null) team1 = player.Team;
            if (player.Team == team1)
            {
                strTeam1 += player.Team.TeamName +" " + player.NickName + " K:" + player.Kills + " D:" + player.Deaths + "\n";
            }
            else
            {
                strTeam2 += player.Team.TeamName +" " +player.NickName + " K:" + player.Kills + " D:" + player.Deaths + "\n";
            }
        }
        plrScore += strTeam1 + "==========\n" + strTeam2;
        GUI.TextArea(new Rect(Screen.width - 200, 0, 200, 200), plrScore);
    }

    public void CrosshairSelected(bool selected)
    {
        _crosshairSelected = selected;
    }


    public void RespawnIn(float seconds)
    {
        _respawnTime = seconds+Time.time;
    }
}
