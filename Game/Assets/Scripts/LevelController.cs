using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    private const int NUM_CUBES = 50;
    private List<GameObject> cubes = new List<GameObject>(); 

	// Use this for initialization
	void Start () {
        var cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;
        for (var i = 0; i < NUM_CUBES; i++)
        {
            
            var point = Random.insideUnitSphere * 500;
            point.y = Math.Abs(point.y);

            GameObject c;
            if (Consts.IsSinglePlayer)
            {
                c = Instantiate(cube, point, Quaternion.identity) as GameObject;
                
            }
            else
            {
                c = Network.Instantiate(cube, point, Quaternion.identity, 0) as GameObject;
            }
            c.transform.localScale += new Vector3(Random.value, Random.value, Random.value) * 10;
            c.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);

            // We will create some stable blocks 
            if (Random.value > 0.5)
            {
                c.GetComponent<Renderer>().material.color = new Color(0.8f,0.8f,0.8f);
                c.GetComponent<Rigidbody>().isKinematic = true;
                c.transform.localScale += c.transform.localScale * 10;
                c.transform.position = new Vector3(c.transform.position.x, 0, c.transform.position.z);
                Destroy(c.GetComponent<Movable>());
            }
            cubes.Add(c);
        }

	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
