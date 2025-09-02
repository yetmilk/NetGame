using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FettersManager : MonoBehaviour
{
    public CharacterController target;

    public List<RareBook> curBooks;

    private void Start()
    {
        PlayerManager.Instance.onPlayerCreate += SetPlayer;
    }

    public void SetPlayer()
    {
        target = PlayerManager.Instance.GetPlayerInfoByName(PlayerManager.Instance.selfId).playerObj;
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

    public void AddRareBook(RareBookName rareBookName)
    {
        RareBook rareBook = LoadManager.Instance.GetResourceByName<RareBook>(rareBookName.ToString());
        curBooks.Add(rareBook);
    }
}

