using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    // Variable que pueda manejar el servidor y sea vista por los clientes
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    [field: SerializeField]
    public int MaxHealth { get; private set; } = 100;
    private bool isDead = false;
    public event Action<Health> OnDie;

    public override void OnNetworkSpawn()
    {
        // Solo el servidor inicializa la salud
        if (!IsServer) return;

        currentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        if (!IsServer) return;

        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        if (!IsServer) return;

        ModifyHealth(healValue);
    }

    // Metodo para modificar la salud
    private void ModifyHealth(int value)
    {
        // Si ya está muerto, no hacer nada
        if (isDead) return;

        int newHealth = currentHealth.Value + value;

        // Limitar la salud entre 0 y MaxHealth
        currentHealth.Value = Mathf.Clamp(newHealth, 0, MaxHealth);

        // Comprobar muerte
        if (currentHealth.Value == 0)
        {
            isDead = true;
            // Avisa que el jugador ha muerto
            OnDie?.Invoke(this);
        }
    }
}
