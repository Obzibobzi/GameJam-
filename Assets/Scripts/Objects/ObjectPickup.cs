using UnityEngine;
using UnityEngine.UI;

public class ObjectPickup : MonoBehaviour
{
    public float pickupRange = 3f;               // Range within which the player can interact
    public Transform holdPosition;                // Position offset for holding the object
    private GameObject heldObject;                // The currently held object
    private Rigidbody heldObjectRb;               // Rigidbody of the held object
    public float maxThrowForce = 20f;            // Maximum force applied when throwing the object
    public float chargeTime = 2f;                 // Time it takes to fully charge
    private float chargeAmount = 0f;              // Current charge amount
    public Slider chargeSlider;                   // Reference to the UI slider
    public GameObject chargeSliderObject;         // Reference to the GameObject containing the slider

    void Start()
    {
        // Initially hide the charge slider
        if (chargeSliderObject != null)
        {
            chargeSliderObject.SetActive(false);
        }
    }

    void Update()
    {
        // Use left-click to pick up or drop the object
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        // Charge the throw when right-click is held
        if (Input.GetMouseButton(1) && heldObject != null)
        {
            ChargeThrow();
        }

        // Throw the object if right-click is released
        if (Input.GetMouseButtonUp(1) && heldObject != null)
        {
            ThrowObject();
        }
    }

    void TryPickupObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup")) // Ensure the object has the tag "Pickup"
            {
                heldObject = hit.collider.gameObject;
                heldObjectRb = heldObject.GetComponent<Rigidbody>();

                // Ensure Rigidbody exists
                if (heldObjectRb != null)
                {
                    // Disable gravity while holding
                    heldObjectRb.useGravity = false;
                }
            }
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            // Re-enable gravity
            heldObjectRb.useGravity = true;

            heldObject = null;
            heldObjectRb = null;
            chargeAmount = 0f; // Reset charge amount
            chargeSlider.value = chargeAmount; // Reset UI slider

            // Hide the charge slider when not charging
            if (chargeSliderObject != null)
            {
                chargeSliderObject.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void MoveHeldObject()
    {
        // Set the held object’s position to the hold position
        heldObject.transform.position = holdPosition.position; 
    }

    void ChargeThrow()
    {
        // Show the charge slider while charging
        if (chargeSliderObject != null)
        {
            chargeSliderObject.SetActive(true);
        }

        // Increase charge amount based on time held
        chargeAmount += Time.deltaTime / chargeTime;
        chargeAmount = Mathf.Clamp(chargeAmount, 0, 1); // Clamp value between 0 and 1

        // Update the UI slider
        if (chargeSlider != null)
        {
            chargeSlider.value = chargeAmount;
        }
    }

    void ThrowObject()
    {
        // Re-enable gravity
        heldObjectRb.useGravity = true;

        // Calculate throw direction (the forward direction of the camera)
        Vector3 throwDirection = Camera.main.transform.forward;

        // Apply force to the held object's Rigidbody based on charge amount
        float throwForce = chargeAmount * maxThrowForce;
        heldObjectRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);

        // Reset held object and charge amount
        heldObject = null;
        heldObjectRb = null;
        chargeAmount = 0f; // Reset charge amount
        chargeSlider.value = chargeAmount; // Reset UI slider

        // Hide the charge slider after throwing
        if (chargeSliderObject != null)
        {
            chargeSliderObject.SetActive(false);
        }
    }
}
