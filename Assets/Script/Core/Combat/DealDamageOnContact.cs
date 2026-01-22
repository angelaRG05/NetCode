using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : NetworkBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;

    public void SetOwner(ulong newOwnerClientId)
    {
        ownerClientId = newOwnerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null) return;

        // Evitar dañar al propietario del proyectil.
        NetworkObject targetNetObj = col.attachedRigidbody.GetComponent<NetworkObject>();
        if (targetNetObj != null && targetNetObj.OwnerClientId == ownerClientId) return;

        Health targetHealth = col.attachedRigidbody.GetComponent<Health>();
        if (targetHealth != null)
            targetHealth.TakeDamage(damage);
    }
}
