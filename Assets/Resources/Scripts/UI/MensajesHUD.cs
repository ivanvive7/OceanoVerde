using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MensajesHUD
{
    private static readonly Color VERDE = new Color(0f, 0.345f, 0f);
    private static readonly Color NARANJA = new Color32(248, 184, 0, 255);
    private static readonly Color MARRON = new Color32(80, 48, 0, 255);

    public static void MostrarMensaje(string mensaje)
    {
        Transform canvas = CrearCanvas();
        CrearTexto(canvas, mensaje, 55, Vector2.zero, new Vector2(1500, 120), VERDE);
    }

    public static void MostrarJuegoCompletado()
    {
        Transform canvas = CrearCanvas();
        CrearTexto(canvas, "JUEGO COMPLETADO", 55, new Vector2(0, 80), new Vector2(1500, 120), VERDE);
        CrearBoton(canvas, "MENU PRINCIPAL", new Vector2(0, -60));
    }

    private static Transform CrearCanvas()
    {
        GameObject canvasGO = new GameObject("HUDMensajes");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }

        return canvasGO.transform;
    }

    private static Text CrearTexto(Transform padre, string contenido, int tamano, Vector2 posicion, Vector2 medida, Color color)
    {
        GameObject textoGO = new GameObject("Texto", typeof(RectTransform), typeof(Text), typeof(Outline));
        textoGO.transform.SetParent(padre, false);

        RectTransform rt = textoGO.GetComponent<RectTransform>();
        rt.anchoredPosition = posicion;
        rt.sizeDelta = medida;

        Text texto = textoGO.GetComponent<Text>();
        texto.text = contenido;
        texto.font = Resources.Load<Font>("pixelFont");
        texto.fontSize = tamano;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.horizontalOverflow = HorizontalWrapMode.Wrap;
        texto.resizeTextForBestFit = true;
        texto.resizeTextMinSize = 14;
        texto.resizeTextMaxSize = tamano;
        texto.color = color;
        texto.raycastTarget = false;

        Outline borde = textoGO.GetComponent<Outline>();
        borde.effectColor = Color.white;
        borde.effectDistance = new Vector2(2, -2);

        return texto;
    }

    private static void CrearBoton(Transform padre, string etiqueta, Vector2 posicion)
    {
        GameObject botonGO = new GameObject("BotonMenu", typeof(RectTransform), typeof(Image), typeof(Outline), typeof(Button));
        botonGO.transform.SetParent(padre, false);

        RectTransform rt = botonGO.GetComponent<RectTransform>();
        rt.anchoredPosition = posicion;
        rt.sizeDelta = new Vector2(440, 80);

        botonGO.GetComponent<Image>().color = NARANJA;

        Outline borde = botonGO.GetComponent<Outline>();
        borde.effectColor = MARRON;
        borde.effectDistance = new Vector2(4, -4);

        Text texto = CrearTexto(botonGO.transform, etiqueta, 30, Vector2.zero, new Vector2(400, 70), MARRON);
        texto.GetComponent<Outline>().enabled = false;

        botonGO.GetComponent<Button>().onClick.AddListener(IrAlMenu);
    }

    private static void IrAlMenu()
    {
        if (GameManager.Instancia != null)
            GameManager.Instancia.IrAlMenu();
        else
            SceneManager.LoadScene(0);
    }
}
