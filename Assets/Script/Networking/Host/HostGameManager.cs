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
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            // Configurar manualmente el transporte para usar Relay; SIN usar el package deprecated de Relay
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,          // IP del servidor Relay
                (ushort)allocation.RelayServer.Port,  // Puerto del servidor Relay
                allocation.AllocationIdBytes,         // ID de la asignación (bytes)
                allocation.Key,                       // Clave de cifrado
                allocation.ConnectionData,            // Datos de conexión del host
                allocation.ConnectionData,            // Datos de conexión del host (hostConnectionData)
                true                                  // Conexión segura (DTLS)
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
