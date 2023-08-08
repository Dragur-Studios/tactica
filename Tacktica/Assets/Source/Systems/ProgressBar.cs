using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    private void OnValidate()
    {
        var slider = GetComponent<Slider>();
        slider.value = invert ? 1 - value : value;
    }

    [Range(0, 1)] public float value = 0;
    public bool invert = false;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = invert ? 1 - value : value;
    }
}
