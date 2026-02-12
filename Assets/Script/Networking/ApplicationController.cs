using Networking.Client;
using Networking.Host;
using UnityEngine;
using UnityEngine.Rendering;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClienteSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

    private void Awake()
    {
        // Persista entre escenas
        DontDestroyOnLoad(gameObject);

        // Para detectar servidor dedicado
        bool isDedicatedServer = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;

        if (isDedicatedServer)
        {
            Debug.Log("Iniciando como servidor dedicado...");
            Instantiate(hostPrefab);
        }
        else
        {
            Debug.Log("Iniciando como cliente/host...");
            Instantiate(hostPrefab);
            Instantiate(clientPrefab);
        }
    }
}