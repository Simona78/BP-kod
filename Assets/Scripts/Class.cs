using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;



public class Class : MonoBehaviour {

    private GameObject diagram;
    private Transform lifeline;
    private UILineRenderer lifelineRenderer;
    private GameObject tmp;

    

	// Use this for initialization
	void Start () {
        tmp = GameObject.Find("TCPService");
        Debug.Log("TEST: " + tmp.GetComponent<TCPService>().getDiagCount());


        diagram = GameObject.Find("#"+ (tmp.GetComponent<TCPService>().getDiagCount()-1));
        lifeline = transform.Find("lifeline");
        lifelineRenderer = lifeline.GetComponent<UILineRenderer>();
	}

    // Update is called once per frame
    void Update() {

       // Debug.Log(diagram.name);
        
        var csize = lifeline.parent.GetComponent<RectTransform>().sizeDelta;
        var toX = csize.x * 0.5f;
        var toY = diagram.GetComponent<RectTransform>().sizeDelta.y;

        lifelineRenderer.Points[0] = new Vector2(toX, -csize.y);
        lifelineRenderer.Points[1] = new Vector2(toX, -toY);
        lifelineRenderer.SetVerticesDirty();
	}
}
