using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Networking.Client
{
    public class ClienteGameManager
    {
        private JoinAllocation allocation;

        // Método asíncrono para iniciar el cliente
        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                Debug.Log($"UnityServices State: {UnityServices.State}");
                Debug.Log($"Is Signed In: {Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn}");
                // Unirse a la asignación existente con Join Code
                allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                Debug.Log("Cliente unido al Relay correctamente");


                // Obtener el transporte de Netcode (Unity Transport),componente que gestiona las conexiones de red
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

                // Configurar manualmente el transporte para usar Relay; SIN usar el package deprecated de Relay
                transport.SetRelayServerData(
                    allocation.RelayServer.IpV4,          // IP del servidor Relay
                    (ushort)allocation.RelayServer.Port,  // Puerto del servidor Relay
                    allocation.AllocationIdBytes,         // ID de la asignación (bytes)
                    allocation.Key,                       // Clave de cifrado
                    allocation.ConnectionData,            // Datos de conexión del host
                    allocation.HostConnectionData,            // Datos de conexión del host (hostConnectionData)
                    true                                  // Conexión segura (DTLS)
                );

                // Iniciar cliente
                NetworkManager.Singleton.StartClient();
                Debug.Log("Cliente iniciado correctamente");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error al unir al Relay: {e.Message}");
            }
        }
        public async Task InitAsync()
        {
            // Inicializamos servicios de unity
            Debug.Log("Inicializando Unity Services...");

            await UnityServices.InitializeAsync();

            Debug.Log("Unity Services inicializados");

            // intento de autenticación
            AuthState authResult = await AuthenticationWrapper.DoAuth(3);

            // Si inicio exitoso
            if (authResult == AuthState.Authenticated)
            {
                GoToMenu(); // vamos al menu 
            }
            else
            {
                Debug.LogError("Fallo en autenticación");
            }
        }

        private void GoToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}