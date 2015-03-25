using UnityEngine;
using System.Collections;

public class GuiGame : MonoBehaviour
{
    public Texture CrosshairTexture;

    private Player _player;
    private PullController _pull;

    private bool _crosshairSelected;

    
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
        if (_respawnTime > 0)
        {
            GUI.TextArea(new Rect(Screen.width / 2 - 12, Screen.height / 2 - 100, 200, 24), _respawnTime.ToString("D"), _hugeGuiStyle);
        }
        else if (_respawnTime > -1)
        {
            GUI.TextArea(new Rect(Screen.width / 2 - 12, Screen.height / 2 - 100, 200, 24), "GO!", _hugeGuiStyle);
        }
        _respawnTime -= Time.deltaTime;
        
    }

    public void CrosshairSelected(bool selected)
    {
        _crosshairSelected = selected;
    }


    public void RespawnIn(float seconds)
    {
        _respawnTime = seconds;
        Debug.Log("resp " + _respawnTime);
    }
}
