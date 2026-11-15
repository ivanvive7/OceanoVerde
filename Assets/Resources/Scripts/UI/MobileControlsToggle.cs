using UnityEngine;

public class MobileControlsToggle : MonoBehaviour
{
    [SerializeField] bool forceShow = false;   // para probar en PC

    void Awake()
    {
        bool mostrar = Application.isMobilePlatform || forceShow;
        gameObject.SetActive(mostrar);
    }
}