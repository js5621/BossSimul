using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Slider slider;

    [SerializeField] private float maxValue;

    public void SetMaxHealth(float health)
    {
        this.maxValue = health;
    }

    public void UpdateHeathBar(float currentValue)
    {
        slider.value = currentValue / maxValue;
    }
}
