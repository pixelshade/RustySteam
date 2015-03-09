using UnityEngine;

public class Consts
{

    public static string GameName = "Rusty Steam Beta";
    public static int maxPlayers = 8;
    public static int Port = 25000;

    public static bool IsSinglePlayer
    {
        get { return !Network.isServer && !Network.isClient; }
    }

    public static bool IsHost
    {
        get { return Network.isServer || IsSinglePlayer; }
    }


    // Scenes


    public static string MainMenuScene
    {
        get { return "MainMenu"; }
    }

    public static string GameScene
    {
        get { return "GameScene"; }
    }
}
