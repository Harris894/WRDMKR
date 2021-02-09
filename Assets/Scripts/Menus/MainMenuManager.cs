using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour, IPointerClickHandler
{

    public Transform splashScreen;
    public Transform activeGames;
    public Transform pausePanel;

    public bool player2 = false;

   

    public void BackToLobby()
    {
        SceneManager.LoadScene(0);
    }

    public void OnStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToContinueAfterGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (splashScreen.gameObject.activeInHierarchy)
        {
            splashScreen.gameObject.SetActive(false);
            activeGames.gameObject.SetActive(true);
            
        }
    }

    public void TopRightButton()
    {
        if (Time.timeScale==1)
        {
            pausePanel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            pausePanel.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    


}
