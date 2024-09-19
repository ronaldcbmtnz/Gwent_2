using Unity.Collections;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private GameObject draggedObject; // Almacenar el objeto que se esta arrastrando
    private RectTransform rectTransform; // Referencia al componente RectTransform para manipular la posición y tamaño del objeto en la UI
    private CanvasGroup canvasGroup; // Referencia al componente CanvasGroup para controlar la visibilidad y la interacción del objeto
    private Vector3 startPosition; // Almacenar la posicion inicial del objeto para poder evertir el arrastre si es necesario 
    private string TipoDeCarta;
    // Se ejecuta al iniciar el objeto, antes del primer frame
    private void Awake()
    {
        // Obtiene el componente RectTransform del objeto para manipular su posición y tamaño
        rectTransform = GetComponent<RectTransform>();
        // Obtiene el componente CanvasGroup para controlar la visibilidad y la interacción
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Se ejecuta en cada frame
    private void Update()
    {
        // Inicializa la búsqueda desde el objeto actual
        Transform currentTransform = transform;

        // Busca hacia arriba en la jerarquía de objetos hasta encontrar uno con el tag "PlayerHand"
        while (currentTransform != null)
        {
            // Si encuentra un objeto con el tag "PlayerHand", habilita el componente DragDrop
            if (currentTransform.tag == "PlayerHand")
            {
                enabled = true;
                return; // Termina la búsqueda
            }
            // Sube un nivel en la jerarquía para buscar en el objeto padre
            currentTransform = currentTransform.parent;
        }
        // Si no se encuentra, deshabilita el componente DragDrop
        enabled = false;
        
    }
    private static VisualCard currentDraggingCard = null;
    public static VisualCard GetOnThisMoment()
    {
        return currentDraggingCard;
    }
    public static bool IsDragDrop = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragDrop = true;

        if (!enabled) return;

        currentDraggingCard = eventData.pointerDrag.GetComponent<VisualCard>();

        // Evita el arrastre si el componente está desactivado
        
        // Asigna el objeto que está siendo arrastrado
        draggedObject = eventData.pointerDrag; 

        // Guarda la posicion inicial del objeto al comenzar el arrastre 
        startPosition = rectTransform.anchoredPosition;

        // Reduce la opacidad para indicar que el objeto está siendo arrastrado
        canvasGroup.alpha = .6f;

        // Permite que otros objetos interactúen con el objeto mientras se arrastra
        canvasGroup.blocksRaycasts = false;
    }

    // Se llama durante el arrastre
    public void OnDrag(PointerEventData eventData)
    {
        // Evita el arrastre si el componente está desactivado
        if (!enabled) return;

        // Actualiza la posición del objeto basado en el movimiento del ratón
        rectTransform.anchoredPosition += eventData.delta;
    }

    // Se llama al finalizar el arrastre
    
    public void OnEndDrag(PointerEventData eventData)
    {
        // Evita el arrastre si el componente está desactivado
        if (!enabled) return;
        // Restaura la opacidad original
        canvasGroup.alpha = 1f;
        // Bloquea las interacciones con otros objetos
        canvasGroup.blocksRaycasts = true;

        if(CustomCollider.IsCalling)
        {
            draggedObject.transform.SetParent(CustomCollider.otherCard.transform.parent);
            GameManager.playedcard = 1;
            RoundsControler.Counter = 0;
            if (!GameManager.Instancia.CurrentPlayer) GameManager.Instancia.CurrentPlayer = true;
            else GameManager.Instancia.CurrentPlayer = false;

            //activar efecto
            GameManager.Instancia.ActualiceContext();
            draggedObject.GetComponent<VisualCard>().card.ActivateEffect(draggedObject);
            GameManager.Instancia.ActualiceVisual();
            GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);
        }

        //Verificar si el objeto fue soltado en una zona valida
        else if (!IsDroppedInValidZone(eventData))
        {
            rectTransform.anchoredPosition = startPosition;
        }
        else 
        {
            ValidZone FinalPosition = eventData.pointerCurrentRaycast.gameObject.GetComponent<ValidZone>();
            if(FinalPosition is not null)
            {
                FinalPosition.PlaceObject(rectTransform);
                GameManager.playedcard = 1;
                RoundsControler.Counter = 0;
                if (!GameManager.Instancia.CurrentPlayer) GameManager.Instancia.CurrentPlayer = true;
                else GameManager.Instancia.CurrentPlayer = false;

                //activar efecto
                GameManager.Instancia.ActualiceContext();
                draggedObject.GetComponent<VisualCard>().card.ActivateEffect(draggedObject);
                GameManager.Instancia.ActualiceVisual();
                GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);

            }
            
        }
        IsDragDrop = false;
    }
    // Método para verificar si el objeto fue soltado en una zona válida
    private bool IsDroppedInValidZone(PointerEventData eventData)
    {
        // Obtiene el objeto sobre el cual se soltó el objeto arrastrado
        GameObject droppedOnObject = eventData.pointerCurrentRaycast.gameObject;
        // Intenta obtener el componente ValidZone del objeto sobre el cual se soltó el objeto arrastrado
        ValidZone validZone = droppedOnObject.GetComponent<ValidZone>();
        
        TipoDeCarta = draggedObject.GetComponent<VisualCard>().card.Type.ToString();

        if((TipoDeCarta == "oro" || TipoDeCarta == "plata") && validZone is not null && GameManager.Instancia.CurrentPlayer == validZone.OyeSiii)
        {
            Range [] ranges = draggedObject.GetComponent<VisualCard>().card.Range;
            foreach (var item in ranges)
            {
                if(item.ToString() == validZone.ZoneType)
                {
                    CustomCollider customCollider = draggedObject.GetComponent<CustomCollider>();
                    customCollider._enabled = false;
                    return true;
                }
            }
            return false;
        }
        else if(TipoDeCarta == "clima" && validZone is not null)
        {
            if(validZone.ZoneType == "Clima") 
            {
                CustomCollider customCollider = draggedObject.GetComponent<CustomCollider>();
                customCollider._enabled = false;
                return true;
            }
            else return false;
        }
        else if(TipoDeCarta == "aumento" && validZone is not null && GameManager.Instancia.CurrentPlayer == validZone.OyeSiii)
        {
            if(validZone.ZoneType == "M" || validZone.ZoneType == "R" || validZone.ZoneType == "S")
            {
                return true;
            }
            else return false;
        }
        else if(TipoDeCarta == "despeje" || TipoDeCarta == "senuelo")
        {
            if(CustomCollider.IsCalling) return true;
            else return false;
        }
        return false;
    }
}