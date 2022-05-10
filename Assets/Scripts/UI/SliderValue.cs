using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValue : MonoBehaviour
{
    Slider slider;
    public TextMeshProUGUI text;
        // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        text.text = slider.value.ToString();
    }

    // Update is called once per frame
    public void UpdateText()
    {
        text.text = slider.value.ToString();
    }
}
