using Networking.Client;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    // Prefab de cada elemento (item) que se mostrará en la lista
    [SerializeField] private LobbyItem lobbyItemPrefab;

    // Lugar en la jerarquía de la UI donde se generarán los items
    [SerializeField] private Transform lobbyItemParent;

    // Control para evitar que un usuario dispare varias uniones a la vez
    private bool isJoining = false;

    // Control para evitar que la lista se refresque sin parar
    private bool isRefreshing = false;

    // Se llama automáticamente cuando este GameObject se activa.
    // Aquí desencadenamos la lógica de refrescar la lista de lobbies disponibles.
    private void OnEnable()
    {
        RefreshList();
    }

    // Método público por si tenemos un botón "Refrescar" que, al pulsarlo,
    // llama a esta función para forzar la actualización de la lista.
    public void RefreshList()
    {
        RefreshListAsync();
    }

    // Método asíncrono que se encarga de:
    // 1. Evitar refrescos múltiples
    // 2. Crear filtros de lobbies
    // 3. Consultar lobbies en Unity Services
    // 4. Destruir items antiguos
    // 5. Instanciar nuevos items en la UI
    private async void RefreshListAsync()
    {
        if (isRefreshing)
        {
            return;
        }

        isRefreshing = true;

        try
        {
            // Opciones de consulta
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                // Cuántos lobbies como máximo queremos recibir
                Count = 25,

                // Filtros: en este caso, queremos:
                // 1. Lobbies que tengan al menos un espacio libre
                // 2. Lobbies que NO estén bloqueados
                Filters = new List<QueryFilter>
                {
                    // Espacios disponibles > 0
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"
                    ),

                    // Está bloqueado = 0 (es decir, falso)
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.IsLocked,
                        op: QueryFilter.OpOptions.EQ,
                        value: "0"
                    )
                }
            };

            // Consultar lobbies disponibles con los filtros
            QueryResponse lobbiesResult = await LobbyService.Instance.QueryLobbiesAsync(options);

            // 1. Destruir todos los items previos para limpiar la lista
            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // 2. Crear un item para cada lobby encontrado
            foreach (Lobby lobby in lobbiesResult.Results)
            {
                LobbyItem newItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                newItem.Initialize(this, lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        finally
        {
            // Pase lo que pase, al final ya no estamos "refrescando"
            isRefreshing = false;
        }
    }

    public async void JoinAsync(Lobby lobby)
    {
        // Evitamos que un jugador dispare múltiples uniones concurrentes
        if (isJoining)
        {
            return;
        }

        isJoining = true;

        try
        {
            // Unirnos al lobby por medio de su ID
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

            // Leemos el código de unión desde la data del lobby
            if (joinedLobby.Data == null || !joinedLobby.Data.ContainsKey("JoinCode"))
            {
                Debug.LogError("El lobby no contiene la clave 'JoinCode' en Data.");
                return;
            }

            string joinCode = joinedLobby.Data["JoinCode"].Value;

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                Debug.LogError("El JoinCode del lobby está vacío.");
                return;
            }

            // Llamamos a la lógica del GameManager del cliente para conectar
            if (ClienteSingleton.Instance == null || ClienteSingleton.Instance.GameManager == null)
            {
                Debug.LogError("ClientSingleton o ClientGameManager no están disponibles.");
                return;
            }

            bool ok = await ClienteSingleton.Instance.GameManager.StartClientAsync(joinCode);

            if (!ok)
            {
                Debug.LogError("No se pudo iniciar el cliente usando el JoinCode del lobby.");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        finally
        {
            isJoining = false;
        }
    }
}