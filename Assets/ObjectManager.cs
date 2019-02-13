using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //GameObject Objects[100] = {GameObject.CreatePrimitive(PrimitiveType.Cube)}

    // Start is called before the first frame update
    void Start()
    {
        // Spawn random objects
        for (int i = 3; i < 103; ++i)
        {
            GameObject Object = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.AddComponent<Rigidbody>();
            Object.AddComponent<BoxCollider>();

            Object.gameObject.name = "Spike";
            Object.transform.position = new Vector3(4.75f * RandomLane(), 1, (i) * 10 + i/10);

            if (RandomHighlight())
            {
                Object.gameObject.name = "Coin";
                Object.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

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

    bool RandomHighlight()
    {
        float value = Random.value * 10;

        if ((int)value == 0)
            return true;
        return false;
    }
}
