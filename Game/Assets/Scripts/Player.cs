﻿
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public int StartHP = 100;
    
    public AudioClip OuchAudioClip;
    public GameObject TeamColorGameObject;

    public int Id = -1;
   

    public bool Dead = false;
    public bool Stunned = false;
    public bool HasFlag = false;

    private GuiGame _gameGui;

    private bool _isMine;
    private AudioSource _audioSource;


    public int HP
    {
        get { return _hp; }
        set
        {
            var PlayerHpText = GameObject.Find("HPText");
            if (PlayerHpText != null)
            {
                if(_isMine)
                    PlayerHpText.GetComponent<Text>().text = value.ToString();
            }
            _hp = value;
        }
    }

    private int _hp = 100;

    public TeamInfo Team
    {
        get
        {
            if(NetworkManager.Get().PlayerList.Count > Id)
                return NetworkManager.Get().PlayerList[Id].Team;
            Debug.LogError("Team id is unavailable.");
            return null;
        }

        set
        {
            if (NetworkManager.Get().PlayerList.Count > Id)
            {
                Debug.Log("Team WAS SET"+value.TeamName);
                NetworkManager.Get().PlayerList[Id].Team = value;
                SetTeamColor(value.Color);
            }
                
        }
    }

    public GameObject FlagGameObject;
    public List<NetworkManager.PlayerInfo> PlayerInfos;


    private int _killer;
    public int deathType;
    private float _stunLeft;

    public string NickName
    {
        get { return _nickName; } 
        set
        {
                _playerNameText = GetComponentInChildren<TextMesh>();
                _nickName = value;
                _playerNameText.text = value;
        }
    }
    private string _nickName;
    private TextMesh _playerNameText;
    // Use this for initialization
    void Start()
    {
        _gameGui = GetComponent<GuiGame>();
        _audioSource = GetComponents<AudioSource>()[0];

        _isMine = GetComponent<NetworkView>().isMine;

        if (_isMine)
        {
            Camera[] c = GetComponentsInChildren<Camera>();
            AudioListener[] al = GetComponentsInChildren<AudioListener>();
            c[0].enabled = true;
            al[0].enabled = true;
            Id = NetworkManager.Get().GetPosition(false);
        }
       
        _playerNameText = GetComponentInChildren<TextMesh>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Stun
        if (_stunLeft > 0)
        {
            _stunLeft -= Time.deltaTime;
        }
        else if (_stunLeft < 0)
        {
            _stunLeft = 0;
            Stunned = false;
        }

        // DEad
        if (HP <= 0)
        {
            HP = 0;
            if (HasFlag)
            {
                HasFlag = false;
                var pos = transform.position;
                pos.z += 2;
                var f = SpawnFlag(pos);

//                var rb = f.GetComponent<Rigidbody>();
//                rb.isKinematic = false;
//                rb.AddExplosionForce(2,transform.position,3);
                
            }
            
            // called once while dead
            
            if (!Dead)
            {
                var levelController = GameObject.Find("Main");
                var playerIndex = NetworkManager.Get().GetPosition(false);
                if (Consts.IsHost)
                    levelController.GetComponent<NetworkView>().RPC("PlayerKillEnemyWith", RPCMode.AllBuffered, _killer, Id, deathType);
                Respawn(5);
                Dead = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 50)
        {
            var movable = collision.gameObject.GetComponent<Movable>();
            TakeDamage(movable != null ? 100 : 10);
            if(HP <= 0)
                if (movable != null)
                {
                    _killer = movable.MovedByPlayer;
                    deathType = (int)DeathType.Hit;
                    }
        }

        var flag = collision.transform.GetComponent<FlagController>();
        if (flag != null)
        {
            if (flag.Team != Team) { 
                Debug.Log("vlajka!");
                HasFlag = true;
                if (Consts.IsSinglePlayer)
                    Destroy(collision.gameObject);
                else
                    Network.Destroy(collision.gameObject);
            }
        }

    }

    public void SetTeamColor(Color color)
    {
        TeamColorGameObject.GetComponent<Renderer>().material.color = color;
    }

    public GameObject SpawnFlag(Vector3 position)
    {
        var flag = Resources.Load("Prefabs/flag", typeof(GameObject)) as GameObject;
        GameObject f;
        if (Consts.IsSinglePlayer)
        {
            f = Instantiate(flag, position, Quaternion.identity) as GameObject;

        }
        else
        {
            f = Network.Instantiate(flag, position, Quaternion.identity, 0) as GameObject;
        }
        return f;
    }

    public void Respawn(float time = 0)
    {
        var fpsRigidCtrl = GetComponent<FPSRigidController>();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        fpsRigidCtrl.enabled = false;

        var gameGui = GetComponent<GuiGame>();
        Invoke("Respawn", time);
        gameGui.RespawnIn(time);
//        yield return new  WaitForSeconds(time);
    }

    private void Respawn()
    {
        var fpsRigidCtrl = GetComponent<FPSRigidController>();
        fpsRigidCtrl.enabled = true;
        Vector3 p;
        if (Team == LevelController.TeamA)
        {
            var spawns = GameObject.FindGameObjectsWithTag("SpawnA");
            var choice = Random.Range(0, spawns.Length);
            p = spawns[choice].transform.position;
        }
        else
        {
            var spawns = GameObject.FindGameObjectsWithTag("SpawnB");
            var choice = Random.Range(0, spawns.Length);
            p = spawns[choice].transform.position;
        }
        var position = new Vector3(p.x, 10, p.z);
        //position +=  Random.insideUnitSphere*(Random.Range(-100,100));
        //position.y = 2;

        HP = StartHP;

        Dead = false;
        
        transform.position = position;
        
    }

    public IEnumerator Respawn(Vector3 position ,float time = 0)
    {
        yield return new WaitForSeconds(time);
        transform.position = position;
    }

    public void Stun(float duration)
    {
        _stunLeft = duration;
        Stunned = true;
    }

    public bool IsAlive()
    {
        return HP > 0;
    }


    public void TakeDamage(int damage)
    {
        HP -= damage;
        _killer = GetComponent<Movable>().MovedByPlayer;
        _gameGui.PlayTakeDamageAnimation();
        _audioSource.PlayOneShot(OuchAudioClip);
    }
}
