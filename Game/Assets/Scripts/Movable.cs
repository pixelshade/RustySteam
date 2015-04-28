using UnityEngine;
using System.Collections;

public class Movable : MonoBehaviour
{

    
    private bool _hasToMove = false;
    private Vector3 vectorToMove;
    private float powerToMove;
    public bool IsMovable = true;

    public int MovedByPlayer;


    private Player _player;

    
	// Use this for initialization
	void Start () {
        _player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (GetComponent<Rigidbody>() == null) return;

        if (_hasToMove)
        {
            //GetComponent<Rigidbody>().velocity = vectorToMove * powerToMove;
            GetComponent<Rigidbody>().AddForce(vectorToMove*powerToMove, ForceMode.VelocityChange);
            _hasToMove = false;
        }
        // no longer moving
        if (GetComponent<Rigidbody>().velocity.sqrMagnitude < 0.01f)
             MovedByPlayer = -1;
    }

    [RPC]
    public void MovePlayerTowards(int actuator, int target, Vector3 vector3, float power = 1)
    {
        
        Debug.Log("MovePLayerTowards: " + actuator + "target:" + target);
        if (_player == null) return;
        
        if (_player.Id == target)
        {
            GuiGame.DmgTaken();
            MoveTowards(actuator, vector3, power);
        }
    }
    
    public void MoveTowards(int actuator, Vector3 vector3, float power = 1)
    {
        MovedByPlayer = actuator;
        vectorToMove = vector3;
        powerToMove = power;
        _hasToMove = true;
        Debug.Log("Moved towards: " + vector3 + " " + MovedByPlayer);
//            rigidbody.AddForce(vector3 * power * Time.deltaTime);
    
    }
}
