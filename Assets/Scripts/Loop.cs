using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Loop : MonoBehaviour {

    private List<GameObject> loopObjects = new List<GameObject>();
    private IList loopBoundaries; // loop  start and end line

    private float startLeft = 0;
    private float startTop = 0;

    private float startWidth = 0;
    private float startHeight = 0;
   

    private bool existingMessate(GameObject tmp_obj)
    {
        bool exist = false;

        int size = loopObjects.Count;

        Debug.LogWarning("Comapre object length:" + size);

        for(int i = 0; i < size; i++)
        {
            Debug.LogError("Comapre object start" );

            Message label = loopObjects[i].GetComponent<Message>();

            if (label.name == tmp_obj.GetComponent<Message>().name && label.line == tmp_obj.GetComponent<Message>().line)
            {
                exist = true;
            }

            Debug.LogError("Comapre object:" + label.name+"|"+ tmp_obj.GetComponent<Message>().name+ "--VS--"+label.line+"|"+tmp_obj.GetComponent<Message>().line);
        }

        return exist;
    }

    public void addLoopObject(GameObject obj)
    {
            
        Debug.LogWarning("ADD OBJEC");

        bool existMessage = this.existingMessate(obj);




        if (!existMessage)
        {
            this.loopObjects.Add(obj);
            this.updateLoop();
        }
        else
        {
            Destroy(obj);
        }
        



        this.updateLoop();
    }

    public void setBounds(IList tmp_bounds)
    {
        this.loopBoundaries = tmp_bounds;
    }

    public IList getBounds()
    {
        return this.loopBoundaries;
    }

    void updateLoop()
    {
        Debug.LogError(this.loopObjects.Count);

        if (this.loopObjects.Count > 0)
        {
            transform.gameObject.SetActive(true);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
        int pom = 0;

        int size = this.loopObjects.Count;

        for (int i = 0; i < size; i++)
        {
            RectTransform objTransform = loopObjects[i].GetComponent<RectTransform>().Find("line").GetComponent<RectTransform>();

            Debug.LogWarning("LOOP DIMENSIONS FOREACH OBJ: " + objTransform+" "+ objTransform.parent.GetComponent<RectTransform>().position);
            Debug.LogWarning("LOOP DIMENSIONS FOREACH: " + objTransform.position );

            if (pom == 0)
            {
                this.startLeft = objTransform.position.x;
                this.startTop = objTransform.position.y;
            }
            

            if(objTransform.position.x  < this.startLeft)
            {
                this.startLeft = objTransform.position.x;
            }

            if (objTransform.position.y < this.startTop)
            {
                this.startTop = objTransform.position.y;
            }

            if (objTransform.position.x + objTransform.sizeDelta.x > this.startLeft)
            {
                this.startWidth = objTransform.position.x + objTransform.sizeDelta.x;
            }

            if (objTransform.position.y + objTransform.sizeDelta.y > this.startHeight)
            {
                this.startHeight = objTransform.position.y + objTransform.sizeDelta.y;
            }
        }


        Debug.LogWarning("LOOP DIMENSIONS: " + this.startTop + " " + this.startLeft);


        transform.position = new Vector3(this.startLeft, this.startTop, 10);


    }

    void Start()
    {
        transform.GetComponent<Canvas>().sortingOrder++;
        transform.GetComponent<Canvas>().sortingOrder--;

        transform.GetComponent<Canvas>().overrideSorting = true;    
    }

        
	
	// Update is called once per frame
	void Update () {

       // this.updateLoop();

        transform.position = new Vector3(this.startLeft, this.startTop, 10);

        int spacing = 25;

        float x = transform.gameObject.GetComponent<RectTransform>().sizeDelta.x; // get diagram width
        float y = transform.gameObject.GetComponent<RectTransform>().sizeDelta.y; // get diagram height
        
        float posX = transform.gameObject.GetComponent<RectTransform>().position.x; // get diagram width
        float posY = transform.gameObject.GetComponent<RectTransform>().position.y; // get diagram height


        //float x = this.startWidth;
        //float y = this.startHeight;
        //
        //float posX = this.startLeft;
        //float posY = this.startTop;
        //draw lines by calculatin their positions 

        //Debug.LogWarning(transform.gameObject);
        //
        //var line = transform.gameObject.GetComponent<LineRenderer>();
        //
        //line.SetPosition(0, new Vector3(-spacing, -spacing, -10));
        //line.SetPosition(1, new Vector3(x + spacing , -spacing, -10));
        //line.SetPosition(2, new Vector3(x + spacing, y + spacing, -10));
        //line.SetPosition(3, new Vector3(-spacing , y + spacing, -10));
        //line.SetPosition(4, new Vector3(-spacing, -spacing, -10));
        //
        //line.material = new Material(Shader.Find("Sprites/Default"));
        //line.startColor = Color.white;
        //line.endColor = Color.white;
        //
        //line.startWidth = 1;
        //line.endWidth = 1;
    }
}
