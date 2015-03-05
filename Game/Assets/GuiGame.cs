using UnityEngine;
using System.Collections;

public class GuiGame : MonoBehaviour
{

    private Player _player;
    private PullController _pull;

	// Use this for initialization
    void Awake()
    {
        _player = GetComponent<Player>();
        _pull = GetComponent<PullController>();
    }

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {

        GUI.TextArea(new Rect(0, 0, 100, 50),"HP:" + _player.HP+ " \n Power:"+ _pull.Power + "\n Range: "+ _pull.Range);

        if (_pull.CdLeft()>0)
        {
            GUI.TextArea(new Rect(Screen.width/2 - 50, Screen.height-50, 100, 50),  " CD ready in "+ _pull.CdLeft() +"s");
        }
//        GUI.Box(new Rect(Screen.width / 2 - 100, 100, 200, 150), "Menu");
    }

   
}
