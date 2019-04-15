using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public float _approachRate { get; set; }

    private Camera camera;
    public LANE lane {
        get
        {
            if (transform.position.x > 0)
                return LANE.RIGHT;
            else if (transform.position.x < 0)
                return LANE.LEFT;
            else
                return LANE.MIDDLE;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //print("This is a test.");

        // Change to increase speed
        _approachRate = 0.0f;

        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _approachRate == 0.0f)
        {
            StartCoroutine(GracePeriod());
        }
        if (_approachRate > 0.0f)
        {
            if (_approachRate < 43.0f)
                _approachRate += Time.deltaTime /** 10.0f*/;

            if (transform.position.z > Game.instance.lastObjectPositionZ + 50 && !Game.instance._GameOver)
            {
                ResetVariable();
            }
            if (/*(Input.GetKeyDown(KeyCode.D) || */Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 1)
            {
                transform.Translate(4.75f, 0, 0);
            }
            else if (/*(Input.GetKeyDown(KeyCode.A) || */Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -1)
            {
                transform.Translate(-4.75f, 0, 0);
            }
            transform.Translate(0, 0, Time.deltaTime * _approachRate);
        }

        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, this.transform.position.z -9.5f);//-8.4f);
    }

    private void ResetVariable()
    {
        _approachRate = Time.deltaTime;
        Game.instance.ResetVariable();
        gameObject.transform.position = new Vector3(0, gameObject.transform.position.y, 0.5f);
    }

    private IEnumerator GracePeriod()
    {
        _approachRate = Time.deltaTime;
        Game.instance.Announcer.fontSize = 60;
        Game.instance.Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f);
        Game.instance.Announcer.text = "GET READY FOR ROUND " + (Game.instance.round + 1).ToString();

        float timePassed = 0.0f;
        while (timePassed < 2.0f)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }
        _approachRate = 5.0f;
    }
}
