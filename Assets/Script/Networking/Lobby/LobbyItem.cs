using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayersText;

    // Referencia a la lista de lobbies "padre"
    private LobbyList lobbyList;

    // Almacena el lobby que representa este item
    private Lobby lobby;

    // Método para inicializar los datos en el item
    public void Initialize(LobbyList lobbyList, Lobby lobby)
    {
        // Guardamos la referencia a la lista de lobbies
        this.lobbyList = lobbyList;

        // Guardamos también el lobby que representa
        this.lobby = lobby;

        // Actualizamos el texto con el nombre del lobby
        lobbyNameText.text = lobby.Name;

        // Mostramos cuántos jugadores hay actualmente y cuántos caben
        lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    // Método que se llamará al pulsar el botón de "Unirse" en este item
    public void OnClick_Join()
    {
        // Llamamos a la función JoinAsync del script LobbyList,
        // pasándole el Lobby al que queremos unirnos.
        lobbyList.JoinAsync(lobby);
    }
}