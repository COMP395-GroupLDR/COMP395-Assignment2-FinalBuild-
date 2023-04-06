using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CarIconDisplay : MonoBehaviour
{
    public enum IconState
    {
        DISABLED = 0,
        WARNING = 1,
        OFF = 2,
        ON = 3
    }

    public enum CarIconDisplayType
    {
        NONE = 0,
        SEAT_BELT = 1,
        LEFT_LIGHT = 2,
        RIGHT_LIGHT = 3,
        MODE_P = 4,
        MODE_R = 5,
        MODE_N = 6,
        MODE_D = 7,
    }

    [Header("Settings")]
    [SerializeField] private CarIconDisplayType carIconDisplayType;
    [SerializeField] private IconState defaultState;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color offColor;
    [SerializeField] private Color onColor;

    [Header("Debug")]
    [SerializeField] private IconState state;

    private Image icon;
    public CarIconDisplayType GetIconType() { return carIconDisplayType; }

    // Start is called before the first frame update
    void Start()
    {
        icon = GetComponent<Image>();
        ChangeState(defaultState);
    }

    public void ChangeState(IconState iconState)
    {
        state = iconState;
        UpdateIconColor();
    }

    [Button("Update Icon Color")]
    private void UpdateIconColor()
    {
        switch (state)
        {
            case IconState.DISABLED:
                icon.color = disabledColor;
                break;
            case IconState.WARNING:
                icon.color = warningColor;
                break;
            case IconState.OFF:
                icon.color = offColor;
                break;
            case IconState.ON:
                icon.color = onColor;
                break;
        }
    }
}
