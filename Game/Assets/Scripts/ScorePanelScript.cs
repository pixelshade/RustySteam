using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScorePanelScript : MonoBehaviour
{

    public GameObject VictoryText;
    public GameObject DefeatText;
    public GameObject MainMenuButton;

    private LevelController _levelController;


	// Use this for initialization
	void Start ()
	{
	    GetComponent<Image>().enabled = false;
        VictoryText.SetActive(false);
        DefeatText.SetActive(false);
        MainMenuButton.SetActive(false);
        _levelController = LevelController.Get();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GoToMainMenu()
    {
        _levelController.LeaveGameAndGoToMainMenu();
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
    }

}
