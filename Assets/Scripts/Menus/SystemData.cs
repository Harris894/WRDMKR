using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatchDetail", menuName = "ScriptableObjects/MatchDetails", order = 1)]
public class MatchDetail : ScriptableObject
{
    public string playerName;
    public int id;
    public string consUsed;
    public int score;
    public int amountWordsFound;
    public bool accepted;
    public string wordsUsed;
    public int amountOfInvites;

}
