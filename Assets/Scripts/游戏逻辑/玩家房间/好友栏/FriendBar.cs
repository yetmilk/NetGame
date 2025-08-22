using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendBar : MonoBehaviour
{
    public Image onlineImg;
    public TMP_Text idText;

    public void Init(Color color, string id)
    {
        onlineImg.color = color;
        idText.text = id;
    }
}
