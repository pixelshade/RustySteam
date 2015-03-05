using UnityEngine;
using System.Collections;

public class GuiGame : MonoBehaviour
{
    public Texture CrosshairTexture;

    private Player _player;
    private PullController _pull;

    private bool _crosshairSelected;

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
        if(_crosshairSelected)
            GUI.DrawTexture(new Rect(Screen.width / 2 - 16, Screen.height / 2 - 16, 32, 32), CrosshairTexture, ScaleMode.ScaleToFit, true);
        else
            GUI.DrawTexture(new Rect(Screen.width / 2 - 12, Screen.height / 2 - 12, 24, 24), CrosshairTexture, ScaleMode.ScaleToFit, true);

        GUI.TextArea(new Rect(0, 0, 100, 50),"HP:" + _player.HP+ " \n Power:"+ _pull.Power + "\n Range: "+ _pull.Range);

        if (_pull.CdLeft()>0)
        {
            GUI.TextArea(new Rect(Screen.width/2 - 50, Screen.height-50, 100, 50),  " CD ready in "+ _pull.CdLeft() +"s");
        }
//        GUI.Box(new Rect(Screen.width / 2 - 100, 100, 200, 150), "Menu");
    }

    public void CrosshairSelected(bool selected)
    {
        _crosshairSelected = selected;
    }

   
}
