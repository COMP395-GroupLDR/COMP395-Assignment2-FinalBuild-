using UnityEngine;
using UnityEngine.UI;

public class CarWheelDisplay : MonoBehaviour
{
    [SerializeField] private Image[] wheels;

    public void UpdateZAngles(int index, float angle)
    {
        wheels[index].transform.rotation = Quaternion.Euler(0, 0, -angle);
    }
}
