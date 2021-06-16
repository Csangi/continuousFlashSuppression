using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class createMond : MonoBehaviour
{
    public Sprite[] spriteArr;
    public Texture2D tex;
    public SpriteRenderer[] sr;
    public float actionTime;

    //CSV variables - we only need the flash value
    static GameObject uiVars;
    public List<int> flash = new List<int>(); //flash of theImg showing
    //eye toggles
    public Toggle rightEye;
    public Toggle leftEye;
    //this is the mod vaule
    private float period;
    // Start is called before the first frame update
    void Start()
    {   
        uiVars = GameObject.Find("UIManager");
        flash = uiVars.GetComponent<UIManager>().flash;
        //set toggles
        rightEye = uiVars.GetComponent<UIManager>().rightEye;
        leftEye = uiVars.GetComponent<UIManager>().leftEye;

        updateMond(rightEye, leftEye);
        
        //set this from csv file - grab the 3nd item in the flash list (skips instruction and labels)
        period = (float)flash[2]/1000;
        actionTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //update mondrians if the actiontime is more than the current time
        if(Time.time > actionTime)
        {
            actionTime = Time.time + period;
            updateMond(rightEye, leftEye);
        }
    }

    void updateMond(Toggle right, Toggle left)
    {
        //right eye dom, show mondrians to right eye
        if(right.isOn == true)
        {
            for(int i = 0; i < 140; i++)
            {
                spriteArr[i] = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                sr[i].color = new Color(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
                sr[i].sprite = spriteArr[i];
                //generate random value, + another random value to create a large range of randomness
                sr[i].transform.localPosition = new Vector3(Random.Range(-1500f, -750f) + Random.Range(0, 100), Random.Range(-700f, 700.0f) + Random.Range(0, 50), Random.Range(-1, 1));
            }
        }

        //left eye dom, show mondrians to left eye
        if(left.isOn == true)
        {
            for(int i = 0; i < 140; i++)
            {
                spriteArr[i] = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                sr[i].color = new Color(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
                sr[i].sprite = spriteArr[i];
                sr[i].transform.localPosition = new Vector3(Random.Range(-2500f, -1750f) + Random.Range(0, 100), Random.Range(-700f, 700.0f) + Random.Range(0, 50), Random.Range(-1, 1));
            }
        }
        
    }
}
