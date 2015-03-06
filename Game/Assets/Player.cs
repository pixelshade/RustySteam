using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{

    public int HP = 100;

    public bool Stunned = false;
    public bool HasFlag = false;
    public GameObject FlagGameObject;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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

            Respawn(5);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("sila" + collision.relativeVelocity.magnitude);
        //        Debug.Log("moja" + GetComponent<Rigidbody>().velocity.magnitude);
        if (collision.relativeVelocity.magnitude > 50)
            HP -= 10;

        var flag = collision.transform.GetComponent<FlagController>();
        if (flag != null)
        {
            Debug.Log("vlajka!");
            HasFlag = true;
            Destroy(collision.gameObject);
        }

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

    public IEnumerator Respawn(float time = 0)
    {
        var gameGui = GetComponent<GuiGame>();
        gameGui.RespawnIn(time);
        var position = Random.insideUnitSphere * 500;
        position.y = 1;
        yield return new  WaitForSeconds(time);
        Respawn(position);
        
    }

    public IEnumerator Respawn(Vector3 position ,float time = 0)
    {
        yield return new WaitForSeconds(time);
        transform.position = position;
    }


    public bool IsAlive()
    {
        return HP > 0;
    }
}
