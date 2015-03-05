using UnityEngine;
using System.Collections;

public class Movable : MonoBehaviour
{

    
    private bool _hasToMove = false;
    private Vector3 vectorToMove;
    private float powerToMove;
    public bool IsMovable = true;


    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        if (GetComponent<Rigidbody>() == null) return;

        if (_hasToMove)
        {
            GetComponent<Rigidbody>().velocity = vectorToMove*powerToMove;
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
