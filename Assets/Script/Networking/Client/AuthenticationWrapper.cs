using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

public enum AuthState
{
    NotAuthenticated,   // Aún no se ha iniciado sesión
    Authenticating,     // Autenticación en proceso
    Authenticated,      // Login correcto
    Error,              // Error durante el login
    Timeout             // Se agotaron los reintentos
}

public class AuthenticationWrapper 
{
    // referencia publica pero setear valor privado
    public static AuthState State { get; private set; } = AuthState.NotAuthenticated;
   
    // Se llama una vez (boton) decide que hacer segun el estado actual
    public static async Task<AuthState> DoAuth(int maxRetries = 3)
    {
        // Si ya está autenticado, no hacemos nada
        if (State == AuthState.Authenticated)
            return State;

        // Si ya hay un proceso en marcha, esperamos
        if (State == AuthState.Authenticating)
            return await WaitForAuthentication();

        // Si nadie ha hecho un proceso, procesa 
        State = AuthState.Authenticating;
        await SignInAnonymouslyAsync(maxRetries);

        return State;
    }

    // 
    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        // Maximo de reintentos 3
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Debug.Log($"[Auth] Intento {attempt} de {maxRetries}");

                // Intento login
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    State = AuthState.Authenticated;
                    Debug.Log("[Auth] Autenticación exitosa");
                    return;
                }
            }
            catch (AuthenticationException e)
            {
                Debug.LogError($"[Auth] AuthenticationException: {e}");
            }
            catch (RequestFailedException e)
            {
                Debug.LogError($"[Auth] RequestFailedException: {e}");
            }

            await Task.Delay(1000); // Espera 1 segundo antes de reintentar
        }

        State = AuthState.Timeout;
        Debug.LogWarning("[Auth] Se agotaron los reintentos de autenticación");
    }

    private static async Task<AuthState> WaitForAuthentication()
    {
        while (State == AuthState.Authenticating)
        {
            await Task.Delay(200);
        }

        return State;
    }

}
