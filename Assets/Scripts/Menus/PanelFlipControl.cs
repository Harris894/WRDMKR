using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelFlipControl : MonoBehaviour
{
    public TextMeshProUGUI face;
    public TextMeshProUGUI backFace;

    public Transform settingsPanel;


    public void SwapFaces()
    {
        if (this.gameObject.name!= "Practise")
        {
            face.gameObject.SetActive(false);
            backFace.gameObject.SetActive(true);
        }
        else
        {
            face.gameObject.SetActive(false);
            settingsPanel.gameObject.SetActive(true);
        }
        
    }
}
