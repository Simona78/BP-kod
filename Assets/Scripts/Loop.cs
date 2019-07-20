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

    public int loopCount = 0;
    public float subloop = 1.0f;
   

    private bool existingMessate(GameObject tmp_obj)
    {
        bool exist = false;

        int size = loopObjects.Count;

        for (int i = 0; i < size; i++)
        {
            Message label = loopObjects[i].GetComponent<Message>();

            if (label.name == tmp_obj.GetComponent<Message>().name && label.line == tmp_obj.GetComponent<Message>().line)
            {
                exist = true;
            }
        }

        return exist;
    }

    public void addLoopObject(GameObject obj)
    {
        bool existMessage = this.existingMessate(obj);

        if (!existMessage)
        {
            if (!obj.GetComponent<Message>().inserted)
            {
                this.loopObjects.Add(obj);

                obj.GetComponent<Message>().inserted = true;

                this.updateLoop();
            }
           
        }
        else
        {
            Destroy(obj);
        }


        this.loopCount++;
        this.updateLoop();
    }

    public void setBounds(IList tmp_bounds)
    {
        this.loopBoundaries = tmp_bounds;

        bool isCondition = false;

        SequenceDiagram seqDiagram = transform.parent.parent.GetComponent<SequenceDiagram>();

        foreach(IList item in seqDiagram.getConditions())
        {
            int topBound = int.Parse(item[0].ToString());
            int bottomBound = int.Parse(item[1].ToString());

            if(int.Parse(this.loopBoundaries[0].ToString()) == topBound && int.Parse(this.loopBoundaries[1].ToString()) == bottomBound)
            {
                isCondition = true;
            }
        }

        if (isCondition)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Opt";
        }
    }

    public IList getBounds()
    {
        return this.loopBoundaries;
    }

    void updateLoop()
    {
        List<GameObject> loopObjectsAll = new List<GameObject>();
        
        loopObjectsAll.Clear();

        loopObjects.ForEach((item) =>
        {
            loopObjectsAll.Add(item);
        });
        

        if (transform.parent.parent.GetComponent<SequenceDiagram>().loopsListAll.Count > 0)
        {
            foreach(GameObject item in transform.parent.parent.GetComponent<SequenceDiagram>().loopsListAll)
            {
                IList itemBounds = item.GetComponent<Loop>().getBounds();

                if (    ((System.Convert.ToInt32(itemBounds[0].ToString()) != System.Convert.ToInt32(this.getBounds()[0].ToString()))
                    &&   (System.Convert.ToInt32(itemBounds[1].ToString()) != System.Convert.ToInt32(this.getBounds()[1].ToString())))
                
                    && 
                       ((System.Convert.ToInt32(itemBounds[0].ToString()) >= System.Convert.ToInt32(this.getBounds()[0].ToString())
                    &&  (System.Convert.ToInt32(itemBounds[1].ToString()) <= System.Convert.ToInt32(this.getBounds()[1].ToString())))))
                {

                    foreach( GameObject subItem in item.GetComponent<Loop>().loopObjects)
                    {
                        loopObjectsAll.Add(subItem);
                    }
                }
            }
        }
        

        if (loopObjectsAll.Count > 0)
        {
            transform.gameObject.SetActive(true);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }

        int size = loopObjectsAll.Count;

        this.startLeft = 1000;
        this.startTop = 1000;
        this.startWidth = -1000;
      
        for (int i = 0; i < size; i++)
        {
            RectTransform objTransform = loopObjectsAll[i].GetComponent<RectTransform>().Find("line").GetComponent<RectTransform>();

            if(objTransform.position.x  < this.startLeft)
            {
                this.startLeft = objTransform.position.x;
            }

            if (objTransform.sizeDelta.x > this.startWidth)
            {
                this.startWidth = objTransform.sizeDelta.x;
            }

            if (objTransform.parent.GetComponent<Message>().getPosition().y < this.startTop)
            {
                this.startTop = objTransform.parent.GetComponent<Message>().getPosition().y;
            }
        }
        transform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.startWidth, (loopObjectsAll.Count) * 60);  
    }

    void Start()
    {
        transform.GetComponent<Canvas>().sortingOrder++;
        transform.GetComponent<Canvas>().sortingOrder--;

        transform.GetComponent<Canvas>().overrideSorting = true;


       // transform.GetComponent<RectTransform>().localScale = new Vector3((float)this.subloop, (float)this.subloop, (float)this.subloop);

      
  
    }


    // decrease level of loop
    public void decreaseLevel() {
        this.subloop -= 0.1f;
    }
	
	// Update is called once per frame
	void Update () {


        this.updateLoop();



        float spacingX = 10; 
        float spacingY = 0; 

        if (subloop != 1.0f)
        {
            transform.position = new Vector3(this.startLeft, this.startTop, 10);
        }
        else {
            transform.position = new Vector3(this.startLeft, this.startTop, 10);
        }






        float subloopX = 0;


        float x = transform.gameObject.GetComponent<RectTransform>().sizeDelta.x; // get diagram width
        float y = transform.gameObject.GetComponent<RectTransform>().sizeDelta.y; // get diagram height

        if (subloop != 1.0f)
        {
            subloopX = (transform.GetComponent<RectTransform>().sizeDelta.x * (1 - this.subloop))/2 + 3;
        }

        float posX = transform.gameObject.GetComponent<RectTransform>().position.x ; // get diagram width
        float posY = transform.gameObject.GetComponent<RectTransform>().position.y; // get diagram height

        var line = transform.gameObject.GetComponent<LineRenderer>();

        line.SetPosition(0, new Vector3(-spacingX + subloopX, -spacingY, -10));
        line.SetPosition(1, new Vector3(x + spacingX + subloopX, -spacingY, -10));
        line.SetPosition(2, new Vector3(x + spacingX + subloopX, y + spacingY, -10));
        line.SetPosition(3, new Vector3(-spacingX + subloopX, y + spacingY, -10));
        line.SetPosition(4, new Vector3(-spacingX + subloopX, -spacingY, -10));

        //setting label border

        line.SetPosition(5, new Vector3(-spacingX + subloopX, y + spacingY - 25, -10));
        line.SetPosition(6, new Vector3(50+ subloopX, y + spacingY - 25, -10));
        line.SetPosition(7, new Vector3(60+ subloopX, y + spacingY - 15, -10));
        line.SetPosition(8, new Vector3(60+ subloopX, y + spacingY, -10));

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
        
        line.startWidth = 3;
        line.endWidth = 3;


        if (this.subloop < 1.0f)
        {
            Debug.LogError("SCALING"+ (1.0f - this.subloop)+ "  " + this.subloop);

            transform.gameObject.GetComponent<RectTransform>().localScale = new Vector3( this.subloop,  1,1f);
        }
    }
}
