using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

[System.Serializable]
public class Word
{
    public int wordLength;
    public string word;

    public Word(int newLength, string newWord)
    {
        wordLength = newLength;
        word = newWord;
    }

}

public class DataParser : MonoBehaviour
{
    public delegate void DoneParsing();
    public static event DoneParsing OnFInish;

    private readonly List<Word> wordsList = new List<Word>();

    public Dictionary<int, List<Word>> wordsDictionary = new Dictionary<int, List<Word>>();

    

    #region Singleton
    public static DataParser Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion
    
    //Loads the text file with all the words.
    //Creates and populates an array out of those words
    //Loops through the array and removes empty spaces after each word and sorts them into a dictionary according to their lenght.
    //When it's done, it lets the IconCollection script know about it.
    void Start()
    {
        TextAsset wordData = Resources.Load<TextAsset>("words");
        string[] data = wordData.text.Split(new char[] { '\n' });
        foreach (var entryString in data)
        {
            string cleaned = Regex.Replace(entryString, @"\s", "");
            Word entry = new Word(cleaned.Length, cleaned);
            PutIntoDictionary(entry);
        }
        OnFInish?.Invoke();
    }
    
    //Method into which a string is passed to check if that string exists in the dictionary of words.
    public bool CheckIfStringExists(string str)
    {
        string cleaned = Regex.Replace(str, @"\s", "");
        if (wordsDictionary.ContainsKey(cleaned.Length))
        {
            for (int i = 0; i < wordsDictionary[cleaned.Length].Count; i++)
            {
                if (string.Equals(wordsDictionary[cleaned.Length][i].word, cleaned, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //Sorts words into an array depending on the length of the word.
    void PutIntoDictionary(Word entry)
    {
        if (wordsDictionary.ContainsKey(entry.wordLength))
        {
            wordsDictionary[entry.wordLength].Add(entry);
        }
        else
        {
            wordsDictionary.Add(entry.wordLength, new List<Word>());
            wordsDictionary[entry.wordLength].Add(entry);
        }

    }

    //Picks a random word from the dictionary and removes the vowels from it. The string that remains, is then used to populate the consonants in the game.
    public string GetRandomWordNoVowels(int randomNum)
    {
        bool wordFound = false;
        string toReturn = "";
        int randomNumToUse = randomNum;

        while (!wordFound)
        {
            if (wordsDictionary.ContainsKey(randomNumToUse))
            {
                if (toReturn.Length<12)
                {
                    int rndNumForDictionary = Random.Range(0, wordsDictionary[randomNumToUse].Count);
                    string rndmWord = wordsDictionary[randomNumToUse][rndNumForDictionary].word;
                    toReturn += Regex.Replace(rndmWord, "[aeiouy]", "", RegexOptions.IgnoreCase);
                    randomNumToUse = 12- toReturn.Length;
                    if (randomNumToUse==1)
                    {
                        toReturn = "";
                        randomNumToUse = randomNum;
                    }
                    Debug.Log(toReturn);

                }
                else if (toReturn.Length>12)
                {
                    toReturn = "";
                    randomNumToUse = randomNum;
                }
                else 
                {
                    wordFound = true;
                    return toReturn;
                }
            }
            
        }
        return null;
    }



}

