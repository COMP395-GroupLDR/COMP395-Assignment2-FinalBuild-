using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public static CarController instance;
    [System.Serializable]
    public class Wheel
    {
        public WheelCollider WheelCollider;
        public Transform WheelTransform;
        [Tooltip("To be instantiated by script")]
        public ParticleSystem smokeParticle;
        [Range(0f, 1f)]
        public float footBrakeWeight;
        [Range(0f, 1f)]
        public float handBrakeWeight;
    }

    public enum DirectionalLightMode
    {
        OFF = 0,
        LEFT_ON = 1,
        RIGHT_ON = 2,
    }

    public enum Mode
    {
        PARK = 0,
        REVERSE = 1,
        NEUTRAL = 2,
        DRIVE = 3,
    }

    [Header("Settings")]
    [SerializeField] private CarAudioManager audioManager;
    [SerializeField] private Wheel[] wheels;
    [SerializeField] private float slipSmokeAllowance;
    [SerializeField] private float motorPower;
    [SerializeField] private float brakePower;
    [SerializeField] private float minSteeringAngle;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private AnimationCurve steeringCurve;
    [SerializeField] private ParticleSystem smokePrefab;

    [Header("Input Key Codes")]
    [SerializeField] private KeyCode startCarKeyCode;
    [SerializeField] private KeyCode hornKeyCode;
    [SerializeField] private KeyCode seatBeltKeyCode;
    [SerializeField] private KeyCode handBrakeKeyCode;
    [SerializeField] private KeyCode turnLeftLightKeyCode;
    [SerializeField] private KeyCode turnRightLightKeyCode;
    [SerializeField] private KeyCode changeUpperModeKeyCode;
    [SerializeField] private KeyCode changeLowerModeKeyCode;
    [SerializeField] private KeyCode footBrakeKeyCode;

    [Header("Debug")]
    [SerializeField] private bool started;
    //[SerializeField] private float slipAngle;
    [SerializeField] private float speed;
    [SerializeField] private float gasInput;
    [SerializeField] private int handBrakeInput; // parking brake
    [SerializeField] private int footBrakeInput;
    [SerializeField] private float steeringInput;
    [SerializeField] public DirectionalLightMode lightMode;
    [SerializeField] public bool seatBeltTightened;
    [SerializeField] public Mode mode;

    private Rigidbody rb;

    // Callback
    private Action<CarIconDisplay.IconState> seatBeltCallback;
    private Action<CarIconDisplay.IconState> leftLightCallback;
    private Action<CarIconDisplay.IconState> rightLightCallback;
    private Action<CarIconDisplay.IconState> pModeCallback;
    private Action<CarIconDisplay.IconState> rModeCallback;
    private Action<CarIconDisplay.IconState> nModeCallback;
    private Action<CarIconDisplay.IconState> dModeCallback;
    private Action<float> speedUpdateCallback;
    private Action<int, float> carWheelAngleUpdateCallback;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        InstantiateSmoke();
        RegisterCallback();
    }

    private void InstantiateSmoke()
    {
        foreach (var wheel in wheels)
        {
            wheel.smokeParticle = Instantiate(
                smokePrefab,
                wheel.WheelCollider.transform.position - Vector3.up * wheel.WheelCollider.radius,
                Quaternion.identity,
                wheel.WheelCollider.transform);
        }
    }

    private void RegisterCallback()
    {
        CarIconDisplay[] carIconDisplays = FindObjectsOfType<CarIconDisplay>();
        foreach (CarIconDisplay icon in carIconDisplays)
        {
            switch (icon.GetIconType())
            {
                case CarIconDisplay.CarIconDisplayType.SEAT_BELT:
                    seatBeltCallback = icon.ChangeState;
                    break;
                case CarIconDisplay.CarIconDisplayType.LEFT_LIGHT:
                    leftLightCallback = icon.ChangeState;
                    break;
                case CarIconDisplay.CarIconDisplayType.RIGHT_LIGHT:
                    rightLightCallback = icon.ChangeState;
                    break;
                case CarIconDisplay.CarIconDisplayType.MODE_P:
                    pModeCallback = icon.ChangeState;
                    break;
                case CarIconDisplay.CarIconDisplayType.MODE_R:
                    rModeCallback = icon.ChangeState;
                    break;
                case CarIconDisplay.CarIconDisplayType.MODE_N:
                    nModeCallback = icon.ChangeState;
                    break;
                case CarIconDisplay.CarIconDisplayType.MODE_D:
                    dModeCallback = icon.ChangeState;
                    break;
                default:
                    Debug.LogWarning($"Please add switch case for {icon.GetIconType()}.");
                    break;
            }
        }

        Speedometer speedometer = FindObjectOfType<Speedometer>();
        speedUpdateCallback = speedometer.UpdateMeter;

        CarWheelDisplay carWheelDisplay = FindObjectOfType<CarWheelDisplay>();
        carWheelAngleUpdateCallback = carWheelDisplay.UpdateZAngles;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWheels();
        GetSpeed();
        GetInput();
        Brake();
        Move();
        Rotate();
        EmitSlipSmoke();
    }

    private void GetSpeed()
    {
        speed = rb.velocity.magnitude;
        speedUpdateCallback?.Invoke(speed);
    }

    private void GetInput()
    {
        if (started)
        {
            if (Input.GetKeyDown(hornKeyCode))
            {
                audioManager.PlayAudio(CarAudioManager.AudioType.HORN, false);
            }

            if (Input.GetKeyDown(turnLeftLightKeyCode))
            {
                ChangeLightMode(DirectionalLightMode.LEFT_ON);
                GameController.instance.CarState();
            }

            if (Input.GetKeyDown(turnRightLightKeyCode))
            {
                ChangeLightMode(DirectionalLightMode.RIGHT_ON);
                GameController.instance.CarState();
            }

            float positiveInput = Mathf.Max(Input.GetAxis("Vertical"), 0f); // only take "upward"
            gasInput = mode == Mode.REVERSE ? - positiveInput : positiveInput; 

            if (gasInput > 0)
            {
                audioManager.PlayAudio(CarAudioManager.AudioType.ENGINE_RUN, true, false);
            }
            else
            {
                audioManager.StopAudio(CarAudioManager.AudioType.ENGINE_RUN);
            }

            steeringInput = Input.GetAxis("Horizontal");

            //slipAngle = Vector3.Angle(transform.forward, rb.velocity - transform.forward);
        }

        if (Input.GetKey(footBrakeKeyCode))
        {
            audioManager.PlayAudio(CarAudioManager.AudioType.CAR_STOPPING, false, false);
            footBrakeInput = 1;
            gasInput = 0;
        }
        else
        {
            footBrakeInput = 0;
        }

        if (Input.GetKeyDown(startCarKeyCode))
        {
            started = !started;
            audioManager.PlayAudio(CarAudioManager.AudioType.KEY_TURN, false);

            if (started)
            {
                audioManager.PlayAudio(CarAudioManager.AudioType.ENGINE_START, false);
                audioManager.PlayAudio(CarAudioManager.AudioType.DRIVING, true);

                seatBeltCallback?.Invoke(seatBeltTightened ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.WARNING);
            }
            else
            {
                audioManager.PlayAudio(CarAudioManager.AudioType.ENGINE_STOP, false);
                audioManager.StopAudio(CarAudioManager.AudioType.DRIVING);
                audioManager.StopAudio(CarAudioManager.AudioType.HORN);

                seatBeltCallback?.Invoke(CarIconDisplay.IconState.DISABLED);

                lightMode = DirectionalLightMode.OFF;
                audioManager.StopAudio(CarAudioManager.AudioType.DIRECTIONAL_LIGHT_ON);
                leftLightCallback?.Invoke(CarIconDisplay.IconState.DISABLED);
                rightLightCallback?.Invoke(CarIconDisplay.IconState.DISABLED);
            }
        }

        if (Input.GetKeyDown(seatBeltKeyCode))
        {
            seatBeltTightened = !seatBeltTightened;
            GameController.instance.CarState();

            audioManager.PlayAudio(CarAudioManager.AudioType.SEATBELT, false);

            if (started)
            {
                seatBeltCallback?.Invoke(seatBeltTightened ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.WARNING);
            }
        }

        if (Input.GetKeyDown(handBrakeKeyCode))
        {
            audioManager.PlayAudio(CarAudioManager.AudioType.HANDBRAKE, false);

            handBrakeInput = handBrakeInput == 1 ? 0 : 1;
        }

        if (Input.GetKeyDown(changeUpperModeKeyCode))
        {
            audioManager.PlayAudio(CarAudioManager.AudioType.MODE_BUTTON, false);
            ChangeMode(true);
        }

        if (Input.GetKeyDown(changeLowerModeKeyCode))
        {
            audioManager.PlayAudio(CarAudioManager.AudioType.MODE_BUTTON, false);
            ChangeMode(false);
        }
    }

    private void ChangeLightMode(DirectionalLightMode input)
    {
        if (lightMode != input)
        {
            lightMode = input;
            audioManager.PlayAudio(CarAudioManager.AudioType.DIRECTIONAL_LIGHT_ON, true);
        }
        else
        {
            lightMode = DirectionalLightMode.OFF;
            audioManager.StopAudio(CarAudioManager.AudioType.DIRECTIONAL_LIGHT_ON);
        }

        leftLightCallback?.Invoke(lightMode == DirectionalLightMode.LEFT_ON ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.OFF);
        rightLightCallback?.Invoke(lightMode == DirectionalLightMode.RIGHT_ON ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.OFF);
    }

    private void ChangeMode(bool up)
    {
        if (up)
        {
            mode = (int)mode - 1 < 0 ? mode : (Mode)((int)mode - 1);
            GameController.instance.CarState();
        }
        else
        {
            mode = (int)mode + 1 > (int)Mode.DRIVE ? mode : (Mode)((int)mode + 1);
            GameController.instance.CarState();
        }

        pModeCallback?.Invoke(mode == Mode.PARK ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.OFF);
        rModeCallback?.Invoke(mode == Mode.REVERSE ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.OFF);
        nModeCallback?.Invoke(mode == Mode.NEUTRAL ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.OFF);
        dModeCallback?.Invoke(mode == Mode.DRIVE ? CarIconDisplay.IconState.ON : CarIconDisplay.IconState.OFF);
    }

    private void UpdateWheels()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            {
                UpdateWheel(wheels[i], i);
            }
        }
    }

    private void UpdateWheel(Wheel wheel, int index)
    {
        wheel.WheelCollider.GetWorldPose(out Vector3 position, out Quaternion quaternion);
        wheel.WheelTransform.position = position;
        wheel.WheelTransform.rotation = quaternion;
        carWheelAngleUpdateCallback?.Invoke(index, wheel.WheelTransform.localEulerAngles.y);
    }

    private void Move()
    {
        wheels[2].WheelCollider.motorTorque = motorPower * gasInput;
        wheels[3].WheelCollider.motorTorque = motorPower * gasInput;
    }

    private void Rotate()
    {

        float steeringAngle = wheels[0].WheelCollider.steerAngle;

        if (steeringInput != 0)
        {
            steeringAngle += steeringInput * steeringCurve.Evaluate(speed);

        }

        if (gasInput > 0)
        {
            float adjustmentAngle = Vector3.SignedAngle(transform.forward, rb.velocity + transform.forward, Vector3.up);
            //print("Adj angle: " + adjustmentAngle);
            //print("Steering angle:" +  steeringAngle);
            if(steeringAngle > 0)
            {
                steeringAngle -= adjustmentAngle;
                steeringAngle = Mathf.Clamp(steeringAngle, 0, maxSteeringAngle);
            }
            else
            {
                steeringAngle -= adjustmentAngle;
                steeringAngle = Mathf.Clamp(steeringAngle, minSteeringAngle, 0);
            } 
        }

        steeringAngle = Mathf.Clamp(steeringAngle, minSteeringAngle, maxSteeringAngle);


        //float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);

        //print("Steering angle before checking for gas: " + steeringAngle);        

        //if (gasInput > 0)
        //{
        //    steeringAngle += Vector3.SignedAngle(transform.forward, rb.velocity + transform.forward, Vector3.up);
        //}
        //else if (steeringInput != 0)
        //{
        //    steeringAngle += wheels[0].WheelCollider.steerAngle * steeringCurve.Evaluate(speed);
        //    print("Else statment steering angle: " + steeringAngle);
        //}

        //steeringAngle = Mathf.Clamp(steeringAngle, -60f, 60f);

        //print("Steering multiplier" + steeringCurve.Evaluate(speed));
        //print("Steering Angle after gas: " + steeringAngle);

        wheels[0].WheelCollider.steerAngle = steeringAngle;
        wheels[1].WheelCollider.steerAngle = steeringAngle;
    }

    private void Brake()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.WheelCollider.brakeTorque = footBrakeInput * brakePower * wheel.footBrakeWeight 
                + handBrakeInput * brakePower * wheel.handBrakeWeight;
        }
    }

    private void EmitSlipSmoke()
    {
        WheelHit[] wheelHits = new WheelHit[wheels.Length];

        for (int i = 0; i < wheels.Length;i++)
        {
            wheels[i].WheelCollider.GetGroundHit(out wheelHits[i]);

            if (Mathf.Abs(wheelHits[i].sidewaysSlip) + Mathf.Abs(wheelHits[i].forwardSlip) > slipSmokeAllowance)
            {
                // Emit smoke
                wheels[i].smokeParticle.Play();
            }
            else
            {
                // Stop smoke
                wheels[i].smokeParticle.Stop();
            }
        }
    }
}
