using System;
using UnityEngine;
using TMPro;

public class WeaponInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public Transform weaponHolder;

    [Header("Bala")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 40f;
    public Vector3 bulletRotationOffset = new Vector3(90f, 0f, 0f);

    [Header("Interação")]
    public float pickupRange = 3f;
    public TMP_Text interactionText;

    private PickupableWeapon currentWeapon;
    private float nextShotTime;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    void Update()
    {
        UpdateInteractionText();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpWeapon();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
        }
    }

    void UpdateInteractionText()
    {
        if (interactionText == null)
        {
            return;
        }

        if (currentWeapon == null && weaponHolder != null)
        {
            currentWeapon = weaponHolder.GetComponentInChildren<PickupableWeapon>();
        }

        if (currentWeapon != null)
        {
            interactionText.text = "<size=18><color=#FFFFFF99>G soltar arma</color></size>";
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            PickupableWeapon weapon = hit.collider.GetComponentInParent<PickupableWeapon>();

            if (weapon != null)
            {
                interactionText.text = "<size=18><color=#FFFFFF99>E pegar arma</color></size>";
                return;
            }
        }

        interactionText.text = "";
    }

    void TryPickUpWeapon()
    {
        if (currentWeapon != null)
        {
            Debug.Log("Você já está segurando uma arma.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            PickupableWeapon weapon = hit.collider.GetComponentInParent<PickupableWeapon>();

            if (weapon != null)
            {
                currentWeapon = weapon;
                currentWeapon.PickUp(weaponHolder);

                Debug.Log("Você pegou: " + weapon.weaponName);
            }
        }
    }

    void Shoot()
    {
        if (currentWeapon == null && weaponHolder != null)
        {
            currentWeapon = weaponHolder.GetComponentInChildren<PickupableWeapon>();
        }

        if (currentWeapon == null)
        {
            Debug.Log("Pegue uma arma primeiro apertando E.");
            return;
        }

       if (Time.time < nextShotTime)
{
    return;
}

if (GameManager.Instance != null)
{
    if (GameManager.Instance.IsGameOver)
    {
        return;
    }

    if (!GameManager.Instance.TryUseAmmo())
    {
        Debug.Log("Sem munição ou jogo finalizado.");
        return;
    }
}
        nextShotTime = Time.time + currentWeapon.fireCooldown;

        Ray cameraRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        Vector3 targetPoint = playerCamera.transform.position + playerCamera.transform.forward * currentWeapon.shootRange;

        RaycastHit[] hits = Physics.RaycastAll(
            cameraRay,
            currentWeapon.shootRange,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (currentWeapon != null && hit.collider.transform.IsChildOf(currentWeapon.transform))
            {
                continue;
            }

            targetPoint = hit.point;

            TargetHit target = hit.collider.GetComponentInParent<TargetHit>();

            if (target != null)
            {
                target.TakeHit(currentWeapon.damage);
            }

            break;
        }

        if (bulletPrefab != null && firePoint != null)
        {
            Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

            Quaternion bulletRotation =
                Quaternion.LookRotation(shootDirection) *
                Quaternion.Euler(bulletRotationOffset);

            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                bulletRotation
            );

            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = shootDirection * bulletSpeed;
            }
        }
        else
        {
            Debug.Log("Bullet Prefab ou Fire Point não foi colocado no Inspector.");
        }

        Debug.Log("Tiro disparado!");
    }

    void DropWeapon()
    {
        if (currentWeapon == null && weaponHolder != null)
        {
            currentWeapon = weaponHolder.GetComponentInChildren<PickupableWeapon>();
        }

        if (currentWeapon == null)
        {
            return;
        }

        currentWeapon.Drop(playerCamera.transform);
        currentWeapon = null;

        Debug.Log("Arma solta.");
    }
}