using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Sockets.Database;
using static Sockets.Client.Client;

[System.Serializable]
public class Letters
{
    public char letter;
    public int value;
    public GameObject icon;

}

public class IconCollection : MonoBehaviour
{
    VowelObjectPooler vowelPool;
    
    public string wordToUse;

    public float spacing;
    public int minimumWordLengthToGet;
    public GameObject verticalGroupObject;

    public GameObject consonantsParent;
    public GameObject vowelsParent;


    public List<Letters> consonants = new List<Letters>();

    public Dictionary<char, Queue<GameObject>> poolDictionary;

    public MatchDetails matchDetails;

    char[] chars;

    public bool player2;
    private int id;

    #region Singleton
    public static IconCollection Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    void Start()
    {
        vowelPool = VowelObjectPooler.Instance;
        DataParser.OnFInish += DelayedStart;

        player2 = MenuToMainPersistant.Instance.player2;
        id = MenuToMainPersistant.Instance.id;

    }

    //Method that runs AFTER the DataParser is done parsing. If it wasn't delayed, it would throw a null reference error.
    void DelayedStart()
    {

        if (!player2)
        {
            Debug.Log("player2 is: " + player2);
            wordToUse = DataParser.Instance.GetRandomWordNoVowels(Random.Range(minimumWordLengthToGet, 10));
        }
        else
        {
            matchDetails = Database.Instance.GetMatchDetailsFromDatabase(id);
            Debug.Log("player2 is: " + player2);
            wordToUse = matchDetails.ConsUsed;
        }
        
        InstaConsonants();
        InstaVowelsStart();

        DataParser.OnFInish -= DelayedStart;

    }

    //Method for instantiating the consonants.
    private void InstaConsonants()
    {
        chars = wordToUse.ToCharArray();
        foreach (char chr in chars)
        {
            foreach (Letters cons in consonants)
            {
                if (char.ToUpper(cons.letter) == char.ToUpper(chr))
                {
                    GameObject icon = Instantiate(cons.icon, consonantsParent.transform);
                    TextMeshProUGUI valueTxt = icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    valueTxt.text = cons.value.ToString();
                }
            }
            
        }

    }

    
    //Method that instantiates the vowels at the start.
    void InstaVowelsStart()
    {
        foreach (Letters letter in vowelPool.vowels)
        {
            GameObject icon= vowelPool.SpawnFromPool(letter.letter, vowelsParent.transform);
            TextMeshProUGUI valueTxt = icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            valueTxt.text = letter.value.ToString();
            
        }
    }

    //Method that replaces any of the vowels used. They need to be unlimited.
    public void ReplaceVowel(char letterToReplace)
    {
        int value = 0;
        foreach (Letters vowel in VowelObjectPooler.Instance.vowels)
        {
            if (letterToReplace==vowel.letter)
            {
                value = vowel.value;
            }
        }
        Letters letter = new Letters
        {
            letter = letterToReplace,
            value = value
            
        };
        Debug.Log(letter.value);
        GameObject icon = vowelPool.SpawnFromPool(letter.letter, vowelsParent.transform);
        TextMeshProUGUI valueTxt = icon.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        valueTxt.text = letter.value.ToString();
    }
    
}
