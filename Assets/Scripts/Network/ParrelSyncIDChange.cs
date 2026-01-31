using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine;

public class ParrelSyncIDChange : MonoBehaviour
{
    async void Awake()
    {
        var options = new InitializationOptions();

        #if UNITY_EDITOR
        
        // Use ParrelSync profile in Editor clones
        if (ParrelSync.ClonesManager.IsClone())
            options.SetProfile(ParrelSync.ClonesManager.GetArgument());

        #else

        // In build, generate a unique random profile per instance
        string uniqueBuildProfile = System.Guid.NewGuid().ToString();

        options.SetProfile(uniqueBuildProfile);

        #endif

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log($"Signed in as: {AuthenticationService.Instance.PlayerId}");
    }
}