using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var parent = transform.parent;

        var sizeX = parent.GetComponent<RectTransform>().sizeDelta.x;
        var sizeY = parent.GetComponent<RectTransform>().sizeDelta.y;

        if (sizeX == 0 && sizeY == 20)
        {
            transform.localScale = new Vector3(0,0, 1);
        }
        else
        {
            transform.localScale = new Vector3(sizeX + 25, sizeY + 25, 1);
        }
       
	}
}
