using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UnityEventTrigger : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private UnityEvent events;
    [ShowNonSerializedField] private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag(triggerTag))
        {
            triggered = true;
            events?.Invoke();
        }
    }

    void OnValidate()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
