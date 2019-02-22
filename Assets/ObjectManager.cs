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

    public Texture2D HighlightTexture;

    public Material LightMaterial;
    private Material _OriginalMaterial;
    private float _FlashingTimer;

    // Start is called before the first frame update
    void Start()
    {
        //RANDOMIZE VISUALORDER
        Shuffle(VisualVariableOrder); 

        VisualVariableCounter = 0;
        _FlashingTimer = 0.0f;

        _OriginalObjectColor = Color.gray;
        _OriginalTextColor = Announcer.color;

    }

    // Update is called once per frame
    void Update()
    {
        // Fade out text
        

        // Flash flashing objects
        if (_FlashingTimer > 0.0f)
        {
            _FlashingTimer += Time.deltaTime;

            // Time to change intensity
            if(_FlashingTimer > 0.1f)
            {

                // Reset timer
                _FlashingTimer = Time.deltaTime;

                // Loop and update intensity
                for (int i = 0; i < Objects.Length; ++i)
                {
                    if (Objects[i].GetComponent<MeshRenderer>().material.color.r == 0)
                    {
                        Objects[i].GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255);
                    }
                    else if (Objects[i].GetComponent<MeshRenderer>().material.color.r == 255)
                    {
                        Objects[i].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
                    }
                }
            }
        }
    }

    void SpawnObjects(VISUAL_VARIABLE visVar)
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            if(i >= 5 && i < 15)
            {
                Objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Objects[i].transform.localScale = new Vector3(1.1f, 0.1f, 1.1f);
                Objects[i].transform.Rotate(Vector3.right, 90.0f);
                Objects[i].gameObject.name = "Coin";
            }
            else
            {
                Objects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Objects[i].gameObject.name = "Spike";
            }
                
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.white;//Color.grey;
        }
        _OriginalMaterial = Objects[0].GetComponent<MeshRenderer>().material;

        // Randomize object array
        ObjectShuffle();
        //RandomizeObjectsForHighlight();

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
                ApplyHighlight(i, VisualVariableOrder[0]);
                //ApplyHighlight(i, 6);
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

    //void RandomizeObjectsForHighlight()
    //{
    //    List<int> indexList = new List<int>();

    //    while (indexList.Count < nrOfHighlightedObjects)
    //    {
    //        int value = (int)(Random.value * Objects.Length);
    //        if(!indexList.Contains(value) && value > 5)
    //        {
    //            indexList.Add(value);
    //        }   
    //    }

    //    for (int i = 0; i < nrOfHighlightedObjects; ++i)
    //    {
    //        Objects[indexList[i]].gameObject.name = "Coin";
    //    }
    //}

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

    void ObjectShuffle()
    {
        bool shuffled = false;
        int loopLimit = 0;

        while(!shuffled && loopLimit < 100)
        {
            for (int i = 5; i < Objects.Length; ++i)
            {
                if (Objects[i].gameObject.name == "Coin")
                {
                    // Save coin object to temp var
                    GameObject temp = Objects[i];

                    int index = (int)(5 + Random.value * 95);
                    print("Index " + index + " which is a object: " + Objects[index].gameObject.name);
                    GameObject tempRandom = Objects[index];
                    if (tempRandom.gameObject.name == "Spike")
                    {
                        Objects[i] = Objects[index];
                        Objects[index] = temp;

                        
                        // make coin a spike instead, change position back to old position
                        //Objects[i] = tempRandom;

                        //print("Getting " + tempRandom.gameObject.name + " from index: " + index);
                        //print("Coin  with position Z: " + tempZ + " became: " + Objects[i].gameObject.name + " with position Z: " + Objects[i].transform.position.z);


                        //// chosen spike now becomes a coin
                        //Objects[index] = temp;
                        //Objects[index].transform.position = tempRandom.transform.position;


                        Objects[index].transform.position = new Vector3(temp.transform.position.x, temp.transform.position.y + 3, temp.transform.position.z);
                        Objects[i].transform.position = new Vector3(tempRandom.transform.position.x, tempRandom.transform.position.y, tempRandom.transform.position.z);

                        loopLimit++;
                    }
                }
            }

            shuffled = true;
            //bool previousObjectWasCoin = false;
            //// Validation loop to check if shuffled
            //for (int i = 0; i < Objects.Length; ++i)
            //{
            //    //print(Objects[i].gameObject.name);
            //    if (Objects[i].gameObject.name == "Coin" && !previousObjectWasCoin)
            //    {
            //        previousObjectWasCoin = true;
            //    }
            //    // Two coins in a row
            //    else if (Objects[i].gameObject.name == "Coin" && previousObjectWasCoin)
            //    {
            //        shuffled = false;
            //    }
            //    else if (previousObjectWasCoin)
            //        previousObjectWasCoin = false;
            //}
        }
    }

    public void Reset()
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            Destroy(Objects[i].GetComponent<Light>());
            
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
            Objects[i].GetComponent<MeshRenderer>().material = _OriginalMaterial;
            Objects[i].transform.position = new Vector3(4.75f * RandomLane() , Objects[i].transform.position.y, Objects[i].transform.position.z);
            Objects[i].SetActive(true);
        }

        VisualVariableCounter++;
        _FlashingTimer = 0.0f;

        // Choose random objects that will be highlighted
        ObjectShuffle();
        //RandomizeObjectsForHighlight();

        for (int i = 0; i < Objects.Length; ++i)
        {

            if (Objects[i].type == TYPE.COIN)
            {
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
        else if (highlightType == 6)
        {
            Objects[index].GetComponent<MeshRenderer>().material = LightMaterial;
            //Light tempLight = Objects[index].AddComponent<Light>();
            //tempLight.intensity = 10.0f;
            //tempLight.range = 5.0f;
        }
        //Objects[index].GetComponent<MeshRenderer>().material.color = new Color(125, 125, 125);
        else if (highlightType == 7)
        {
            Objects[index].GetComponent<MeshRenderer>().material.mainTexture = HighlightTexture;
        }
        else if (highlightType == 8)
        {
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
            _FlashingTimer = Time.deltaTime;
        }
    }

    private IEnumerator Rotator(int index)
    {
        while (VisualVariableOrder[VisualVariableCounter] == 3)
        {
            Objects[index].transform.Rotate(new Vector3(0, 1, 0), 20.0f);
            yield return null;
        }
        Objects[index].transform.localRotation = Quaternion.identity;
        Objects[index].transform.Rotate(Vector3.right, 90.0f);
    }
}
