using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public HighlightableObject[] Objects = new HighlightableObject[100];

    public Mesh SpikeMesh;
    private Mesh CoinMesh;

    private Color _OriginalObjectColor = Color.gray;

    public Texture2D HighlightTextureCoin;
    public Texture2D HighlightTextureSpike;

    public Material LightMaterial;
    public Material OriginalMaterial;
    public Material HueMaterial;

    private System.Random _random = new System.Random();

    private float _FlashingTimer;

    public int lastObjectPositionZ = 0;
    public int nrOfHighlightedObjects { get; private set; } = 10;

    public Subset owner;

    // Start is called before the first frame update
    void Start()
    {
        _FlashingTimer = 0.0f;
        _OriginalObjectColor = Color.gray;
        owner = FindObjectOfType<Subset>();
    }

    // Update is called once per frame
    void Update()
    {
        #region flashing
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
                        Objects[i].GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1);
                    }
                    else if (Objects[i].GetComponent<MeshRenderer>().material.color.r == 1)
                    {
                        Objects[i].GetComponent<MeshRenderer>().material.color = new Color(0 , 0, 0);
                    }
                }
            }
        }
        #endregion
    }

    public void SpawnObjects()
    {
        for (int i = 0; i < Objects.Length; ++i)
        {
            if(i >= 5 && i < 55)
            {
                GameObject gObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Objects[i] = gObj.AddComponent<HighlightableObject>();
                Objects[i].transform.localScale = new Vector3(1.1f, 0.1f, 1.1f);
                Objects[i].transform.Rotate(Vector3.right, 90.0f);
                Objects[i].type = TYPE.COIN;

                Rigidbody rBody = Objects[i].gameObject.AddComponent<Rigidbody>();
                rBody.constraints &= RigidbodyConstraints.FreezeRotationX;
                rBody.constraints &= RigidbodyConstraints.FreezeRotationZ;
                rBody.constraints &= ~RigidbodyConstraints.FreezePosition;

                Destroy(gObj.GetComponent<CapsuleCollider>());

                BoxCollider collider = gObj.AddComponent<BoxCollider>();
                collider.center = new Vector3(0, 0, -0.3f);
                collider.size = new Vector3(2f, 8f, 1.75f/1.1f);
            }
            else
            {
                GameObject gObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Objects[i] = gObj.AddComponent<HighlightableObject>();
                Objects[i].gameObject.GetComponent<MeshFilter>().mesh = SpikeMesh;
                Objects[i].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                Objects[i].transform.Rotate(Vector3.up, 180.0f);
                Objects[i].type = TYPE.SPIKE;

                Rigidbody rBody = Objects[i].gameObject.AddComponent<Rigidbody>();
                rBody.constraints &= RigidbodyConstraints.FreezeRotationX;
                rBody.constraints &= RigidbodyConstraints.FreezeRotationZ;

                BoxCollider collider = gObj.GetComponent<BoxCollider>();
                collider.center = new Vector3(0, 0.375f, 0);
                collider.size = new Vector3(2.5f, 1.75f, 1f);
            }
            Objects[i].transform.position = new Vector3(0, 1, (i) * 10 + i / 10 + 50);
            Objects[i].GetComponent<MeshRenderer>().material = OriginalMaterial;
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.gray;
            
            if (Objects[i].transform.position.z > lastObjectPositionZ)
                lastObjectPositionZ = (int)Objects[i].transform.position.z;
        }
        CoinMesh = Objects[5].gameObject.GetComponent<MeshFilter>().mesh;
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
        int loopLimit = 0;

        // Shuffle objects 10 times
        while(loopLimit < 10)
        {
            for (int i = 5; i < Objects.Length; ++i)
            {
                // Loop through all coins and switch places with random spikes
                if (Objects[i].type == TYPE.COIN)
                {
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
        }
    }

    void SelectObjectsForHighlight(SCENARIO scenario)
    {
        bool finished = false;
        int nrOfHighlightsChosen = 0;
        int nrOfHighlightedObjects = 25;

        while (!finished)
        {
            // selects 25 objects for highlight
            if (nrOfHighlightsChosen == nrOfHighlightedObjects)
                finished = true;

            int highlightIndex = (int)(Random.value * 94 + 5);
            if (Objects[highlightIndex].type == TYPE.COIN && scenario == SCENARIO.POSITIVE && Objects[highlightIndex].highlight == HIGHLIGHT.NO)
            {
                nrOfHighlightsChosen++;
                Objects[highlightIndex].highlight = HIGHLIGHT.HIGHLIGHTEDCOIN;
            }
            else if (Objects[highlightIndex].type == TYPE.SPIKE && scenario == SCENARIO.NEGATIVE && Objects[highlightIndex].highlight == HIGHLIGHT.NO)
            {
                nrOfHighlightsChosen++;
                Objects[highlightIndex].highlight = HIGHLIGHT.HIGHLIGHTEDSPIKE;
            }
        }
    }

    public void Reset(VISUAL_VARIABLE visVar, SCENARIO scenario)
    {
        StopAllCoroutines();

        // Reset values between rounds
        for (int i = 0; i < Objects.Length; ++i)
        {
            Objects[i].GetComponent<MeshRenderer>().material = OriginalMaterial;
            Objects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
            Objects[i].transform.position = new Vector3(4.75f * RandomLane(), 1, Objects[i].transform.position.z);

            Objects[i].highlight = HIGHLIGHT.NO;
            Objects[i].hasEnteredView = false;
            Objects[i].enteredViewAt = 0;
            Objects[i].gameObject.SetActive(true);

            Rigidbody rBody = Objects[i].GetComponent<Rigidbody>();
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
        }

        Camera.main.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>().enabled = false;
        _FlashingTimer = 0.0f;

        // Choose random objects that will be highlighted and shuffle
        SelectObjectsForHighlight(scenario);
        ObjectShuffle();

        // Apply highlight
        for (int i = 0; i < Objects.Length; ++i)
        {
            if (Objects[i].type == TYPE.COIN)
            {
                Objects[i].transform.rotation = Quaternion.Euler(90.0f, 0, 0);
                if (scenario == SCENARIO.POSITIVE && Objects[i].highlight != HIGHLIGHT.NO)
                {
                    ApplyHighlight(i, visVar);
                }
            }
            else if (Objects[i].type == TYPE.SPIKE)
            {
                Objects[i].transform.rotation = Quaternion.Euler(0, 180.0f, 0);
                if (scenario == SCENARIO.NEGATIVE && Objects[i].highlight != HIGHLIGHT.NO)
                {
                    ApplyHighlight(i, visVar);
                }
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
            if (Objects[index].type == TYPE.COIN)
                Objects[index].GetComponent<MeshRenderer>().material.mainTexture = HighlightTextureCoin;
            else
                Objects[index].GetComponent<MeshRenderer>().material.mainTexture = HighlightTextureSpike;
        }
        // V8 : FLASHING
        else if (visVar == VISUAL_VARIABLE.FLASH)
        {
            Objects[index].GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
            _FlashingTimer = Time.deltaTime;
        }
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

    private IEnumerator Rotator(int index, VISUAL_VARIABLE visVar)
    {
        while (visVar == VISUAL_VARIABLE.MOTION)
        {
            float spinPerSecond = 1.0f / Time.deltaTime;
            float anglePerSpin = spinPerSecond / 3.0f;
            if (Objects[index].type == TYPE.COIN)
                Objects[index].transform.Rotate(new Vector3(0, 0, 1), anglePerSpin);
            else if (Objects[index].type == TYPE.SPIKE)
                Objects[index].transform.Rotate(Vector3.up, anglePerSpin);
            yield return null;
        }
    }
}
