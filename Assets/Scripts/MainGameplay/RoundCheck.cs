using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class RoundCheck : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public Transform timesUpPanel;
    public TextMeshProUGUI panelScoreText;
    public Image bigX;

    public TextMeshProUGUI wordUsedTXT;
    public Animator wordUsedAnim;

    SlotsManagement slotsManager;
    [HideInInspector]
    public List<string> usedWords = new List<string>();
    [HideInInspector]
    public List<string> wrongWordsUsed = new List<string>();
    public int score = 0;
    public int amountWordsFound = 0;
    public float timeInMinutes;
    private float time;

    FMOD.Studio.EventInstance s_wrongAnswer;
    FMOD.Studio.EventInstance s_wordUsed;

    #region Singleton
    public static RoundCheck Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        time = timeInMinutes * 60;
        slotsManager = SlotsManagement.Instance;
        s_wrongAnswer = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/wrongAnswer");
        s_wordUsed = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/wordUsedNotification");
    }
    
    //Method that checks if the word that the player gave exists. If it exists, the word is added to a list of already UsedWords so that they don't use the same again
    //Updates the score depending on the value and length of the word. And lastly, removes everything from the slots. 
    public void CheckWord()
    {
        string word = WordGiven();

        if (usedWords.Contains(word))
        {
            if (wordUsedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !wordUsedAnim.IsInTransition(0))
            {
                StartCoroutine(WordAlreadyUsed(word));
            }
            
            Debug.Log("You have already used the word: " + word);
        }
        else
        {
            if (DataParser.Instance.CheckIfStringExists(word))
            {
                Debug.Log("Word: " + word + " exists");
                amountWordsFound++;
                usedWords.Add(word);
                score = score + slotsManager.ScoreAmount();
                UpdateScoreboard();
                slotsManager.ClearAll();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/correctAnswer");
            }
            else
            {
                Debug.Log("Word: " + word + " DOESNT exist and has been added to the list.");
                if (!wrongWordsUsed.Contains(word))
                {
                    wrongWordsUsed.Add(word);
                }
                else
                {
                    Debug.Log("Word: " + word + " already exists in the incorrect used words.");
                }
                StartCoroutine(ShowX());
                //Sound and pop up for wrong answer
            }
        }
    }
    
    //Takes all the chharacters/letters the player has put into the slots and converts them from a batch of chars into a single string.
    string WordGiven()
    {
        StringBuilder instertedWord = new StringBuilder();
        char letter;
        for (int i = 0; i < slotsManager.slotsParent.childCount - slotsManager.AvailableSlots(); i++)
        {
            letter = slotsManager.slotsParent.GetChild(i).GetChild(1).gameObject.name[0];
            instertedWord.Append((char)letter);
        }
        string word = instertedWord.ToString();
        return word;
    }

    //Updates the score TXT
    void UpdateScoreboard()
    {
        scoreText.text = score.ToString();
    }

    private void Update()
    {
        if (timesUpPanel!=null)
        {
            if (!timesUpPanel.gameObject.activeInHierarchy)
            {
                if (time > 0.1f)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    TimeUp();
                }
            }
            var minutes = Mathf.Floor(time / 60);
            var seconds = time % 60;
            if (seconds > 59) seconds = 59;
            timerText.text = string.Format("{0:00} : {1:00} ", minutes, seconds);
        }
        
        
        

        
    }

    void TimeUp()
    {
        timesUpPanel.gameObject.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/timeOut");
        panelScoreText.text = "Score: " + score.ToString()+ "\n Words found: " + amountWordsFound;
    }

    IEnumerator ShowX()
    {
        bigX.gameObject.SetActive(true);
        s_wrongAnswer.start();
        yield return new WaitForSeconds(0.85f);
        bigX.gameObject.SetActive(false);
    }

    IEnumerator WordAlreadyUsed(string word)
    {
        s_wordUsed.start();
        wordUsedTXT.text = word.ToUpper() + " is gebruikt";
        wordUsedAnim.SetTrigger("alreadyUsed");
        yield return new WaitForSeconds(2f);
        wordUsedAnim.SetTrigger("retract");
        
        
    }
}
