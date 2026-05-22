using UnityEngine;

public class PickupableWeapon : MonoBehaviour
{
    public string weaponName = "Arma";

    [Header("Tiro")]
    public int damage = 1;
    public float shootRange = 100f;
    public float fireCooldown = 0.3f;

    [Header("Posição na mão")]
    public Vector3 holdPosition = new Vector3(0.35f, -0.25f, 0.7f);
    public Vector3 holdRotation = new Vector3(0f, 180f, 0f);

    public bool changeScaleOnPickup = false;
    public Vector3 holdScale = Vector3.one;

    [Header("Soltar arma")]
    public float dropForwardForce = 2f;
    public float dropUpForce = 1f;
    public float dropTorque = 4f;

    private Collider[] colliders;
    private Rigidbody rb;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();

        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = 1f;
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void PickUp(Transform weaponHolder)
    {
        transform.SetParent(weaponHolder, false);

        transform.localPosition = holdPosition;
        transform.localRotation = Quaternion.Euler(holdRotation);

        if (changeScaleOnPickup)
        {
            transform.localScale = holdScale;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void Drop(Transform cameraTransform)
{
    transform.SetParent(null);

    transform.position = cameraTransform.position + cameraTransform.forward * 1.2f;
    transform.rotation = cameraTransform.rotation;

    foreach (Collider col in colliders)
    {
        col.enabled = true;
    }

    rb.isKinematic = false;
    rb.useGravity = true;

    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    // Impulso leve para frente e para cima
    Vector3 dropDirection = cameraTransform.forward * dropForwardForce + cameraTransform.up * dropUpForce;

    rb.AddForce(dropDirection, ForceMode.Impulse);

    // Impede a arma de sair girando
    rb.angularVelocity = Vector3.zero;
}
  }