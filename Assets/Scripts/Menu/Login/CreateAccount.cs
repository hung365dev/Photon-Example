using System;
using System.Collections;
using SradnickDev.FlexGUI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class CreateAccount : FlexScreen
{

    const string URLREGISTER = "https://sradnickdev.de/photon/account/register.php";

    [SerializeField] private InputField m_username;
    [SerializeField] private InputField m_password;
    [SerializeField] private InputField m_passwordRepeat;
    [SerializeField] private InputField m_email;
    
    [SerializeField] private Button m_createAccount;


    [Header("Info")] 
    [SerializeField] private Text m_logInfo;
    [SerializeField] private Color m_defaultInfo = Color.white;
    [SerializeField] private Color m_failedInfo = Color.red;
    [SerializeField] private Color m_successInfo = Color.green;

    public void OnCreateAccount()
    {
        if (m_username.text == string.Empty || m_password.text == string.Empty ||
            m_passwordRepeat.text == string.Empty || m_email.text == string.Empty)
        {
            LogInfo("Please fill out all fields!",m_failedInfo);
        }
        else if (!m_password.text.Equals(m_passwordRepeat.text))
        {
            LogInfo("Passwords do not match!",m_failedInfo);
        }
        else
        {
            StartCoroutine(TryCreateAccount(m_username.text, m_password.text, m_email.text));
            if (m_createAccount)
            {
                m_createAccount.interactable = false;
            }
        }
    }

    IEnumerator TryCreateAccount(string username, string password, string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);
        form.AddField("emailPost", email);

        LogInfo("please wait a moment..",m_defaultInfo);
        using (UnityWebRequest www = UnityWebRequest.Post(URLREGISTER, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                LogInfo(www.error, m_failedInfo);
            }
            else
            {
                var body = www.downloadHandler.text;
                Debug.Log(body);

                if (body == "01")
                {
                    Close();
                    LogInfo("Account created.", m_successInfo);
                }
                else if(body == "00")
                {
                    LogInfo("Account already Exists!", m_failedInfo);

                    if (m_createAccount)
                    {
                        m_createAccount.interactable = true;
                    }
                }
                else
                {
                    LogInfo(body, m_failedInfo);

                    if (m_createAccount)
                    {
                        m_createAccount.interactable = true;
                    }
                }
            }
        }
    }

    private void LogInfo(string txt, Color color)
    {
        m_logInfo.color = color;
        m_logInfo.text = txt;
    }
}
