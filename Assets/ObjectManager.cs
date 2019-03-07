using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public HighlightableObject[] Objects = new HighlightableObject[100];
    public int lastObjectPositionZ = 0;
    public int nrOfHighlightedObjects { get; private set; } = 10;
    private Color _OriginalObjectColor = Color.gray;
    
    private System.Random _random = new System.Random();

    public Mesh coinMesh;

    public Subset owner;

    // Start is called before the first frame update
    void Start()
    {
        owner = gameObject.GetComponent<Subset>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnObjects()
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            GameObject gObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Objects[i] = gObj.AddComponent<HighlightableObject>();
            Objects[i].gameObject.AddComponent<Rigidbody>();
            Objects[i].gameObject.AddComponent<BoxCollider>();
            Objects[i].owner = owner;
        }
    }

    int RandomLane()
    {
        float value = Random.value * 3;

        if ((int)value == 0)
            return -1;
        else if ((int)value == 1)
            return 0;
        else
            return 1;
    }

    void RandomizeObjectsForHighlight()
    {
        List<int> indexList = new List<int>();

        while (indexList.Count < nrOfHighlightedObjects)
        {
            int value = (int)(Random.value * Objects.Length);
            if(!indexList.Contains(value) && value > 5)
            {
                indexList.Add(value);
            }   
        }

        for (int i = 0; i < nrOfHighlightedObjects; ++i)
        {
            Objects[indexList[i]].type = TYPE.COIN;
        }
    }

    public void ResetHighlight(VISUAL_VARIABLE visVar)
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i].type = TYPE.SPIKE;
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.gray;
            Objects[i].transform.position = new Vector3(4.75f * RandomLane() , 1, (i) * 10 + i / 10 + 50);
            if (Objects[i].transform.position.x > 0)
                Objects[i].lane = LANE.RIGHT;
            else if (Objects[i].transform.position.x < 0)
                Objects[i].lane = LANE.LEFT;
            else
                Objects[i].lane = LANE.MIDDLE;
            Objects[i].hasEnteredView = false;
            Objects[i].enteredViewAt = 0;
            Objects[i].gameObject.SetActive(true);
        }

        // Choose random objects that will be highlighted
        RandomizeObjectsForHighlight();
        int highlightCounter = 0;
        for (int i = 0; i < Objects.Length; ++i)
        {

            if (Objects[i].type == TYPE.COIN)
            {
                Objects[i].ID = highlightCounter;
                ++highlightCounter;
                // Object should be highlighted, apply highlight
                ApplyHighlight(i, visVar);
            }

            if (i == Objects.Length - 1)
                lastObjectPositionZ = (int)Objects[i].transform.position.z;
        }
    }

    public virtual void ApplyHighlight(int index, VISUAL_VARIABLE visVar)
    {
        // V0 : Control
        if (visVar == VISUAL_VARIABLE.CONTROL)
            Objects[index].gameObject.GetComponent<MeshFilter>().mesh = coinMesh;
        // V1 : Hue (red)
        else if (visVar == VISUAL_VARIABLE.HUE_RED)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
        // V2 : Hue (blue)
        else if (visVar == VISUAL_VARIABLE.HUE_BLUE)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 255);
        // V3 : Motion
        //else if (visVar == VISUAL_VARIABLE.MOTION)
        //StartCoroutine(Rotator(index));
        // V4 : Luminance, low value
        else if (visVar == VISUAL_VARIABLE.LUM_LO)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
        // V5 : Luminance, high value
        else if (visVar == VISUAL_VARIABLE.LUM_HI)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255);
        //else if (visVar == 6)
        //    Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
        //else if (visVar == 7)
        //    Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
        //else if (visVar == 8)
        //    Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
    }

    public void SendGazeData()
    {
        foreach (var highlighted in Objects)
        {
            if ((highlighted.type == TYPE.COIN && Game.instance.scenario == SCENARIO.POSITIVE) ||
                (highlighted.type == TYPE.SPIKE && Game.instance.scenario == SCENARIO.NEGATIVE))
            {
                highlighted.SendGazeData();
            }
        }
    }

    //private IEnumerator Rotator(int index)
    //{
    //    while (VisualVariableOrder[VisualVariableCounter] == 3)
    //    {
    //        Objects[index].transform.Rotate(new Vector3(0, 1, 0), 20.0f);
    //        yield return null;
    //    }
    //    Objects[index].transform.localRotation = Quaternion.identity;
    //}
}
