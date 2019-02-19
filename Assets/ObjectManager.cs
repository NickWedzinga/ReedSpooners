using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    public GameObject[] Objects = new GameObject[100];
    public int lastObjectPositionZ = 0;
    private int nrOfHighlightedObjects = 10;
    private Color _OriginalObjectColor;

    public Text Announcer;
    private float _AnnouncerTextTimer;
    private Color _OriginalTextColor;

    public int VisualVariableCounter;
    public int highlightType;

    public int[] VisualVariableOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

    private System.Random _random = new System.Random();

    public Mesh coinMesh;

    public int nrOfHighlightedObjects { get; private set; } = 10;
        
    // Start is called before the first frame update
    void Start()
    {
        //RANDOMIZE VISUALORDER
        Shuffle(VisualVariableOrder); 

        VisualVariableCounter = 0;

        _OriginalObjectColor = Color.gray;//Objects[0].GetComponent<MeshRenderer>().material.color;
        _OriginalTextColor = Announcer.color;
        _AnnouncerTextTimer = Time.deltaTime;
        
        Announcer.text = "GET READY FOR ROUND " + VisualVariableCounter.ToString();
        Announcer.fontSize = 40;

        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (_AnnouncerTextTimer > 0)
        {
            Announcer.color = Color.Lerp(_OriginalTextColor, Color.clear, Mathf.Min(1, _AnnouncerTextTimer / 3.0f));
            _AnnouncerTextTimer += Time.deltaTime;
            if (_AnnouncerTextTimer > 3.0f)
            {
                _AnnouncerTextTimer = 0.0f;
                Announcer.gameObject.SetActive(false);
            }
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Objects[i].gameObject.name = "Spike";
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
        }

        RandomizeObjectsForHighlight();

        // Spawn random objects
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i].AddComponent<Rigidbody>();
            Objects[i].AddComponent<BoxCollider>();

            // i* 10 for 10 distance between objects, + i/10 to increase distance between slightly the higher the approach rate, +30 to start at z = 30
            Objects[i].transform.position = new Vector3(4.75f * RandomLane(), 1, (i) * 10 + i / 10 + 50);

            if (Objects[i].gameObject.name == "Coin")
            {
                // Object should be highlighted, apply highlight
                //ApplyHighlight(i, VisualVariableOrder[VisualVariableCounter]);
                ApplyHighlight(i, 3);
            }
            if (i == Objects.Length - 1)
                lastObjectPositionZ = (int)Objects[i].transform.position.z;
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
            Objects[indexList[i]].gameObject.name = "Coin";
        }
    }

    void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            // find random index to swap
            int randomIndex = _random.Next(0, i);
            int temp = array[randomIndex];

            // swap
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }

    public void Reset()
    {
        VisualVariableCounter++;

        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i].gameObject.name = "Spike";
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
            Objects[i].transform.position = new Vector3(4.75f * RandomLane() , Objects[i].transform.position.y, Objects[i].transform.position.z);
            Objects[i].SetActive(true);
        }

        // Choose random objects that will be highlighted
        RandomizeObjectsForHighlight();

        // Announcer text reset and active
        Announcer.text = "GET READY FOR ROUND " + VisualVariableCounter.ToString();
        Announcer.gameObject.SetActive(true);
        _AnnouncerTextTimer += Time.deltaTime;

        for (int i = 0; i < Objects.Length; ++i)
        {
            if (Objects[i].gameObject.name == "Coin")
            {
                // Object should be highlighted, apply highlight
                ApplyHighlight(i, VisualVariableOrder[VisualVariableCounter]);
            }
        }
    }

    public virtual void ApplyHighlight(int index, int highlightType)
    {
        // V0 : Control
        if (highlightType == 0)
            Objects[index].gameObject.GetComponent<MeshFilter>().mesh = coinMesh;
        // V1 : Hue (red)
        else if (highlightType == 1)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
        // V2 : Hue (blue)
        else if (highlightType == 2)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 255);
        // V3 : Motion
        else if (highlightType == 3)
            StartCoroutine(Rotator(index));
        // V4 : Luminance, low value
        else if (highlightType == 4)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
        // V5 : Luminance, high value
        else if (highlightType == 5)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255);
        else if (highlightType == 6)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
        else if (highlightType == 7)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
        else if (highlightType == 8)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
    }

    private IEnumerator Rotator(int index)
    {
        while (VisualVariableOrder[VisualVariableCounter] == 3)
        {
            Objects[index].transform.Rotate(new Vector3(0, 1, 0), 20.0f);
            yield return null;
        }
        Objects[index].transform.localRotation = Quaternion.identity;
    }
}
