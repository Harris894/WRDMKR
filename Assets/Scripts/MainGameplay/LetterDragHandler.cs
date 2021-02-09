using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LetterDragHandler : MonoBehaviour,/*IDragHandler, IEndDragHandler,*/ IPointerClickHandler
{
    
    public bool inSlot = false;
    GameObject slotsParent;
    public Transform originalParent;
    SlotsManagement slotManager;
    public int originalHierarchyPos;
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        slotManager = SlotsManagement.Instance;
        originalHierarchyPos = transform.GetSiblingIndex();

        originalParent = transform.parent;
        slotsParent = GameObject.Find("Slots");
    }

    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (inSlot)
    //    {
    //        slotManager.rectScroll.enabled = false;
    //        slotManager.MoveSlots(Input.mousePosition);
    //    }
    //    //transform.localScale = new Vector2(1.1f, 1.1f);
    //    //transform.position = Input.mousePosition;
        
    //}

    //public void OnEndDrag(PointerEventData eventData)
    //{
    //    if (inSlot)
    //    {
    //        slotManager.rectScroll.enabled = true;
    //    }
    //}
    
    //On click it lets it know that it needs to move into one of the slots.
    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (inSlot)
        {
            Debug.LogWarning("Use the backspace button to remove!");
            
        }
        else
        {

            id = Random.Range(0, 500);
            StartCoroutine(slotManager.OnFinishMoving());
            slotManager.LetterPlacementCheck(transform.GetSiblingIndex(), this.gameObject, id);
            
            
        }
    }

    //Method that removes letters from the slots back into their previous place or in case of vowels, deactivates them.
    public void Remove(bool vowel)
    {   
        
        if (vowel)
        {
            this.gameObject.SetActive(false);
            transform.SetParent(slotManager.usedLetters);
        }
        else
        {
            StartCoroutine(slotManager.LetterLerp(transform.position, originalParent.transform.position, this.gameObject,originalHierarchyPos, true));
        }
        transform.localScale = new Vector2(1f, 1f);
        inSlot = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Slots")
        {
            inSlot = true;
            transform.localScale = new Vector2(0.8f, 0.8f);
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Slots")
        {
            inSlot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Slots")
        {
            inSlot = false;
        }
    }

}
