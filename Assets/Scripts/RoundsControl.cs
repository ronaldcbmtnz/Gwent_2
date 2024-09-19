using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundsControler : MonoBehaviour
{
    public static int Counter = 0;
    public static int Rounds = 1;
    // Start is called before the first frame update
    public void RoundsControl()
    {
        Counter += 1;
        if(Counter > 1 && Rounds < 3)
        {
            if(GameManager.Instancia.CurrentPlayer) 
            {
                GameManager.Instancia.CurrentPlayer = false;
                int n1 = 10 - GameManager.Instancia.Hand1.transform.childCount;
                int n2 = 10 - GameManager.Instancia.Hand2.transform.childCount;

                if(2 > n1)
                {
                    GameManager.Instancia.Stole(GameManager.Instancia.Deck1.transform,n1,GameManager.Instancia.CurrentPlayer);
                }
                else GameManager.Instancia.Stole(GameManager.Instancia.Deck1.transform,2,GameManager.Instancia.CurrentPlayer);

                if(2 > n2)
                {
                    GameManager.Instancia.Stole(GameManager.Instancia.Deck2.transform,n2,true);
                }
                else GameManager.Instancia.Stole(GameManager.Instancia.Deck2.transform,2,true);
        
                GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);
                Counter = 0;
            }
            else
            {
                GameManager.Instancia.CurrentPlayer = true;
                int n1 = 10 - GameManager.Instancia.Hand1.transform.childCount;
                int n2 = 10 - GameManager.Instancia.Hand2.transform.childCount;
                if(2 > n1)
                {
                    GameManager.Instancia.Stole(GameManager.Instancia.Deck1.transform,n1,false);
                }
                else GameManager.Instancia.Stole(GameManager.Instancia.Deck1.transform,2,false);

                if(2 > n2)
                {
                    GameManager.Instancia.Stole(GameManager.Instancia.Deck2.transform,n2,GameManager.Instancia.CurrentPlayer);
                }
                else GameManager.Instancia.Stole(GameManager.Instancia.Deck2.transform,2,GameManager.Instancia.CurrentPlayer);
   
                GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);
                Counter = 0;
            }
            Rounds += 1;
            CleanBoard.Clean();
            StartRounds.UpdateRounds();
            if(CounterPoints.totalRound_P1 == 2)
            {
                SceneManager.LoadScene(2);
            }
            else if(CounterPoints.totalRound_P2 == 2)
            {
                SceneManager.LoadScene(3);
            }
            return;
        }
        else if(GameManager.Instancia.CurrentPlayer)
        {
            GameManager.Instancia.CurrentPlayer = false;
        }
        else if(!GameManager.Instancia.CurrentPlayer)
        {
            GameManager.Instancia.CurrentPlayer = true;
        }
            //cargar escenas acorde a los ganadores
            if(CounterPoints.totalRound_P1 == CounterPoints.totalRound_P2 && CounterPoints.totalRound_P2 == 2)SceneManager.LoadScene(4);
            else if(CounterPoints.totalRound_P1 == 2)
            {
                SceneManager.LoadScene(2);
            }
            else if(CounterPoints.totalRound_P2 == 2)
            {
                SceneManager.LoadScene(3);
            }
        GameManager.Instancia.StarGame(GameManager.Instancia.CurrentPlayer);
    }

}
   