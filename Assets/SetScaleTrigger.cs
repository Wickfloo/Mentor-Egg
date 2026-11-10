using UnityEngine;

public class SetScaleTrigger : MonoBehaviour
{
    [Header("Scale Settings")]
    [Tooltip("The target scale to apply to the player when entering this trigger.")]
    public Vector3 TargetScale = new Vector3(1f, 1f, 1f);

    [Tooltip("Should this trigger only work once?")]
    public bool TriggerOnce = false;

    private bool _hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TriggerOnce && _hasTriggered) return;

        // Check if the object or its root parent is tagged as "Player"
        if (collision.CompareTag("Player") || collision.transform.root.CompareTag("Player"))
        {
            // Get the absolute root of the player so the entire character scales correctly
            Transform playerRoot = collision.transform.root;
            playerRoot.localScale = TargetScale;

            if (TriggerOnce)
            {
                _hasTriggered = true;
            }
        }
    }
}