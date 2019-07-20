using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;
using TMPro;
using System;

public class Message : MonoBehaviour
{
    public RectTransform fromClass;
    public RectTransform toClass;

    public bool dashed = false;
    public float segmentWidth = 5;

    public int line = 0;
    public bool inLoop = false;

    //existing in subloop or else
    public bool inserted = false;


    public string name = "";

    protected virtual float Width
    {
        get { return Mathf.Abs(fromClass.anchoredPosition.x - toClass.anchoredPosition.x); }
    }

    protected virtual bool Reversed
    {
        get { return fromClass.anchoredPosition.x > toClass.anchoredPosition.x; }
    }

    protected virtual int Orientation
    {
        get { return Reversed ? 0 : 180; }
    }

    protected virtual int Direction
    {
        get { return Reversed ? -1 : 1; }
    }

    protected virtual float StartPosX
    {
        get { return Reversed ? toClass.anchoredPosition.x : fromClass.anchoredPosition.x; }
    }

    protected virtual List<Vector2> LinePoints
    {
        get
        {
            var points = new List<Vector2>();

            points.Add(new Vector2(0, 0));
            if (dashed)
            {
                for (float i = 0; i < Width; i += segmentWidth)
                {
                    points.Add(new Vector2(i, 0));
                }
            }
            if (points.Count % 2 == 0)
            {
                points.RemoveAt(points.Count - 1);
            }
            points.Add(new Vector2(Width, 0));

            return points;
        }
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public Vector3 getLocalPosition()
    {
        return transform.localPosition;
    }



    protected virtual List<Vector2> LineAltPoints
    {
        get
        {
            var points = new List<Vector2>();

            float padding = 25;

            points.Add(new Vector2(-padding, padding));
            points.Add(new Vector2(Width+ padding, padding));

            points.Add(new Vector2(Width + padding, padding));
            points.Add(new Vector2(Width+ padding, -50 - padding));

            points.Add(new Vector2(Width+ padding, -50 - padding));
            points.Add(new Vector2(-padding, -50 - padding));

            points.Add(new Vector2(-padding, -50- padding));
            points.Add(new Vector2(-padding, padding));

            return points;
        }
    }

    private void Start()
    {
        try
        {
            var sequenceLoops = transform.parent.parent.GetComponent<SequenceDiagram>().getLoops();

            IList savedLoop = null;


            foreach (IList singleLoop in sequenceLoops)
            {
                int topBound = int.Parse(singleLoop[0].ToString());
                int bottomBound = int.Parse(singleLoop[1].ToString());

                if (this.line >= topBound && this.line <= bottomBound)
                {
                    this.inLoop = true;

                    savedLoop = singleLoop;
                }
            }

            if (this.inLoop)
            {
                transform.parent.parent.GetComponent<SequenceDiagram>().AddLoop(savedLoop,transform.gameObject);
            }
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if target or source class removed delete message as well
        if(fromClass == null || toClass == null)
        {
            Destroy(gameObject);
            return;
        }

        var line = transform.Find("line");
        var label = transform.Find("label");
        var arrow = line.Find("arrow");


        UILineRenderer lr = line.GetComponent<UILineRenderer>();
        var rta = arrow.GetComponent<RectTransform>();

        rta.anchoredPosition = new Vector2(Direction * (Width - rta.sizeDelta.x) * 0.5f, 0);
        arrow.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, Orientation, 0);
        GetComponent<VerticalLayoutGroup>().padding.left = (int)StartPosX;

        line.GetComponent<LayoutElement>().minHeight = rta.sizeDelta.y;
        var rt = line.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Width, 0); //using 0 since height is ignored due to layout

        rt = label.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Width, 0);
   
        lr.Points = LinePoints.ToArray();


     
    }
}
