using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorOn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public bool isOver = false;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;
        anim.SetBool("isOver", isOver);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;
        anim.SetBool("isOver", isOver);
    }
}
