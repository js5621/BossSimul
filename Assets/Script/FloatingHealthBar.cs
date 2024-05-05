using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider slider;

    public void UpdateHeathBar(float currentValue,float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
