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
    public float bulletSpeed = 50f;
    public Vector3 bulletRotationOffset = new Vector3(90f, 0f, 0f);

    [Header("Interação")]
    public float pickupRange = 3f;
    public TMP_Text interactionText;

    [Header("Mira com botão direito")]
    public float normalFOV = 60f;
    public float aimFOV = 40f;
    public float aimSpeed = 10f;

    public Vector3 normalWeaponHolderPosition;
    public Vector3 aimWeaponHolderPosition = new Vector3(0f, -0.12f, 0.45f);

    private PickupableWeapon currentWeapon;
    private float nextShotTime;

    private bool isAiming;

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

        if (playerCamera != null)
        {
            normalFOV = playerCamera.fieldOfView;
        }

        if (weaponHolder != null)
        {
            normalWeaponHolderPosition = weaponHolder.localPosition;
        }
    }

    void Update()
    {
        UpdateInteractionText();
        HandleAim();

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
            {
                GameManager.Instance.StartReload();
            }
        }
    }

    void HandleAim()
    {
        if (currentWeapon == null && weaponHolder != null)
        {
            currentWeapon = weaponHolder.GetComponentInChildren<PickupableWeapon>();
        }

        isAiming = currentWeapon != null && Input.GetMouseButton(1);

        if (playerCamera != null)
        {
            float targetFOV = isAiming ? aimFOV : normalFOV;
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetFOV,
                aimSpeed * Time.deltaTime
            );
        }

        if (weaponHolder != null)
        {
            Vector3 targetPosition = isAiming ? aimWeaponHolderPosition : normalWeaponHolderPosition;

            weaponHolder.localPosition = Vector3.Lerp(
                weaponHolder.localPosition,
                targetPosition,
                aimSpeed * Time.deltaTime
            );
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
                Debug.Log("Sem munição ou recarregando.");
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

        if (weaponHolder != null)
        {
            weaponHolder.localPosition = normalWeaponHolderPosition;
        }

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = normalFOV;
        }

        Debug.Log("Arma solta.");
    }
}