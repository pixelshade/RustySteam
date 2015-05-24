using UnityEngine;
using System.Collections;

public class Movable : MonoBehaviour
{

    
    private bool _hasToMove = false;
    private Vector3 vectorToMove;
    private float powerToMove;
    public bool IsMovable = true;

    public int MovedByPlayer;
    public int Id;


    private Player _player;
    private Rigidbody _rigidbody;
    private float _tagCD;
    
	// Use this for initialization
	void Start () {
        _player = GetComponent<Player>();
	    _rigidbody = GetComponent<Rigidbody>();
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
            _rigidbody.AddForce(vectorToMove*powerToMove, ForceMode.VelocityChange);
            _hasToMove = false;
        }
        // no longer moving
        if (_tagCD > 0.0f)
            _tagCD -= Time.deltaTime;
        if (_tagCD <= 0.0f) {
            MovedByPlayer = -1;
        }
    }
    
    public void MoveTowards(int actuator, Vector3 vector3, float power = 1)
    {
        MovedByPlayer = actuator;
        _tagCD = 20.0f;
        vectorToMove = vector3;
        powerToMove = power;
        _hasToMove = true;
        //Debug.Log("Moved towards: " + vector3 + " " + MovedByPlayer);
//            rigidbody.AddForce(vector3 * power * Time.deltaTime);
    
    }
}
