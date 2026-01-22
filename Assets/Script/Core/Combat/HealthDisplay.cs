using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [SerializeField]
    private Health health;
    [SerializeField]
    private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return; // Solo clientes necesitan la UI
        health.currentHealth.OnValueChanged += HandleHealthChanged;
        // Llamada manual para inicializar
        HandleHealthChanged(0, health.currentHealth.Value);
    }
    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.currentHealth.OnValueChanged -= HandleHealthChanged;
    }
    
    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        if (healthBarImage == null) return;

        float max = health.MaxHealth <= 0 ? 1f : health.MaxHealth;
        healthBarImage.fillAmount = newHealth / max;
    }
}
