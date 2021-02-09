using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sockets.Client;
using System.Text.RegularExpressions;

public class EndScreenManagement : MonoBehaviour
{
    RoundCheck roundCheck;

    public TextMeshProUGUI nameTXT;
    public TextMeshProUGUI scoreTXT;
    public TextMeshProUGUI amountOfWordsFound;
    public TextMeshProUGUI wordsFound;

    public TextMeshProUGUI opponentenemyNameTXT;
    public TextMeshProUGUI opponentScoreTXT;
    public TextMeshProUGUI opponentAmountOfWordsFound;
    public TextMeshProUGUI opponentWordsFound;

    private Client.MatchDetails matchDetails1;

    private bool player2 = IconCollection.Instance.player2;

    private void Awake()
    {
        roundCheck = RoundCheck.Instance;
        if (player2)
        {
            matchDetails1 = IconCollection.Instance.matchDetails;
        }
        
    }
    void Start()
    {
        amountOfWordsFound.text = roundCheck.amountWordsFound.ToString() + " Words";
        scoreTXT.text = roundCheck.score.ToString();

        if (matchDetails1!=null)
        {
            opponentAmountOfWordsFound.text = matchDetails1.AmountWordsFound.ToString() + " Words";
            opponentScoreTXT.text = matchDetails1.Score.ToString();
        }

        PopulateWordsLists();
        SendData();

    }

    void PopulateWordsLists()
    {
        string words="";
        string noWords="";
        string wordsOpponent="";
        for (int i = 0; i < roundCheck.usedWords.Count; i++)
        {
            words+= (1+i).ToString() + " " + roundCheck.usedWords[i] + "\n";
            
        }

        for (int i = 0; i < roundCheck.wrongWordsUsed.Count; i++)
        {
            noWords+= (1+i).ToString() + " " + roundCheck.wrongWordsUsed[i] + "\n";
        }

        if (matchDetails1!=null)
        {
          wordsOpponent = Regex.Replace(matchDetails1.usedWords, "[,]", "\n");
          opponentWordsFound.text = wordsOpponent;  
        }

        wordsFound.text = words;


    }
     
    private string ListToCommaString(List<string> usedWords)
    {
        string commaString = string.Join(",", usedWords);

        return commaString;
    }

    public void SendData()
    {
        if (!player2)
        {
            Client.Instance.InitiateConnection(IconCollection.Instance.wordToUse, roundCheck.score, roundCheck.amountWordsFound, true, ListToCommaString(roundCheck.usedWords));
            Debug.Log("sent data");
        }
        
    }


}
