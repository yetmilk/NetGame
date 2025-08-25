using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogUIContriller : MonoBehaviour
{

    [Header("登录页面引用")]
    public TMP_InputField userName_login_InputField;
    public TMP_InputField password_login_InputField;
    public Button login_loginBtn;

    //注册页面引用
    public TMP_InputField userName_logon_InputField;
    public TMP_InputField password_logon_InputField;
    public Button logon_logonBtn;

    //转场动画机
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
        TipManager.Instance.ShowTip(TipType.LogTip, "登陆中");
        LogInManager.SendMsg(msgLogin);
    }

    public void LogOn()
    {
        MsgRegister msgRegister = new MsgRegister();
        msgRegister.id = userName_logon_InputField.text;
        msgRegister.pw = password_logon_InputField.text;
        TipManager.Instance.ShowTip(TipType.LogTip, "注册中");
        LogInManager.SendMsg(msgRegister);
    }
}
