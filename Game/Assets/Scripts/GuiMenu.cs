using UnityEngine;
using System.Collections;

public class GuiMenu : MonoBehaviour {
//    private NetworkManager _networkManager;



	// Use this for initialization
	void Start () {
	
	}

    void OnGUI()
    {


        GUI.Label(new Rect(Screen.width / 2 - 50, 20, 100, 50), "Rusty Steam");

        GUI.Box(new Rect(Screen.width /2 - 100, 100, 200, 150), "Menu");

        // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
        if (GUI.Button(new Rect(Screen.width / 2 -50, 140, 100, 20), "Create game"))
        {
            Application.LoadLevel("GameScene");
        }

        // Make the second button.
        if (GUI.Button(new Rect(Screen.width / 2 -50, 180 , 100, 20), "Join game"))
        {
            Application.LoadLevel("GameScene");
        }
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
