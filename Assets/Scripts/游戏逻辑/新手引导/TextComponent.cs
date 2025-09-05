using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextComponent : MonoBehaviour
{
    public Animator animator;

    public TMP_Text dialogText;


    public void Show()
    {
        animator.SetBool("Active", true);
    }

    public void ShowText(string text, bool open = false)
    {
        if (open)
        {
            animator.SetBool("Active", true);
        }
        else
            animator.SetTrigger("Next");
        dialogText.text = text;
    }

    public void Close()
    {
        animator.SetBool("Active", false);
    }
}
