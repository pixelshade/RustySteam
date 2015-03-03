using UnityEngine;
using System.Collections;

public class Movable : MonoBehaviour
{

    private bool _isMoving = false;
    private bool _hasToMove = false;
    private Vector3 vectorToMove;
    private float powerToMove;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (rigidbody == null) return;
        if(rigidbody.velocity == Vector3.zero)
            _isMoving = false;

        if (_hasToMove)
        {
            rigidbody.velocity = vectorToMove*powerToMove;
            _hasToMove = false;
        }
    }

    public void MoveTowards(Vector3 vector3, float power = 1)
    {
        vectorToMove = vector3;
        powerToMove = power;
        _hasToMove = true;
//            rigidbody.AddForce(vector3 * power * Time.deltaTime);
    
    }
}
