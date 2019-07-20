using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SequenceDiagram : MonoBehaviour {

    public Transform target;
    public GameObject classPrefab;
    public GameObject messagePrefab;
    public GameObject loopPrefab; // arc

    private Transform classesTransform;
    private Transform messagesTransform;
    private Transform loopsTransform;

    private Dictionary<string, Transform> classesDict;

    private GameObject obj;

    private Boolean started = false;

    float[] topLeft = new float[3];
    float[] topRight = new float[3];
    float[] bottomLeft = new float[3];
    float[] bottomRight = new float[3];

    private IList<IList> loops = null; // loops rows 
    private IList<IList> conditions = null; // loops rows 
    private List<Loop> LoopList = null; // loops objects

    public List<GameObject> loopsListAll = new List<GameObject>();

    IEnumerator Test()
    {
        AddClass("test1");
        AddClass("test2");
        AddClass("test3");
        Canvas.ForceUpdateCanvases();
        AddMessage("test1", "test2", "bla()",5);
        AddMessage("test2", "test3", "bla()",5);
        AddMessage("test3", "test2", "bla()",5, true);
        yield return new WaitForSeconds(5);
        RemoveClass("test3");
        yield return new WaitForEndOfFrame();
        AddClass("test3");
        AddMessage("test2", "test3", "bla()",5, true);
    }

    private void Update()
    {
        this.refreshBounds();
    }

    private void Awake()
    {
        this.refreshInfo();
    }


    public IList<IList> getConditions()
    {
        return this.conditions;
     }


    public void setConditions(IList<IList> conditions_tmp)
    {
        Debug.Log("Adding loops: " + this.loops.Count);

        this.conditions = conditions_tmp;

        foreach(IList item in this.conditions)
        {
            this.loops.Add(item);
        }

        Debug.Log("Adding conditions: " + this.loops.Count);
      
    }

    public void setLoops(IList<IList> loops_tmp)
    {
        this.loops = loops_tmp;
    }

    public void printList()
    {

        try
        {
            foreach (var outer in this.loops)
            {
                foreach (var inner in outer)
                {
                    Debug.LogWarning(inner);
                }
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.Log(ex.Message);
        }
        
    }

    public IList<IList> getLoops()
    {
        return this.loops;
    }

    // set classes and messages prefab wrapper 
    void refreshInfo()
    {
        if (this.obj != null)
        {
            this.obj.GetComponent<CanvasGroup>().alpha = 1;

            classesTransform = this.obj.transform.Find("ClassesHL");
            messagesTransform = this.obj.transform.Find("MessagesVL");
            loopsTransform = this.obj.transform.Find("LoopsVL");
        }


        classesDict = new Dictionary<string, Transform>();
    }

    // Use this for initialization
    void Start () {
            
        this.obj = GameObject.Find("#"+ (GameObject.Find("TCPService").GetComponent<TCPService>().GetDiagCount()-1));


        this.refreshBounds();

        this.refreshInfo();
    }

    public void refreshBounds()
    {
        if (this.obj != null) {

            int spacing = 25;

            float x = this.obj.GetComponent<RectTransform>().sizeDelta.x; // get diagram width
            float y = this.obj.GetComponent<RectTransform>().sizeDelta.y; // get diagram height

            float posX = this.obj.GetComponent<RectTransform>().position.x; // get diagram width
            float posY = this.obj.GetComponent<RectTransform>().position.y; // get diagram height

            //draw lines by calculatin their positions 

            var line = this.obj.GetComponent<LineRenderer>();

            line.SetPosition(0, new Vector3(posX +  0            , posY +  y/2 + spacing, this.obj.GetComponent<RectTransform>().position.z));
            line.SetPosition(1, new Vector3(posX + -x/2 - spacing, posY +  y/2 + spacing, this.obj.GetComponent<RectTransform>().position.z));
            line.SetPosition(2, new Vector3(posX + -x/2 - spacing, posY + -y/2 - spacing, this.obj.GetComponent<RectTransform>().position.z));
            line.SetPosition(3, new Vector3(posX +  x/2 + spacing, posY + -y/2 - spacing, this.obj.GetComponent<RectTransform>().position.z));
            line.SetPosition(4, new Vector3(posX +  x/2 + spacing, posY +  y/2 + spacing, this.obj.GetComponent<RectTransform>().position.z));
            line.SetPosition(5, new Vector3(posX +  0            , posY +  y /2 + spacing, this.obj.GetComponent<RectTransform>().position.z));

            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = Color.black;
            line.endColor = Color.black;

            line.startWidth = 5;
            line.endWidth = 5;

        }

    }

    



    public Transform AddClass(string name)
    {
        if(!classesDict.ContainsKey(name) || classesDict[name] == null)
        {
            var cls = Instantiate(classPrefab, classesTransform);
            int currLayer = (GameObject.Find("#" + (GameObject.Find("TCPService").GetComponent<TCPService>().GetDiagCount() - 1))).layer;
            cls.layer = currLayer;
            TCPService.SetLayerRecursively(cls, currLayer);
            var tmp = cls.GetComponentInChildren<TextMeshProUGUI>();
            cls.name = tmp.text = name;
            classesDict[name] = cls.transform;
        }
        return classesDict[name];
    }

    public void AddLoop(IList tmpBounds,GameObject tmpObject)
    {
        bool existingLoop = false;
        Loop tmpLoop = null;

        //subloop scaling resize;
        int subloop = 0;

        List<Transform> subloopObjectList = new List<Transform>();

        if (loopsTransform.childCount > 0)
        {
            for(int i = 0; i < loopsTransform.childCount; i++)
            {
                Transform item = loopsTransform.GetChild(i);

                IList itemBounds = item.GetComponent<Loop>().getBounds();

                if(    (System.Convert.ToInt32(itemBounds[0].ToString()) == System.Convert.ToInt32(tmpBounds[0].ToString()))
                    && (System.Convert.ToInt32(itemBounds[1].ToString()) == System.Convert.ToInt32(tmpBounds[1].ToString())))
                {
                    existingLoop = true;

                    tmpLoop = item.GetComponent<Loop>();
                }

              
                if (!existingLoop)
                {
                    bool fromBottom = ( (System.Convert.ToInt32(itemBounds[0].ToString()) >= System.Convert.ToInt32(tmpBounds[0].ToString())));
                    bool fromTop = ((System.Convert.ToInt32(itemBounds[1].ToString()) <= System.Convert.ToInt32(tmpBounds[1].ToString())));

                    Debug.LogWarning("SUBLOOP CONDITION:" + System.Convert.ToInt32(itemBounds[0].ToString())+"  |  " + System.Convert.ToInt32(tmpBounds[0].ToString()));
                    Debug.LogWarning("SUBLOOP CONDITION:" + System.Convert.ToInt32(itemBounds[1].ToString()) + "  |  " + System.Convert.ToInt32(tmpBounds[1].ToString()));


                    if (fromBottom && fromTop)
                    {

                        Debug.LogWarning("SUBLOOP ADD:" + subloop);
                        subloop += 1;

                        subloopObjectList.Add(item);
                    }
                }
                
               
            }

        }

        if (!existingLoop)
        {

            var loop = Instantiate(loopPrefab, loopsTransform);
            int currLayer = (GameObject.Find("#" + (GameObject.Find("TCPService").GetComponent<TCPService>().GetDiagCount() - 1))).layer;
            loop.layer = currLayer;
            TCPService.SetLayerRecursively(loop, currLayer);


            loopsListAll.Add(loop);

            // get loop object script
            var lopScript = loop.GetComponent<Loop>();

            // init Object
            lopScript.setBounds(tmpBounds);
            lopScript.addLoopObject(tmpObject);

            Debug.LogWarning("SUBLOOP LOOP:"+ subloop);


            for (int i = 0; i < subloop; i++)
            {
                subloopObjectList[i].GetComponent<Loop>().decreaseLevel();
            }
        }
        else
        {
            tmpLoop.addLoopObject(tmpObject);
        }
    }

    public void AddMessage(string from, string to, string message, int lineNo, bool dashed=false)
    {
        var msg = Instantiate(messagePrefab, messagesTransform);
        int currLayer = (GameObject.Find("#" + (GameObject.Find("TCPService").GetComponent<TCPService>().GetDiagCount() - 1))).layer;
        msg.layer = currLayer;
        TCPService.SetLayerRecursively(msg, currLayer);
        var label = msg.transform.Find("label");
        var msgScript = msg.GetComponent<Message>();
        var fromClass = AddClass(from);
        var toClass = AddClass(to);
        msgScript.fromClass = fromClass.GetComponent<RectTransform>();
        msgScript.toClass = toClass.GetComponent<RectTransform>();
        msgScript.dashed = dashed;
        msgScript.line = lineNo;
        msgScript.name = message;
        label.GetComponent<TextMeshProUGUI>().text = message;
    }

    

    public void RemoveMessage(string from, string to)
    {
        //TODO  
        //destroy
        //but need unique id and from to is not unique
        //probably will be possible from UI only
    }

    public void RemoveClass(string name)
    {
        var toRemove = classesDict[name];
        Destroy(toRemove.gameObject);
        classesDict.Remove(name);
    }

}
