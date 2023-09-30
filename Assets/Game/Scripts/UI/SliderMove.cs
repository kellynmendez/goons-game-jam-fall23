using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMove : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text textExit;
    [SerializeField] private Text textWait;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        textWait.enabled = true;
        textExit.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value += speed * Time.deltaTime;
        textWait.text = (string.Format("{0:0}", slider.value));
        if (slider.value >= slider.maxValue)
        {
            textWait.enabled = false;
            textExit.enabled = true;
        }
    }
}
