using UnityEngine;
using UnityEngine.UI;

public class BurbujasHUD : MonoBehaviour
{
    private Image[] burbujas;

    public static BurbujasHUD Crear(int cantidad)
    {
        GameObject canvasGO = new GameObject("HUDBurbujas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        BurbujasHUD hud = canvasGO.AddComponent<BurbujasHUD>();
        hud.burbujas = new Image[cantidad];
        Sprite sprite = BuscarSprite();

        for (int i = 0; i < cantidad; i++)
        {
            GameObject burbuja = new GameObject("Burbuja" + (i + 1), typeof(RectTransform), typeof(Image));
            burbuja.transform.SetParent(canvasGO.transform, false);

            RectTransform rt = burbuja.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(30 + i * 60, -30);
            rt.sizeDelta = new Vector2(50, 50);

            Image imagen = burbuja.GetComponent<Image>();
            imagen.sprite = sprite;
            imagen.preserveAspect = true;
            imagen.raycastTarget = false;
            hud.burbujas[i] = imagen;
        }

        return hud;
    }

    private static Sprite BuscarSprite()
    {
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprites/bubbles"))
        {
            if (sprite.name == "bubbles_2")
                return sprite;
        }
        Debug.LogWarning("BurbujasHUD: no se ha encontrado el sprite bubbles_2");
        return null;
    }

    public void Mostrar(int cantidad)
    {
        for (int i = 0; i < burbujas.Length; i++)
            burbujas[i].enabled = i < cantidad;
    }
}
