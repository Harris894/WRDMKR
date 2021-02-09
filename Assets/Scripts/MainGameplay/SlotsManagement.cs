using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using UnityEngine.UI;

public class SlotsManagement : MonoBehaviour
{
    
    public int amountOfSlots;
    public GameObject slotPrefab;
    public Transform slotsParent;
    public Transform usedLetters;

    private HorizontalLayoutGroup layoutGroupH;
    public ScrollRect rectScroll;

    [Tooltip("The higher the value, the fast the letter moves")]
    [Range(0f,1f)]
    public float letterMoveSpeed;

    public List<GameObject> slotsList = new List<GameObject>();

    private int lastIndex = 0;
    int x = 0;


    #region Singleton
    public static SlotsManagement Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion
    

    void Start()
    {
        PlaceSlots();
        layoutGroupH = GameObject.Find("Consonants").GetComponent<HorizontalLayoutGroup>();
    }
    

    //Initial slots
    void PlaceSlots()
    {
        for (int i = 0; i < amountOfSlots; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slotsList.Add(slot);
            TextMeshProUGUI text = slot.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = (i + 1).ToString() + "x";
        }

    }

    void GreyOutLetters(int index,bool state)
    {
        if (state)
        {
            for (int i = index-1; i >= 0; i--)
            {
                Image alphaTransparent = IconCollection.Instance.consonantsParent.transform.GetChild(i).GetComponent<Image>();
                if (alphaTransparent!=null)
                {
                    var tempColor = alphaTransparent.color;
                    tempColor.a = 0.6f;
                    alphaTransparent.color = tempColor;
                }
                
            }

        }
        else
        {
            foreach (Transform child in IconCollection.Instance.consonantsParent.transform)
            {
                Image image = child.GetComponent<Image>();
                if (image != null)
                {
                    var tempColor = image.color;
                    tempColor.a = 1f;
                    image.color = tempColor;
                }
                
            }
        }
        
    }

    public void CreateDummyForPosition(int index, int id)
    {
        GameObject dummy = Instantiate(new GameObject(), IconCollection.Instance.consonantsParent.transform);
        dummy.name = id.ToString();
        dummy.transform.SetSiblingIndex(index);
    }

    public void LetterPlacementCheck(int index, GameObject letterPrefab,int id)
    {
        
        if (!CheckIfVowel(letterPrefab))
        {

            if ((amountOfSlots - x) == AvailableSlots())
            {
                lastIndex = index;
                GreyOutLetters(lastIndex, true);
                PutLetter(letterPrefab);
                CreateDummyForPosition(index, id);
                x = 0;
                Debug.Log("First if");

            }
            else if (index >= lastIndex)
            {
                CreateDummyForPosition(index, id);
                PutLetter(letterPrefab);
                x = 0;
                Debug.Log("2ND if");
            }
            else if (index < lastIndex)
            {
                Debug.Log("You are trying to use a letter that is placed before the initial letter you chose");
            }
        }
        else
        {
            if (amountOfSlots - x==AvailableSlots())
            {
                x++;
                
            }
            PutLetter(letterPrefab);
        }
        //Debug.Log(x);
        Debug.Log("X is: " + x + "There are available slots: " + AvailableSlots() + "LastIndex is " + lastIndex);

    }

    //Method that checks which slot is available and starts the procedure of putting a letter in a slot.
    //Chceks if the clicked letter was a vowel, then it replaces it.
    public void PutLetter(GameObject letterPrefab)
    {

        foreach (Transform slot in slotsParent)
        {
            if (slot.transform.childCount <= 1)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/placeLetter");
                letterPrefab.transform.SetParent(slot.transform);
                letterPrefab.transform.position = slot.transform.position;
                rectScroll.horizontalNormalizedPosition = 1;
                
                
                break;
            }
        }

        if (CheckIfVowel(letterPrefab))
        {
            IconCollection.Instance.ReplaceVowel(letterPrefab.name[0]);
        }
        
    }

    //Function for the backspace
    public void RemoveLetter()
    {
        if (amountOfSlots!=AvailableSlots())
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/backspace");
            if (LetterToDisable() != null)
            {
                GameObject letter = LetterToDisable();
                LetterDragHandler handler = letter.GetComponent<LetterDragHandler>();
                if (CheckIfVowel(letter))
                {

                    handler.Remove(true);
                }
                else
                {
                    handler.Remove(false);

                }

                if (slotsParent.childCount > amountOfSlots)
                {
                    GameObject lastItem = slotsList.Last().gameObject;
                    Destroy(lastItem);
                    slotsList.Remove(lastItem);

                }
            }
        }

        if (AvailableSlots()==amountOfSlots-1)
        {
            GreyOutLetters(0, false);
            x = 0;
        }

        
        Debug.Log("X: " + x);
        Debug.Log("LastIndex"+lastIndex);
    }

    //Function for the clear all button. clears letters and sets the amount of slots to the initial one if there were extra.
    public void ClearAll()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/clearAll");

        for (int i = 0; i < slotsParent.childCount; i++)
        {
            if (slotsParent.GetChild(i).childCount>1)
            {
                GameObject letter = slotsParent.GetChild(i).transform.GetChild(1).gameObject;
                LetterDragHandler handler = letter.GetComponent<LetterDragHandler>();

                if (CheckIfVowel(letter))
                {
                    handler.Remove(true);

                }
                else
                {
                    handler.Remove(false);
                }

            }
            
        }
        if (slotsParent.childCount>amountOfSlots)
        {
            for (int i = slotsParent.childCount; i-- > amountOfSlots;)
            {
                GameObject slot = slotsList.Last().gameObject;
                Destroy(slot);
                slotsList.Remove(slot);
                
            }

        }

        GreyOutLetters(0, false);
        x = 0;
    }

    //Method that returns the amount of slots that DONT have a letter in them.
    public int AvailableSlots()
    {
        int availableSlots = 0;
        foreach(Transform slot in slotsParent)
        {
            if (slot.transform.childCount<=1)
            {
                availableSlots++;
            }
        }
        return availableSlots;
    }

    //Method that returns true or false depending on if a letter is a vowel or not
    bool CheckIfVowel(GameObject letterToCheck)
    {
        if (Regex.IsMatch(letterToCheck.name[0].ToString(), "[aeiouyAEIOUY]", RegexOptions.IgnoreCase))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Returns the last gameobject/letter in the slots
    GameObject LetterToDisable()
    {
        for (int i = slotsParent.childCount; i-- > 0;)
        {
            if (slotsParent.GetChild(i).childCount > 1)
            {
                GameObject duplicate = slotsParent.GetChild(i).transform.GetChild(1).gameObject;
                return duplicate;
            }
        }
        
        Debug.LogWarning("No letter in slot");
        return null;
    }

    //Updates the score amount 
    public int ScoreAmount()
    {
        int score = 0;
        int multiplier = 0;
        for (int i = 0; i < slotsParent.childCount-AvailableSlots(); i++)
        {
            if (slotsParent.GetChild(i).childCount>1)
            {
                multiplier++;
                string text = slotsParent.GetChild(i).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text;
                Int32.TryParse(text[0].ToString(), out int value);
                score += value;
            }
            
        }
        Debug.Log("Score "+(score * multiplier)+"-Multiplier: "+multiplier);
        return score*multiplier;
    }

    //Method used to animate the letters moving from stacks to slots. //PRONE TO CHANGE!!
    public IEnumerator LetterLerp(Vector2 startPos, Vector2 endPos, GameObject letter,int siblingIndex, bool consonant)
    {
        float timer = letterMoveSpeed;

        while (timer <= 1f)
        {

            timer += Time.deltaTime;
            float vals = Mathf.Lerp(0, 1, timer);
            letter.transform.localScale = new Vector2(vals, vals);
            yield return new WaitForEndOfFrame();

            
        }

        if (consonant)
        {

            LetterDragHandler handler = letter.GetComponent<LetterDragHandler>();
            GameObject dummy = GameObject.Find(handler.id.ToString());
            Debug.Log(dummy.name);
            letter.transform.SetParent(handler.originalParent);
            //layoutGroupH.spacing += 0.0001f;
            letter.transform.SetSiblingIndex(dummy.transform.GetSiblingIndex());
            //letter.transform.localScale = new Vector2(1, 1);
            Destroy(dummy);
            //Debug.Log(siblingIndex);
        }

    }

    //Method that runs after every placement of a letter. Checks how many available slots are left, if there are less than 4, spawns more
    public IEnumerator OnFinishMoving()
    {
        
        if (AvailableSlots() < 4)
        {
            yield return new WaitForEndOfFrame();
            GameObject slot = Instantiate(slotPrefab, slotsParent);
            slotsList.Add(slot);
            TextMeshProUGUI text = slot.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            text.text = (slotsParent.childCount).ToString() + "x";
            
        }
    }
    
}
