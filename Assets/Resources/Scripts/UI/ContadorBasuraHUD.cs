using UnityEngine;
using UnityEngine.UI;

public class ContadorBasuraHUD : MonoBehaviour
{
    private Text texto;
    private int ultimoValor = -1;

    public static void Crear()
    {
        Transform canvas = MensajesHUD.CrearCanvas();
        Text texto = MensajesHUD.CrearTexto(canvas, "", 35, Vector2.zero, new Vector2(700, 60), MensajesHUD.VERDE);
        texto.alignment = TextAnchor.UpperRight;

        RectTransform rt = texto.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-30, -30);

        ContadorBasuraHUD contador = canvas.gameObject.AddComponent<ContadorBasuraHUD>();
        contador.texto = texto;
    }

    void Update()
    {
        if (GameManager.Instancia == null) return;

        int recogidas = GameManager.Instancia.TotalRecogidas;
        if (recogidas == ultimoValor) return;

        ultimoValor = recogidas;
        texto.text = "BASURA RECOGIDA: " + recogidas;
    }
}
