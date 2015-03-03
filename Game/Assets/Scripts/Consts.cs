using UnityEngine;

public class Consts
{

    public static bool IsSinglePlayer
    {
        get { return !Network.isServer && !Network.isClient; }
    }

    public static bool IsHost
    {
        get { return Network.isServer || IsSinglePlayer; }
    }
}
