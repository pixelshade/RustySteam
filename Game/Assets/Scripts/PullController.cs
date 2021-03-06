﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PullController : MonoBehaviour
{

    public float Power = 10;
    public int Range = 100;
    public float CD = 0.5f;
    public float Stun = 1.0f;
    public float EnemyKoeficient = 2.0f;

    private float _cdLeft = 0.0f;
    private float _stunLeft = 0.0f;
    private GuiGame _guiGame;
    private LevelController _levelController;
    private RopeScript _ropeScript;
    private NetworkView _networkView;
    private Player _playerSelf;
    private AudioSource[] _audioSources;

    public GameObject SpawnedBlock;
    public ParticleSystem ShootParticleSystem;
    public ParticleSystem SuckParticleSystem;

    public AudioClip SuckAudioClip;
    public AudioClip ShootAudioClip;


    void Awake()
    {
        
    }

	// Use this for initialization
	void Start ()
	{
	    _guiGame = GetComponent<GuiGame>();
	    _ropeScript = GetComponent<RopeScript>();
	    _networkView = GetComponent<NetworkView>();
	    _playerSelf = GetComponent<Player>();
        _levelController = LevelController.Get();
	    _audioSources = GetComponents<AudioSource>();
        
        ShootParticleSystem.Stop();
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
                //int playerIndex = NetworkManager.Get().GetPosition(false);
                int playerIndex = _playerSelf.Id;
                if (hit.rigidbody != null && hitObj != null)
                {
                    var hitPlayer = hit.transform.GetComponent<Player>();
                    var direction = hit.transform.position - transform.position;

                    if (hitPlayer != null)
                    {
                        var targetIndex = hitPlayer.Id;
                        //Debug.Log(targetIndex+" indexy "+playerIndex);
                        GetComponent<NetworkView>().RPC("MovePlayerTowards", RPCMode.AllBuffered, playerIndex, targetIndex, direction.normalized, EnemyKoeficient * Power);
                    }
                    else
                    {
                        var objIndex = hitObj.Id;
                        //Debug.Log(objIndex + " Objindexy " + playerIndex);
                        GetComponent<NetworkView>().RPC("MovePlayerTowards", RPCMode.AllBuffered, playerIndex, objIndex, direction.normalized, EnemyKoeficient * Power);
                        //hitObj.MoveTowards(playerIndex, direction.normalized, Power);
                    }
                }
                else
                {
                    var self = transform.GetComponent<Movable>();
                    self.MoveTowards(playerIndex, fwd.normalized, -Power);
                }
            
            }

            PlayerShotAnimate(false);
            _networkView.RPC("PlayerShootsAnimate",RPCMode.OthersBuffered,_playerSelf.Id,false);
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
                var hitObj = hit.transform.GetComponent<Movable>();
//                int playerIndex = NetworkManager.Get().GetPosition(false);
                int playerIndex = _playerSelf.Id;

                if (hit.rigidbody != null && hitObj != null)
                {
//                    _ropeScript.BuildRope(hit.transform, false); 
                    var hitPlayer = hit.transform.GetComponent<Player>();
                    var direction = hit.transform.position - transform.position;
            
                    if (hitPlayer != null)
                    {
                        var targetIndex = hitPlayer.Id;
                        //Debug.Log(targetIndex + " indexy " + playerIndex);
                        GetComponent<NetworkView>().RPC("MovePlayerTowards", RPCMode.AllBuffered, playerIndex, targetIndex, direction.normalized, EnemyKoeficient * -Power);
                    }
                    else
                    {
                        var objIndex = hitObj.Id;
                        //Debug.Log(objIndex + " Objindexy " + playerIndex);
                        GetComponent<NetworkView>().RPC("MovePlayerTowards", RPCMode.AllBuffered, playerIndex, objIndex, direction.normalized, EnemyKoeficient * -Power);
                        //hitObj.MoveTowards(playerIndex, direction.normalized, -Power);
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
            PlayerShotAnimate(true);
            _networkView.RPC("PlayerShootsAnimate", RPCMode.OthersBuffered, _playerSelf.Id, true);
            _cdLeft = CD;
        }
        else
        {
//            _ropeScript.DestroyRope();
        }

    }


    [RPC]
    public void SendBlockToPosition(int blockId, Vector3 position)
    {
        var movable = GetMovableWithId(blockId);
        if (movable == null) return;
        movable.transform.position = position;
        movable.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }


    private void ProcessActions(Ray ray, RaycastHit hit)
    {
        if (Input.GetButtonDown("SpawnBlock"))
        {
            var count = _levelController.MovableGameObjects.Count;
            if (count > 0)
            {
                int rnd = Random.Range(0, count);
                var blockId = _levelController.MovableGameObjects[rnd].GetComponent<Movable>().Id;

                var playerPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                var playerDirection = transform.forward;
                const float spawnDistance = 5;

                var pos = playerPos + playerDirection*spawnDistance;
                _networkView.RPC("SendBlockToPosition", RPCMode.AllBuffered, blockId, pos);
                _cdLeft = CD;
            }
        } 

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

    public Movable GetMovableWithId(int movableId)
    {
        foreach (var targetGO in _levelController.MovableGameObjects)
        {
            var movable = targetGO.GetComponent<Movable>();
            if (movable.Id == movableId)
            {
                return movable;
            }
        }
        return null;
    }

    [RPC]
    public void MovePlayerTowards(int actuator, int target, Vector3 vector3, float power = 1)
    {
        if (target < 0)
        {
            var movable = GetMovableWithId(target);
            movable.MoveTowards(actuator, vector3, power);
        }
        else
        {
            foreach (var playerGO in _levelController.PlayersGameObjects)
            {
                if (playerGO.GetComponent<Player>().Id == target)
                {
                    playerGO.GetComponent<Movable>().MoveTowards(actuator, vector3, power);
                    if (NetworkManager.Get().GetPosition(false) == target)
                        _guiGame.PlayTakeDamageAnimation();
                }

            }
        }
    }


    [RPC]
    public void PlayerShootsAnimate(int playerId, bool suckAnimation)
    {
        Debug.Log("[RPC]PlayerShootsAnimate("+playerId+","+suckAnimation+")");
        foreach (var playerGO in _levelController.PlayersGameObjects)
        {
            
            if (playerGO.GetComponent<Player>().Id == playerId)
            {
                playerGO.GetComponent<PullController>().PlayerShotAnimate(suckAnimation);
            }
            
        }
        
    }

    private void PlayerShotAnimate(bool suckAnimation)
    {
        if (suckAnimation)
        {
            _audioSources[0].PlayOneShot(SuckAudioClip);
            SuckParticleSystem.Play();
        }
        else
        {
            _audioSources[0].PlayOneShot(ShootAudioClip);
            ShootParticleSystem.Play();
        }
        
    }
}
