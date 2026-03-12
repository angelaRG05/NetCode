using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


namespace Networking.Host
{
    public class HostGameManager
    {
        private const int MAX_CONNECTIONS = 20;
        private const string GAME_SCENE_NAME = "Game";

        private Allocation allocation;
        private string joinCode;
        public async Task InitAsync()
        {
            Debug.Log("Inicializando HostGameManager...");

            // Simulación de inicialización de servidor y cliente
            await Task.Delay(1000);

            Debug.Log("HostGameManager inicializado correctamente");
        }

        // Método principal para iniciar una partida como Host usando Relay (UGS puro).
        public async Task StartHostAsync()
        {
            Debug.Log("Iniciando Host con Relay (Unity Gaming Services)...");

            // Crear una asignación en Relay
            // Relay reserva un servidor intermedio para nuestras conexiones
            allocation = await RelayService.Instance.CreateAllocationAsync(MAX_CONNECTIONS);

            // Obtener el Join Code, se compartirá con los clientes
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Join Code generado: {joinCode}");

            // Asignar el joincode al HosSingleton 
            HostSingleton.Instance.CurrentJoinCode = joinCode.ToUpper();

            // Obtener el transporte de Netcode (Unity Transport),componente que gestiona las conexiones de red
            NetworkManager networkmanager = NetworkManager.Singleton;
            if (networkmanager == null)
            {
                Debug.LogError("NetworkManager.Singleton es null, asegurate tener networkmaanger en el NetBoostrap");
                return;
            }

            UnityTransport transport = networkmanager.GetComponent<UnityTransport>();

            if(transport == null)
            {
                Debug.LogError("No se encontro UnityTransport en el mismo GameObject que el NetworkManager");
                return;
            }

            // Configurar manualmente el transporte para usar Relay
            string relayIp = allocation.RelayServer.IpV4;
            ushort relayPort = (ushort)allocation.RelayServer.Port;

            byte[] allocationIdBytes = allocation.AllocationIdBytes;
            byte[] key = allocation.Key;
            byte[] connectionData = allocation.ConnectionData;
            byte[] hostConnectionData = allocation.ConnectionData;

            bool isSecure = false;

            Debug.Log("HOST Relay Config => IP: " + relayIp + " Port: " + relayPort + " Secure: " + isSecure);

            transport.SetRelayServerData(
                relayIp,
                relayPort,
                allocationIdBytes,
                key,
                connectionData,
                hostConnectionData,
                isSecure
            );

            // Iniciar el modo Host, actúa como servidor + cliente local
            bool started = NetworkManager.Singleton.StartHost();

            if (!started)
            {
                Debug.LogError("No se pudo iniciar el Host");
                return;
            }

            // 6Cargar la escena de juego, Netcode se encarga de sincronizar la escena con los clientes
            NetworkManager.Singleton.SceneManager.LoadScene(
                GAME_SCENE_NAME,
                LoadSceneMode.Single
            );

            Debug.Log("Host iniciado correctamente con Relay");
        }
    }
}
