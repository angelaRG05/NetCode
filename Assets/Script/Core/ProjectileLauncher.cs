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
    [Header("Settings")]
    [SerializeField]
    private float projectileSpeed;
    private bool shouldFire = false;

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
        projectileInstance.transform.up = direction;
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
        projectileInstance.transform.up = direction;
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
        if (!shouldFire) return;
        // 1. Crear proyectil local
        SpawnDummyProjectile(projectileSpawnPoint.position,
        projectileSpawnPoint.up);
        // 2. Avisar al servidor
        PrimaryFireServerRpc(projectileSpawnPoint.position,
        projectileSpawnPoint.up);
    }



}
