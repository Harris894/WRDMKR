using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuToMainPersistant : MonoBehaviour
{

    #region Singleton
    public static MenuToMainPersistant Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }
    #endregion
    public TMP_InputField input;

    public bool player2;
    public int id; 


    public void ChangePlayer()
    {
        if (player2)
        {
            player2 = false;
        }
        else
        {
            player2 = true;
        }
    }

    public void ChangeID()
    {
        if (input.isActiveAndEnabled)
        {
            string idInput = input.text;
            try
            {
                id = Int32.Parse(idInput);
                Debug.Log(id);
            }
            catch (System.Exception e)
            {
                Debug.Log("Write a valid ID and try again");
                Debug.Log(e.Message);
                throw;
            }
        }
        
    }
}
