using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class GuiGame : MonoBehaviour
{
    
    public Texture DmgTexture;
    public Texture CrosshairTexture;
    public Texture CdBarBgTexture;
    public Texture CdBarTexture;
    public Texture TeamsScoreTexture;
    public AudioClip OuchAudioClip;

    private Player _player;
    private PullController _pullController;
    

    private bool _crosshairSelected;
    private List<NetworkManager.PlayerInfo> _playerInfos;
    
    private float _respawnTime;

    private GUIStyle _hugeGuiStyle;
    private float _dmgTakenTime;
    private NetworkView _networkView;
    private AudioSource _audioSource;
 

    private const float DMG_TAKEN_ANIMATION_DUR = 1;

	// Use this for initialization
    void Awake()
    {
        _player = GetComponent<Player>();
        _pullController = GetComponent<PullController>();
    }

	void Start ()
	{
	    _hugeGuiStyle = new GUIStyle();
	    _hugeGuiStyle.fontSize = 72;
	    _playerInfos = NetworkManager.Get().PlayerList;
	    _networkView = GetComponent<NetworkView>();
	    _audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
    
	}

    void OnGUI()
    {
//                GUI.Label(new Rect(120, 0, 200, 100), "velocity " + _rb.velocity + "\n pos: " + _rb.position);
        if (_networkView.isMine)
        {

//            GUI.TextArea(new Rect(0, 0, 100, 50),
//                "id:"+ _player.Id + "HP:" + _player.HP + " \n Power:" + _pullController.Power + "\n Range: " + _pullController.Range);

//        if (_pullController.CdLeft()>0)
//        {
//            GUI.TextArea(new Rect(Screen.width/2 - 50, Screen.height - 50, 100, 50),
//                " CD ready in " + _pullController.CdLeft().ToString("F1") + "s");

        //    GUI.DrawTexture(new Rect(Screen.width / 2 - 128, -5, 256, 68), TeamsScoreTexture);
            GUI.DrawTexture(new Rect(Screen.width / 2 - 50, Screen.height - 30, 100, 25), CdBarBgTexture);
            GUI.DrawTexture(new Rect(Screen.width / 2 - 35, Screen.height - 22, 70 * ((_pullController.CD-_pullController.CdLeft())/_pullController.CD), 10), CdBarTexture, ScaleMode.StretchToFill);
//        }
//        GUI.Box(new Rect(Screen.width / 2 - 100, 100, 200, 150), "Menu");

            CrossHairGUI();

            RespawnGUI();

//          ScoreGUI();

            DmgTakenAnimate();
        }
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

    public void PlayTakeDamageAnimation()
    {
        _dmgTakenTime = Time.time;
        _audioSource.PlayOneShot(OuchAudioClip);

    }

    private void DmgTakenAnimate()
    {
        var diff = Time.time - _dmgTakenTime;
        if (diff < DMG_TAKEN_ANIMATION_DUR)
        {
            GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), DmgTexture, ScaleMode.StretchToFill, true);
        }
    }

    private void ScoreGUI()
    {
        string plrScore = "";
        string strTeam1 = "", strTeam2 = "";
        TeamInfo team1 = null;
        if (_playerInfos != null)
        foreach (var player in _playerInfos)
        {

            if (player == null || player.Team == null) continue;
            if (team1 == null) team1 = player.Team;

                //var info = player.Team.Id +" " + player.Team.TeamName + "- " + player.Team.Score + " " + player.NickName + " K:" + player.Kills + " D:" + player.Deaths + "\n";
            var info = player.NickName + " K:" + player.Kills + " D:" + player.Deaths + "\n";
            if (player.Team == team1)
            {
                strTeam1 += info;
            }
            else
            {
                strTeam2 += info;
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
