using TMPro;
using UnityEngine;

public class ChatText : MonoBehaviour
{
    public enum TextType
    {
        ChatTitle, ChatContent, SystemNotice,
    }
    public const string TEXT_YELLOWCOLOR = "<color=#73CFF5>";
    public const string TEXT_BLUECOLOR = "<color=#73CFF5>";
    public const string TEXT_COLOREND = "</color>";

    public TMP_Text text;

    public void SetText(TextType type, string text, string snedId = "")
    {
        switch (type)
        {
            case TextType.ChatTitle:
                this.text.text = text;

                break;
            case TextType.ChatContent:

                this.text.text = TEXT_BLUECOLOR + snedId + TEXT_COLOREND + ":";
                this.text.text += text;
                break;
            case TextType.SystemNotice:
                this.text.text = TEXT_YELLOWCOLOR + text + TEXT_COLOREND;
                break;
            default:
                break;
        }
    }
}
