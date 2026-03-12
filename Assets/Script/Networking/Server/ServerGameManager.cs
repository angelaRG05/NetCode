using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


namespace Networking.Server
{
    public class ServerGameManager
    {
        private const int MAX_CONNECTIONS = 20;
        private const string GAME_SCENE_NAME = "Game";

        private Allocation allocation;
        private string joinCode;


        public async Task InitAsync()
        {
            await Task.Delay(1000);
            Debug.Log("HostGameManager inicializado correctamente");
        }

        
        public async Task StartHostAsync()
        {
            Debug.Log("Iniciando Host con Relay (Unity Gaming Services)...");

            allocation = await RelayService.Instance.CreateAllocationAsync(MAX_CONNECTIONS);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code generado: {joinCode}");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,          // IP del servidor Relay
                (ushort)allocation.RelayServer.Port,  // Puerto del servidor Relay
                allocation.AllocationIdBytes,         // ID de la asignación (bytes)
                allocation.Key,                       // Clave de cifrado
                allocation.ConnectionData,            // Datos de conexión del host
                allocation.ConnectionData,            // Datos de conexión del host (hostConnectionData)
                true                                  // Conexión segura (DTLS)
            );

            bool started = NetworkManager.Singleton.StartHost();

            if (!started)
            {
                Debug.LogError("No se pudo iniciar el Host");
                return;
            }

            NetworkManager.Singleton.SceneManager.LoadScene(GAME_SCENE_NAME, LoadSceneMode.Single);
            Debug.Log("Host iniciado correctamente con Relay");
        }
    }
}
