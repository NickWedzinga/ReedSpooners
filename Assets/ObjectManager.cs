using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject Spike;
    // Start is called before the first frame update
    void Start()
    {
        Spike = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Spike.transform.position = new Vector3(4.75f, 1, 20);
        Spike.AddComponent<Rigidbody>();
        Spike.AddComponent<BoxCollider>();
        //Spike = new GameObject("Spike");
        //Spike.AddComponent<Rigidbody>();
        //Spike.AddComponent<MeshFilter>();
        //Spike.AddComponent<BoxCollider>();
        //Spike.AddComponent<MeshRenderer>();
        //Spike.GetComponent<MeshFilter>().mesh = Mesh.
        //Spike.transform.position = new Vector3(0, 0, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
}
