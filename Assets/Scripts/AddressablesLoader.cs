using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

public class AddressablesLoader : MonoBehaviour
{
    [SerializeField] private List<string> _AddressablesPaths;
    private Dictionary<string, AsyncOperationHandle> _addressablesLoading;
    
    private void Start()
    {
        LoadAddressables();
    }
    
    private async Task LoadAddressables()
    {
        _addressablesLoading = new();
        
        foreach (string addressable in _AddressablesPaths)
        {
            AsyncOperationHandle? nullableHandle = addressable.Split('.').Last() switch
            {
                "prefab" => Addressables.LoadAssetAsync<GameObject>(addressable),
                "png" or "jpg" or "jpeg" => Addressables.LoadAssetAsync<Texture>(addressable),
                _ => null
            };

            if (nullableHandle is null)
            {
                Debug.LogError("Unsupported file type given in AddressablesLoader.");
                continue;
            }
            
            AsyncOperationHandle currentHandle = nullableHandle.Value;
            
            _addressablesLoading.Add(addressable, currentHandle);
            currentHandle.Completed += (_) => print(addressable + " loaded");
        }
        
        Task<object[]> allTasks = Task.WhenAll(_addressablesLoading.Values.Select(x => x.Task));
        await allTasks;
        
        print("loaded everything");
    }
}
