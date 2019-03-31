using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class CreateAccount : Photon.PunBehaviour
{


    [SerializeField] Text m_username;
    [SerializeField] InputField m_password;
    [SerializeField] Text m_email;
    [SerializeField] Text m_accountInfo;
    const string m_registerAdress = "https://photonexample.000webhostapp.com/register.php";
    string m_fetchedText;



    public void OnCreateAccount()
    {
        StartCoroutine(TryCreateAccount(m_username.text, m_password.text, m_email.text));
    }

    IEnumerator TryCreateAccount(string username, string password, string email)
    {
        WWWForm form = new WWWForm();
        form.AddField("usernamePost", username);
        form.AddField("passwordPost", password);
        form.AddField("emailPost", email);

        WWW m_www = new WWW(m_registerAdress, form);

        yield return m_www;

        m_fetchedText = m_www.text;
        Debug.Log(m_fetchedText);

        if (m_fetchedText == "True")
        {
            m_accountInfo.color = Color.green;
            m_accountInfo.text = "Account created.";

        }
        else
        {
            m_accountInfo.color = Color.red;
            m_accountInfo.text = "Account creation Failed!";
        }

    }

}
