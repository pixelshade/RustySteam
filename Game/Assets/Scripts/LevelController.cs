using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    private const int NUM_CUBES = 500;
    private List<GameObject> cubes = new List<GameObject>(); 

	// Use this for initialization
	void Start () {
        var cube = Resources.Load("Cube", typeof(GameObject)) as GameObject;
        for (var i = 0; i < NUM_CUBES; i++)
        {
            
            var point = Random.insideUnitSphere * 50;
            point.y = Math.Abs(point.y);
            
            
            if (Consts.IsSinglePlayer)
            {
                var c = Instantiate(cube, point, Quaternion.identity) as GameObject;
                c.transform.localScale += new Vector3(Random.value, Random.value, Random.value)*10;
                c.renderer.material.color = new Color(Random.value,Random.value,Random.value);
                cubes.Add(c);
            }
            else
            {
                 cubes.Add(Network.Instantiate(cube, point, Quaternion.identity, 0) as GameObject);
            }    
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
