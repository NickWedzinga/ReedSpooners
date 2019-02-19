using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject[] Objects = new GameObject[100];
    public int lastObjectPositionZ = 0;
    public int nrOfHighlightedObjects { get; private set; } = 10;
        
    // Start is called before the first frame update
    void Start()
    {
        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObjects()
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Objects[i].gameObject.name = "Spike";
        }

        RandomHighlight();

        // Spawn random objects
        for (int i = 0; i < Objects.Length; ++i)
        {
            //Objects[i-3] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Objects[i].AddComponent<Rigidbody>();
            Objects[i].AddComponent<BoxCollider>();

            // i* 10 for 10 distance between objects, + i/10 to increase distance between slightly the higher the approach rate, +30 to start at z = 30
            Objects[i].transform.position = new Vector3(4.75f * RandomLane(), 1, (i) * 10 + i / 10 + 30);

            if (Objects[i].gameObject.name == "Coin")
            {
                // Object should be highlighted, apply highlight
                ApplyHighlight(i, 0);
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

    void RandomHighlight()
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

    public void Reset()
    {
        print("WHERE WE DROPPIN BOIS");
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i].gameObject.name = "Spike";
            Objects[i].GetComponent<MeshRenderer>().material.color = Objects[0].GetComponent<MeshRenderer>().material.color;
            Objects[i].transform.position = new Vector3(4.75f * RandomLane() , Objects[i].transform.position.y, Objects[i].transform.position.z);
            //Objects[i].gameObject.SetActive(false);
            Objects[i].SetActive(true);
        }

        RandomHighlight();

        for (int i = 0; i < Objects.Length; ++i)
        {
            if (Objects[i].gameObject.name == "Coin")
            {
                // Object should be highlighted, apply highlight
                ApplyHighlight(i, 1);
            }
        }
    }

    public virtual void ApplyHighlight(int index, int highlightType)
    {
        // Red Hue
        if(highlightType == 0)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
        else if (highlightType == 1)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 255);
    }
}
