using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Text displayText;

    public void Awake()
    {
        displayText = GetComponent<Text>();
    }

    public void SetHealth(int health)
    {
        displayText.text = health.ToString();
    }

    public void SetColor(Color color)
    {
        displayText.color = color;
    }
}
