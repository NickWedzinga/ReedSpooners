using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    float _currentTime;
    public float _approachRate { get; set; }
    public int Score;
    public Text ScoreText;
    public ObjectManager objectManager;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        print("WHY AH U RUNNING?!");
        objectManager = gameObject.GetComponent<ObjectManager>();

        _currentTime = Time.deltaTime;
        Score = 0;
        ScoreText.text = "Score: " + Score.ToString();
        

        // Change to increase speed
        _approachRate = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > objectManager.lastObjectPositionZ + 50)
        {
            Reset();
        }

        _currentTime += Time.deltaTime;
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && transform.position.x < 1)
        {
            transform.Translate(4.75f, 0, 0);
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && transform.position.x > -1)
        {
            transform.Translate(-4.75f, 0, 0);
        }
        transform.Translate(0, 0, Time.deltaTime * _approachRate);

        _approachRate += Time.deltaTime;
    }

    private void Reset()
    {
        objectManager.Reset();

        _approachRate = 5.0f;
        gameObject.transform.position = new Vector3(0, 1, 0.5f);
    }

    public void OnCollisionEnter(Collision bigColliderBoi)
    {
        if (bigColliderBoi.gameObject.name == "Spike")
        {
            //Destroy(bigColliderBoi.gameObject);
            //bigColliderBoi.gameObject.transform.position = new Vector3(bigColliderBoi.gameObject.transform.position.x, 10, bigColliderBoi.gameObject.transform.position.z);
            bigColliderBoi.gameObject.SetActive(false);
            Score -= 50;
        }
        if (bigColliderBoi.gameObject.name == "Coin")
        {
            //Destroy(bigColliderBoi.gameObject);
            //bigColliderBoi.gameObject.transform.position = new Vector3(bigColliderBoi.gameObject.transform.position.x, 10, bigColliderBoi.gameObject.transform.position.z);
            bigColliderBoi.gameObject.SetActive(false);
            Score += 50;
        }
        ScoreText.text = "Score: " + Score.ToString();
    }
}
