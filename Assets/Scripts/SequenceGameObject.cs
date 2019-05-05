using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceGameObject : MonoBehaviour {

    GameObject diagram;

    // Use this for initialization
    void Start () {
        int id = GameObject.Find("TCPService").GetComponent<TCPService>().getDiagCount();



        diagram = new GameObject("TEST: "+id.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
