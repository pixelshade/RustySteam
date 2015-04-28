using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PullController : MonoBehaviour
{

    public float Power = 10;
    public int Range = 100;
    public float CD = 0.5f;
    public float Stun = 1.0f;

    private float _cdLeft = 0.0f;
    private float _stunLeft = 0.0f;
    private GuiGame _guiGame;
    private RopeScript _ropeScript;
    private NetworkView _networkView;

    void Awake()
    {
        
    }

	// Use this for initialization
	void Start ()
	{
	    _guiGame = GetComponent<GuiGame>();
	    _ropeScript = GetComponent<RopeScript>();
	    _networkView = GetComponent<NetworkView>();
	}

    private void ProcessLeftClick(Ray ray, RaycastHit hit, Vector3 fwd)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (_cdLeft > 0)
            {
                return;
            }
            if (Physics.Raycast(ray, out hit, Range))
            {
                var hitObj = hit.transform.GetComponent<Movable>();
                int playerIndex = NetworkManager.Get().GetPosition(false);
                if (hit.rigidbody != null && hitObj != null)
                {
                    var player = hit.transform.GetComponent<Player>();
                    var direction = hit.transform.position - transform.position;

                    if (player != null)
                    {
                        var targetIndex = player.Id;
                        Debug.Log(targetIndex+" indexy "+playerIndex);
                        GetComponent<NetworkView>().RPC("MovePlayerTowards", RPCMode.AllBuffered, playerIndex, targetIndex, direction.normalized, Power);
                        //if (player.Team != transform.GetComponent<Player>().Team)
                        //{
                        //player.Stun(Stun);
                        //}
                    }
                    else
                    {
                        hitObj.MoveTowards(playerIndex, direction.normalized, Power);
                    }
                }
                else
                {
                    var self = transform.GetComponent<Movable>();
                    self.MoveTowards(playerIndex, fwd.normalized, -Power);
                }
            
            }
            _cdLeft = CD;
        }
    }


    private void ProcessRightClick(Ray ray, RaycastHit hit, Vector3 fwd)
    {

        if (Input.GetButtonDown("Fire2"))
        {
            if (_cdLeft > 0)
            {
                return;
            }
            if (Physics.Raycast(ray, out hit, Range))
            {
                Debug.Log(hit);
                
                var hitObj = hit.transform.GetComponent<Movable>();
                int playerIndex = NetworkManager.Get().GetPosition(false);
                if (hit.rigidbody != null && hitObj != null)
                {
//                    _ropeScript.BuildRope(hit.transform, false); 
                    var player = hit.transform.GetComponent<Player>();
                    var direction = hit.transform.position - transform.position;
            
                    if (player != null)
                    {
                        var targetIndex = player.Id;
                        Debug.Log(targetIndex + " indexy " + playerIndex);
                        GetComponent<NetworkView>().RPC("MovePlayerTowards", RPCMode.AllBuffered, playerIndex, targetIndex, direction.normalized, Power);
                        //if (player.Team != transform.GetComponent<Player>().Team)
                        //{
                        //    player.Stun(Stun);
                        //}
                    }
                    else
                    {
                        hitObj.MoveTowards(playerIndex, direction.normalized, -Power);
                    }
                }
                else
                {
//                    _ropeScript.BuildRope(hit.transform, true);
                    var self = transform.GetComponent<Movable>();
                    //var direction = hit.transform.position - transform.position;
                    self.MoveTowards(playerIndex, fwd.normalized, Power);
                }
            }
           
            _cdLeft = CD;
        }
        else
        {
//            _ropeScript.DestroyRope();
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
        if (!_networkView.isMine) return;
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
        
        if (Camera.main != null && Camera.main.enabled)
        {
            // Direction of shot depends on rotation of player and his camera
            var c = Camera.main.transform.TransformDirection(Vector3.forward);
            Vector3 cam = transform.Find("Camera").TransformDirection(Vector3.forward);
//            Debug.Log("main"+c);
//            Debug.Log("find" + cam);
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            fwd.x -= fwd.x*Math.Abs(cam.y);
            fwd.y = cam.y;
            fwd.z -= fwd.z*Math.Abs(cam.y);


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));

            //chage crosshair if we have something hit 
            _guiGame.CrosshairSelected(Physics.Raycast(ray, out hit, Range));


            


            ProcessLeftClick(ray, hit, fwd);
            ProcessRightClick(ray, hit, fwd);
            ProcessActions(ray, hit);
        }


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
