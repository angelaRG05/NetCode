using System.Threading.Tasks;
using UnityEngine;

namespace Networking.Host
{
    public class HostGameManager
    {
        public async Task InitAsync()
        {
            Debug.Log("Inicializando HostGameManager...");

            // Simulación de inicialización de servidor y cliente
            await Task.Delay(1000);

            Debug.Log("HostGameManager inicializado correctamente");
        }

        // Aquí luego podrás añadir:
        // - Iniciar servidor
        // - Aceptar conexiones
        // - Sincronizar jugadores
        // - Reglas de la partida
    }
}