using UnityEngine;
using Unity.Netcode;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject serverProjectilePrefab;
    [SerializeField]
    private GameObject clientProjectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private InputReader inputReader;
    [SerializeField]
    private GameObject muzzleFlash;
    [SerializeField]
    private Collider2D playerCollider;


    [Header("Settings")]
    [SerializeField]
    private float projectileSpeed;
    private bool shouldFire = false;
    [SerializeField]
    private float fireRate = 1f; // Disparos por segundo
    private float previousFireTime = 0f;

    [SerializeField]
    private float muzzleFlashDuration = 0.075f;
    private float muzzleFlashTimer = 0f;

    private void Start()
    {
        muzzleFlash.SetActive(false);
    }

    // Manejo evento disparo
    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    // Suscribirse OnNetworkSpawn si es el propietario 
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    // Desuscribirse o destruir el el objeto para evitar errores
    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            inputReader.PrimaryFireEvent -= HandlePrimaryFire;
        }
    }

    // Creacion proyectil dummy cliente 
    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(
            clientProjectilePrefab,
            spawnPos,
            Quaternion.identity
        );

        var projectileCollider = projectileInstance.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, projectileCollider);

        // Orientamos
        projectileInstance.transform.up = direction;

        // Disparamos con movimiento
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rb))
        {
            // Importante en 2D usar "transform.up" en lugar de "transform.forward"
            rb.velocity = direction * projectileSpeed;
        }


        // MuzzleFlash
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

    }

    // Creacion proyectil real server
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // Instanciar el proyectil real
        GameObject projectileInstance = Instantiate(
        serverProjectilePrefab,
        spawnPos,
        Quaternion.identity
        );

        var projectileCollider = projectileInstance.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, projectileCollider);

        projectileInstance.transform.up = direction;

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rb))
        {
            // Importante en 2D usar "transform.up" en lugar de "transform.forward"
            rb.velocity = direction * projectileSpeed;
        }

        // Notificar a todos los clientes
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    // Creacion disparo proyectil dummy cliente REMOTO
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        // Evita crear doble proyectil en quien disparo
        if (IsOwner) return; 
        SpawnDummyProjectile(spawnPos, direction);
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!shouldFire) return;

        if (Time.time < previousFireTime + (1f / fireRate))
        {
            return; // Aún no ha pasado el tiempo suficiente para disparar
        }

        // Si pasa la validación:
        previousFireTime = Time.time;
        
        // 1. Crear proyectil local
        SpawnDummyProjectile(projectileSpawnPoint.position,
        projectileSpawnPoint.up);



        // 2. Avisar al servidor
        PrimaryFireServerRpc(projectileSpawnPoint.position,
        projectileSpawnPoint.up);
    }



}
