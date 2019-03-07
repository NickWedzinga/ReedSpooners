using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public HighlightableObject[] Objects = new HighlightableObject[100];
    public Mesh SpikeMesh;
    public int lastObjectPositionZ = 0;
    public int nrOfHighlightedObjects { get; private set; } = 10;
    private Color _OriginalObjectColor = Color.gray;
    
    private System.Random _random = new System.Random();

    public Texture2D HighlightTexture;

    public Material LightMaterial;
    public Material OriginalMaterial;
    public Material HueMaterial;
    private float _FlashingTimer;

    // Start is called before the first frame update
    void Start()
    {
        _FlashingTimer = 0.0f;
        _OriginalObjectColor = Color.gray;
    }

    // Update is called once per frame
    void Update()
    {
        // To negate velocity build up from spinning, freezeposition does not take care of this apparently.
        for(int i = 0; i < Objects.Length; ++i)
        {
            if (Objects[i].type == TYPE.COIN)
            {
                Rigidbody rBody = Objects[i].GetComponent<Rigidbody>();
                rBody.velocity = Vector3.zero;
            }
        }

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

    public void SpawnObjects(VISUAL_VARIABLE visVar)
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            if (i >= 5 && i < 15)
            {
                GameObject gObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Objects[i] = gObj.AddComponent<HighlightableObject>();
                Objects[i].transform.localScale = new Vector3(1.1f, 0.1f, 1.1f);
                Objects[i].transform.Rotate(Vector3.right, 90.0f);
                Objects[i].gameObject.name = "Coin";
                Objects[i].type = TYPE.COIN;
                Rigidbody rBody = Objects[i].gameObject.AddComponent<Rigidbody>();
                rBody.constraints &= RigidbodyConstraints.FreezeRotationX;
                rBody.constraints &= RigidbodyConstraints.FreezeRotationZ;
                rBody.constraints &= ~RigidbodyConstraints.FreezePosition;
            }
            else
            {
                GameObject gObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Objects[i] = gObj.AddComponent<HighlightableObject>();
                Objects[i].gameObject.GetComponent<MeshFilter>().mesh = SpikeMesh;
                Objects[i].gameObject.name = "Spike";
                Objects[i].type = TYPE.SPIKE;
                Rigidbody rBody = Objects[i].gameObject.AddComponent<Rigidbody>();
                rBody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            Objects[i].transform.position = new Vector3(0, 1, (i) * 10 + i / 10 + 50);
            Objects[i].GetComponent<MeshRenderer>().material = OriginalMaterial;
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.gray;
            
            if (Objects[i].transform.position.z > lastObjectPositionZ)
                lastObjectPositionZ = (int)Objects[i].transform.position.z;
        }

        Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().enabled = false;
        // Randomize object array
        ObjectShuffle();
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

    void ObjectShuffle()
    {
        bool shuffled = false;
        int loopLimit = 0;

        while(!shuffled && loopLimit < 100)
        {
            for (int i = 5; i < Objects.Length; ++i)
            {
                if (Objects[i].type == TYPE.COIN)
                {
                    // Save coin object to temp var
                    HighlightableObject temp = Objects[i];

                    int index = (int)(5 + Random.value * 95);
                    if (Objects[index].type == TYPE.SPIKE)
                    {
                        Rigidbody rBodyTemp = Objects[i].GetComponent<Rigidbody>();
                        rBodyTemp.constraints &= ~RigidbodyConstraints.FreezePosition;

                        Vector3 oldCoinPos = Objects[i].transform.position;
                        Vector3 oldSpikePos = Objects[index].transform.position;

                        Objects[i].transform.position = oldSpikePos;
                        Objects[index].transform.position = oldCoinPos;

                        loopLimit++;
                        rBodyTemp.constraints &= RigidbodyConstraints.FreezePosition;
                    }
                }
            }

            shuffled = true;
            bool previousObjectWasCoin = false;
            // Validation loop to check if shuffled
            for (int i = 0; i < Objects.Length; ++i)
            {
                if (Objects[i].type == TYPE.COIN && !previousObjectWasCoin)
                {
                    previousObjectWasCoin = true;
                }
                // Two coins in a row
                else if (Objects[i].type == TYPE.COIN && previousObjectWasCoin)
                {
                    shuffled = false;
                }
                else if (previousObjectWasCoin)
                    previousObjectWasCoin = false;
            }
        }
    }

    public void Reset(VISUAL_VARIABLE visVar, int scenario)
    {
        StopAllCoroutines();
        Random.InitState((int)visVar);
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i].GetComponent<MeshRenderer>().material = OriginalMaterial;
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
            Objects[i].transform.position = new Vector3(4.75f * RandomLane() , /*Objects[i].transform.position.y*/1, Objects[i].transform.position.z);
            Objects[i].gameObject.SetActive(true);
        }

        Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().enabled = false;
        _FlashingTimer = 0.0f;

        // Choose random objects that will be highlighted
        ObjectShuffle();

        for (int i = 0; i < Objects.Length; ++i)
        {
            if (Objects[i].type == TYPE.COIN && scenario == 0)
            {
                Objects[i].transform.rotation = Quaternion.Euler(90.0f, 0, 0);
                // Object should be highlighted, apply highlight
                ApplyHighlight(i, visVar/*VISUAL_VARIABLE.MOTION*/);
            }
            else if (Objects[i].type == TYPE.SPIKE && scenario == 1)
            {
                Objects[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                // Object should be highlighted, apply highlight
                ApplyHighlight(i, visVar/*VISUAL_VARIABLE.MOTION*/);
            }
        }
    }

    public virtual void ApplyHighlight(int index, VISUAL_VARIABLE visVar)
    {
        // V0 : Control
        // ~ nada
        // V1 : Hue (red)
        if (visVar == VISUAL_VARIABLE.HUE_RED)
        {
            Objects[index].GetComponent<MeshRenderer>().material = HueMaterial;
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
        }
        // V2 : Hue (blue)
        else if (visVar == VISUAL_VARIABLE.HUE_BLUE)
        {
            Objects[index].GetComponent<MeshRenderer>().material = HueMaterial;
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 255);
        }
        // V3 : Motion
        else if (visVar == VISUAL_VARIABLE.MOTION)
            StartCoroutine(Rotator(index, visVar));
        // V4 : Luminance, low value
        else if (visVar == VISUAL_VARIABLE.LUM_LO)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
        // V5 : Luminance, high value
        else if (visVar == VISUAL_VARIABLE.LUM_HI)
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255);
        // V6 : GLOW
        else if (visVar == VISUAL_VARIABLE.GLOW)
        {
            Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().enabled = true;
            Objects[index].GetComponent<MeshRenderer>().material = LightMaterial;
            Objects[index].GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        // V7 : TEXTURE
        else if (visVar == VISUAL_VARIABLE.TEXTURE)
        {
            Objects[index].GetComponent<MeshRenderer>().material.mainTexture = HighlightTexture;
        }
        // V8 : FLASHING
        else if (visVar == VISUAL_VARIABLE.FLASH)
        {
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
            _FlashingTimer = Time.deltaTime;
        }
    }

    private IEnumerator Rotator(int index, VISUAL_VARIABLE visVar)
    {
        while (visVar == VISUAL_VARIABLE.MOTION)
        {
            float spinPerSecond = 1.0f / Time.deltaTime;
            float anglePerSpin = spinPerSecond / 2.0f;
            if(Objects[index].type == TYPE.COIN)
                Objects[index].transform.Rotate(new Vector3(0, 0, 1), anglePerSpin);
            else if (Objects[index].type == TYPE.SPIKE)
                Objects[index].transform.Rotate(new Vector3(0, 1, 0), anglePerSpin);
            yield return null;
        }
        //Objects[index].transform.localRotation = Quaternion.identity;
        //Objects[index].transform.Rotate(Vector3.right, 90.0f);
    }
}
