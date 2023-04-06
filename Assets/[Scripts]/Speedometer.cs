using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private Text speedLabel;
    [SerializeField] private float maxDegree;
    [SerializeField] private float degreeMultiplier;
    [SerializeField] private Image needle;

    public void UpdateMeter(float speed)
    {
        float convertedSpeed = speed * 3.6f;
        speedLabel.text = $"{convertedSpeed:0} kmh";
        needle.transform.rotation = Quaternion.Euler(0, 0, Mathf.Max(convertedSpeed * degreeMultiplier, maxDegree));
    }
}
