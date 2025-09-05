using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FettersManager : MonoBehaviour
{
    public CharacterController target;

    public List<RareBook> curBooks;

    public Transform rarebookPar;

    public GameObject uiPanel;

    private void Start()
    {
        PlayerInputManager.Instance.action.UI.OpenFetter.started += OpenFetter_started;
    }

    private void OpenFetter_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        uiPanel.SetActive(!uiPanel.activeSelf);
    }

    public void SetPlayer(CharacterController character)
    {

        target = character;
        if (target != null)
            SetRareBookToTarget();
    }

    public void SetRareBookToTarget()
    {
        foreach (RareBook book in curBooks)
        {
            foreach (var buff in book.buff)
            {
                AddBuffInfo addBuffInfo = new AddBuffInfo(buff.ToString(), target, target);

                target.curCharaData.buffController.AddBuff(addBuffInfo);

            }
        }
    }

    public void ResetFetter()
    {
        curBooks.Clear();
        for (int i = 0; i < rarebookPar.childCount; i++)
        {
            Destroy(rarebookPar.GetChild(i).gameObject);
        }
    }

    public void AddRareBook(RareBookName rareBookName)
    {
        RareBook rareBook = LoadManager.Instance.GetResourceByName<RareBook>(rareBookName.ToString());

        var go = LoadManager.Instance.GetResourceByName<GameObject>("秘籍信息条");

        var obj = Instantiate(go, rarebookPar);

        obj.GetComponent<RarebookUIObj>().Init(rareBook.bookName.ToString(), rareBook.description);
        curBooks.Add(rareBook);
        SetRareBookToTarget();
    }

    public void AddRareBook(RareBook rareBook)
    {
        var go = LoadManager.Instance.GetResourceByName<GameObject>("秘籍信息条");

        var obj = Instantiate(go, rarebookPar);

        obj.GetComponent<RarebookUIObj>().Init(rareBook.bookName.ToString(), rareBook.description);
        curBooks.Add(rareBook);
        SetRareBookToTarget();
    }
}

