using UnityEngine;

[RequireComponent(typeof (Camera))]
public class LookAtCameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float targetForwardOffset = -5f;
    [SerializeField] private float speed;

    private Rigidbody targetRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetForward = (targetRigidbody.velocity + target.transform.position).normalized;
        transform.position = Vector3.Lerp(
            transform.position,
            target.position + target.transform.TransformVector(offset) + targetForward * targetForwardOffset,
            speed * Time.deltaTime);

        transform.LookAt(target);
    }
}
