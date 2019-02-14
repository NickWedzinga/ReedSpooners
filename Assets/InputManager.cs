using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    float _currentTime;
    public float _approachRate { get; set; }
    public int Score;
    public Text ScoreText;

    // Start is called before the first frame update
    void Start()
    {
        print("Start running");
        _currentTime = Time.deltaTime;
        Score = 0;
        ScoreText.text = "Score: " + Score.ToString();
        

        // Change to increase speed
        _approachRate = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void OnCollisionEnter(Collision bigColliderBoi)
    {
        if (bigColliderBoi.gameObject.name == "Spike")
        {
            Destroy(bigColliderBoi.gameObject);
            Score -= 50;
        }
        if (bigColliderBoi.gameObject.name == "Coin")
        {
            Destroy(bigColliderBoi.gameObject);
            Score += 50;
        }
        ScoreText.text = "Score: " + Score.ToString();
    }
}
