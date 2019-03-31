using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SradnickDev.FlexGUI;

public class MainMenu : FlexScreen
{
    public override void Close()
    {
        base.Close();
        Debug.Log("Close");
        gameObject.SetActive(false);
    }

    public override void Open()
    {
        base.Open();
        Debug.Log("Open");
    }
}
