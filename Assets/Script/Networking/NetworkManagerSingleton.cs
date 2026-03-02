using Unity.Netcode;
using UnityEngine;

public class NetworkManagerSingleton : MonoBehaviour
{
    private static NetworkManagerSingleton instance;

    public static NetworkManagerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<NetworkManagerSingleton>();
                if (instance == null)
                {
                    Debug.LogError("No se encontró ninguna instancia de NetworkManagerSingleton en la escena.");
                }
            }
            return instance;
        }
    }

    private NetworkManager networkManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Mantener entre escenas

        // Comprobar si hay NetworkManager
        networkManager = GetComponent<NetworkManager>();
        if (networkManager == null)
        {
            Debug.LogError("Se requiere un componente NetworkManager en este objeto.");
        }
    }

    // Exponer NetworkManager de manera pública
    public NetworkManager NetManager => networkManager;
}