using UnityEngine;
using Zenject;

public class Main : MonoBehaviour
{
    public GameObject eraseIndicator;

    private Camera _camera;
    
    public bool IsPaused;
   
    [Inject] public void Construct()
    {
        G.main = this;
    }

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void Update()
    {
        var raycastErase = Help.CheckRaycastFromCameraCenter(_camera, LayerMask.GetMask(L.EraseInteract), 15f);
        
        if (raycastErase != null)
        {
            eraseIndicator.SetActive(true);
            eraseIndicator.transform.position = raycastErase.Value.point;
            var radiusErase = 0.2f;
            eraseIndicator.transform.localScale = new Vector3(radiusErase * 2, radiusErase * 2, 1);
        }
        else 
        {
            eraseIndicator.SetActive(false);
        }
    }
}
