using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Se añade a cada botón de la UI (Izquierda, Derecha, Saltar, Abajo, Arriba).
/// Detecta cuándo el dedo pulsa y cuándo suelta el botón.
/// </summary>
public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Accion { Izquierda, Derecha, Saltar, Abajo, Arriba }

    public Accion accion;
    public PlayerController jugador;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (accion)
        {
            case Accion.Izquierda: jugador.SetMobileInput(-1f);    break;
            case Accion.Derecha:   jugador.SetMobileInput(1f);     break;
            case Accion.Arriba:    jugador.SetMobileVertical(1f);  break;
            case Accion.Abajo:     jugador.SetMobileVertical(-1f); break;
            case Accion.Saltar:    jugador.Jump();                 break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (accion)
        {
            case Accion.Izquierda:
            case Accion.Derecha:   jugador.SetMobileInput(0f);    break;
            case Accion.Arriba:
            case Accion.Abajo:     jugador.SetMobileVertical(0f); break;
        }
    }
}