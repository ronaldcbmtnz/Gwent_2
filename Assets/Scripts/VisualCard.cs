using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualCard : MonoBehaviour
{
    public Card card;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Power;
    public Image CardPhoto;
    public void Start ()
    {
        if(card is not null ) InicializaCarta();
    }
    public void InicializaCarta()
    {
        Name.text = card.Name;
        Power.text = card.Power.ToString();
        CardPhoto.sprite = card.CardPhoto;
    }
    public void Update()
    {
      if(Power!=null && card !=null)  Power.text = card.Power.ToString();
    }
    
}
