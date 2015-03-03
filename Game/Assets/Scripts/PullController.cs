using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PullController : MonoBehaviour
{

    public float Power = 10;
    public int Range = 100;
    public Text GuiText;
    public Image Crosshair;

	// Use this for initialization
	void Start () {
	    
	}




    private void ProcessLeftClick(Ray ray, RaycastHit hit, Vector3 fwd)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Range))
            {
                if (hit.rigidbody != null && hit.rigidbody.CompareTag("movable"))
                {
                    var hitObj = hit.transform.GetComponent<Movable>();
                    var direction = hit.transform.position - transform.position;
                    hitObj.MoveTowards(direction.normalized, Power);
                }
                else
                {

                    var self = transform.GetComponent<Movable>();
                    self.MoveTowards(fwd.normalized, -Power);
                }
                Debug.DrawLine(transform.position, hit.point);
            }

        }
    }


    private void ProcessRightClick(Ray ray, RaycastHit hit)
    {
        

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit, Range))
            {
                if (hit.rigidbody != null && hit.rigidbody.CompareTag("movable"))
                {

                    var hitObj = hit.transform.GetComponent<Movable>();
                    var direction = hit.transform.position - transform.position;
                    hitObj.MoveTowards(direction.normalized, -Power);
                }
                else
                {
                    var self = transform.GetComponent<Movable>();
                    var direction = hit.transform.position - transform.position;
                    self.MoveTowards(direction.normalized, Power);
                }
                Debug.DrawLine(transform.position, hit.point);
            }
        }

    }

    private void ProcessActions(Ray ray, RaycastHit hit)
    {
        if(Input.GetButton("Levitate") || Input.GetKey("e"))
        {
            Debug.Log("pressed");
            if (hit.transform.tag.Equals("movable") && hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(0,Power,0);
            }
        }
    }
	
	// Update is called once per frame
    private void Update()
    {
        
        Power += Input.GetAxis("Mouse ScrollWheel")*10;
        GuiText.text = "Power: " + Power;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        Crosshair.color = !Physics.Raycast(ray, out hit, Range) ? Color.red : Color.black;

        ProcessLeftClick(ray, hit,fwd);
        ProcessRightClick(ray, hit);
        ProcessActions(ray, hit);



//	    print("There is something in front of the object!");
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;
//        if (Physics.Raycast(ray, out hit, 100))
//            Debug.DrawLine(ray.origin, hit.point);
    }


}
