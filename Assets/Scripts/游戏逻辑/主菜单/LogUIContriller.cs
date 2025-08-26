using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogUIContriller : MonoBehaviour
{

    [Header("��¼ҳ������")]
    public TMP_InputField userName_login_InputField;
    public TMP_InputField password_login_InputField;
    public Button login_loginBtn;

    //ע��ҳ������
    public TMP_InputField userName_logon_InputField;
    public TMP_InputField password_logon_InputField;
    public Button logon_logonBtn;

    //ת��������
    public Animator blackAnim;


    public void PlayAnimation(string animName)
    {
        blackAnim.CrossFade(animName, 0);
    }

    public void Login()
    {
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = userName_login_InputField.text;
        msgLogin.pw = password_login_InputField.text;
        TipManager.Instance.ShowTip(TipType.LogTip, "��½��");
        LogInManager.SendMsg(msgLogin);
    }

    public void LogOn()
    {
        MsgRegister msgRegister = new MsgRegister();
        msgRegister.id = userName_logon_InputField.text;
        msgRegister.pw = password_logon_InputField.text;
        TipManager.Instance.ShowTip(TipType.LogTip, "ע����");
        LogInManager.SendMsg(msgRegister);
    }
}
