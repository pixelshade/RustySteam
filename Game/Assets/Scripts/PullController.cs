using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PullController : MonoBehaviour
{

    public float Power = 10;
    public int Range = 100;
    public float CD = 0.5f;

    private float _cdLeft = 0.0f;
    private GuiGame _guiGame;


    void Awake()
    {
        
    }

	// Use this for initialization
	void Start ()
	{
	    _guiGame = GetComponent<GuiGame>();
	}

    private void ProcessLeftClick(Ray ray, RaycastHit hit, Vector3 fwd)
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.GetComponent<FPSRigidController>().grounded = false;
            if (_cdLeft > 0)
            {
              
                return;
            }
            if (Physics.Raycast(ray, out hit, Range))
            {
                var hitObj = hit.transform.GetComponent<Movable>();
                if (hit.rigidbody != null && hitObj!=null)
                {
                    var direction = hit.transform.position - transform.position;
                    hitObj.MoveTowards(direction.normalized, Power);
                }
                else
                {

                    var self = transform.GetComponent<Movable>();
                    self.MoveTowards(fwd.normalized, -Power);
                }
            
            }

        }
    }


    private void ProcessRightClick(Ray ray, RaycastHit hit, Vector3 fwd)
    {
        

        if (Input.GetMouseButtonDown(1))
        {
            transform.GetComponent<FPSRigidController>().grounded = false;
            if (Physics.Raycast(ray, out hit, Range))
            {
                var hitObj = hit.transform.GetComponent<Movable>();
                if (hit.rigidbody != null && hitObj != null)
                {
                    var direction = hit.transform.position - transform.position;
                    hitObj.MoveTowards(direction.normalized, -Power);
                }
                else
                {
                    var self = transform.GetComponent<Movable>();
                    //var direction = hit.transform.position - transform.position;
                    self.MoveTowards(fwd.normalized, Power);
                }
            
            }
        }

    }

    private void ProcessActions(Ray ray, RaycastHit hit)
    {
        if(Input.GetButton("Levitate") || Input.GetKey("e"))
        {
            if (hit.transform != null && hit.transform.GetComponent<Movable>() != null && hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(0,Power,0);
            }
        }
    }
	
	// Update is called once per frame
    private void Update()
    {

        // Cooldown 
        if (_cdLeft > 0)
        {
            _cdLeft -= Time.deltaTime;
        }
        else if (_cdLeft < 0)
        {
            _cdLeft = 0;
        }


        Power += Input.GetAxis("Mouse ScrollWheel")*10;

        Vector3 cam = transform.Find("Camera").TransformDirection(Vector3.forward);
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        fwd.y = cam.y;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //chage crosshair if we have something hit 
        _guiGame.CrosshairSelected(Physics.Raycast(ray, out hit, Range));
        

        ProcessLeftClick(ray, hit, fwd);
        ProcessRightClick(ray, hit, fwd);
        ProcessActions(ray, hit);



//	    print("There is something in front of the object!");
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;
//        if (Physics.Raycast(ray, out hit, 100))
//            Debug.DrawLine(ray.origin, hit.point);
    }

    public float CdLeft()
    {
        return _cdLeft;
    }


}
