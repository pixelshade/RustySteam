using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScorePanelScript : MonoBehaviour
{

    public GameObject VictoryText;
    public GameObject DefeatText;
    public GameObject MainMenuButton;
    public GameObject PlayerScorePanel;
    public GameObject PlayerScoreItem;
    public GameObject TeamAPanel;
    public GameObject TeamBPanel;

    private LevelController _levelController;


    private bool _tabShowScore = false;
    private bool _prevTabShowScore = false;
    private bool _tabHideScore;

    // Use this for initialization
	void Start ()
	{
	    GetComponent<Image>().enabled = false;
        VictoryText.SetActive(false);
        DefeatText.SetActive(false);
        MainMenuButton.SetActive(false);
        _levelController = LevelController.Get();
        PlayerScorePanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        _tabShowScore = Input.GetButtonDown("ShowScore");
        _tabHideScore = Input.GetButtonUp("ShowScore");
      
        if (_tabShowScore != _prevTabShowScore)
        {
            if (_tabShowScore)
            {
                Debug.Log("SHOW SCORE");
                ShowPlayerScores();
            }
            _prevTabShowScore = _tabShowScore;
        }

	    if (_tabHideScore)
	    {
            HidePlayerScores();
            Debug.Log("hide SCORE");
	    }
        
	}

    public void GoToMainMenu()
    {
        _levelController.LeaveGameAndGoToMainMenu();
    }


    public void ShowPlayerScores()
    {
      
        Debug.Log("ShowPlayerScores");
        PlayerScorePanel.SetActive(true);
        TeamInfo team1 = null;
        var _playerInfos = _levelController.PlayerInfos;
        if (_playerInfos != null)
            foreach (var player in _playerInfos)
            {
                if (player == null || player.Team == null) continue;
                if (team1 == null) team1 = player.Team;

                var plItem = Instantiate(PlayerScoreItem);
                var plText = plItem.GetComponent<Text>();

                var info = player.Team.Id + "   " + player.NickName + "     " + player.Kills + "    " + player.Deaths;
                plText.text = info;
                //plText.color = player.Team.Color;
                if (player.Team == team1)
                {
                    plItem.transform.SetParent(TeamAPanel.transform,false);
                    
                }
                else
                {
                    plItem.transform.SetParent(TeamBPanel.transform, false);
                }
            }
    }

    public void HidePlayerScores()
    {
        Debug.Log("HidePlayerScores");
            int childs = TeamAPanel.transform.childCount;
        Debug.Log("Child count:"+ childs);
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(TeamAPanel.transform.GetChild(i).gameObject);
            }
            childs = TeamBPanel.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(TeamBPanel.transform.GetChild(i).gameObject);
            }
        PlayerScorePanel.SetActive(false);
    }

    public void ShowEndGameScore()
    {
        GetComponent<Image>().enabled = true;
        MainMenuButton.SetActive(true);
        var networkManager = NetworkManager.Get();
       
        var selfInfo = _levelController.PlayerInfos[networkManager.GetPosition(false)];
        var winningTeam =  (LevelController.TeamA.Score > LevelController.TeamB.Score) ? LevelController.TeamA : LevelController.TeamB;
        if (selfInfo.Team == winningTeam)
        {
            VictoryText.SetActive(true);
        }
        else
        {
            DefeatText.SetActive(true);
        }
        ShowPlayerScores();
    }

}
