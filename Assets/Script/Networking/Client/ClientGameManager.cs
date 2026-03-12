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
        // Evitamos suscribir callbacks varias veces si el usuario pulsa el botón repetidamente
        private bool callbacksHooked = false;

        // Método asíncrono para iniciar el cliente
        //public async Task StartClientAsync(string joinCode)
        //{
        //    try
        //    {
        //        Debug.Log($"UnityServices State: {UnityServices.State}");
        //        Debug.Log($"Is Signed In: {Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn}");
        //        // Unirse a la asignación existente con Join Code
        //        allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        //        Debug.Log("Cliente unido al Relay correctamente");


        //        // Obtener el transporte de Netcode (Unity Transport),componente que gestiona las conexiones de red
        //        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //        // Configurar manualmente el transporte para usar Relay; SIN usar el package deprecated de Relay
        //        transport.SetRelayServerData(
        //            allocation.RelayServer.IpV4,          // IP del servidor Relay
        //            (ushort)allocation.RelayServer.Port,  // Puerto del servidor Relay
        //            allocation.AllocationIdBytes,         // ID de la asignación (bytes)
        //            allocation.Key,                       // Clave de cifrado
        //            allocation.ConnectionData,            // Datos de conexión del host
        //            allocation.HostConnectionData,            // Datos de conexión del host (hostConnectionData)
        //            true                                  // Conexión segura (DTLS)
        //        );

        //        // Iniciar cliente
        //        NetworkManager.Singleton.StartClient();
        //        Debug.Log("Cliente iniciado correctamente");
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogError($"Error al unir al Relay: {e.Message}");
        //    }
        //}

        public async Task<bool> StartClientAsync(string joinCode)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(joinCode))
            {
                Debug.LogError("StartClientAsync: joinCode vacío o null.");
                return false;
            }

            // NetworkManager
            NetworkManager nm = NetworkManager.Singleton;
            if (nm == null)
            {
                Debug.LogError("NetworkManager.Singleton es null. ¿Tienes un NetworkManager en escena?");
                return false;
            }

            // Si ya está en marcha, no intentamos arrancar otra vez
            if (nm.IsClient || nm.IsServer || nm.IsHost)
            {
                Debug.LogWarning("StartClientAsync: Ya hay una instancia de Netcode arrancada (IsClient/IsServer/IsHost). No se vuelve a iniciar.");
                return false;
            }

            // Aseguramos Services inicializados (por si llamas desde el menú sin pasar por InitAsync)
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            // Aseguramos autenticación (Relay la necesita)
            if (!Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn ||
                !Unity.Services.Authentication.AuthenticationService.Instance.IsAuthorized)
            {
                AuthState auth = await AuthenticationWrapper.DoAuth(5);
                Debug.Log("Auth result (StartClientAsync): " + auth);

                if (auth != AuthState.Authenticated)
                {
                    Debug.LogError("No se pudo autenticar antes de unirse a Relay. Estado: " + auth);
                    return false;
                }
            }

            JoinAllocation allocation = default;

            // 1) Unirse a la asignación (Join Allocation) con try/catch
            try
            {
                allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                Debug.Log("JoinAllocationAsync OK. Relay IP: " + allocation.RelayServer.IpV4 + " Port: " + allocation.RelayServer.Port);
            }
            catch (RelayServiceException ex)
            {
                Debug.LogError("JoinAllocationAsync falló: " + ex);
                return false;
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError("JoinAllocationAsync (RequestFailed) falló: " + ex);
                return false;
            }

            UnityTransport transport = nm.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("No hay UnityTransport en el mismo objeto que el NetworkManager.");
                return false;
            }

            // 2) Configurar el Transport para Relay
            // IMPORTANTÍSIMO: Debe coincidir con el Host.
            // Tu Host está usando isSecure = false, así que aquí también va false.
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData,
                false // <- ALINEADO con el Host (UDP no seguro)
            );

            // 2.1) Hooks de debug para saber si conecta o se desconecta
            if (!callbacksHooked)
            {
                callbacksHooked = true;

                nm.OnClientConnectedCallback += (id) =>
                {
                    Debug.Log("CLIENT: Conectado al host. ClientId=" + id);
                };

                nm.OnClientDisconnectCallback += (id) =>
                {
                    Debug.LogError("CLIENT: Desconectado. ClientId=" + id);
                };
            }

            // 3) Iniciar el modo Client
            bool started = nm.StartClient();
            Debug.Log("CLIENT: StartClient() => " + started);

            if (!started)
            {
                Debug.LogError("NetworkManager no pudo arrancar como Client (StartClient devolvió false).");
                return false;
            }

            Debug.Log("CLIENT iniciado con Relay. JoinCode usado: " + joinCode);
            // OJO: No cargamos Game aquí. Si Enable Scene Management está activado,
            // el host sincroniza la escena y el cliente la carga automáticamente al conectar.
            return true;
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