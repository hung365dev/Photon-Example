using System.Collections;
using System.Collections.Generic;
using SradnickDev.FlexGUI;
using UnityEngine;

public class Options : FlexScreen
{

    public void ResetLogin()
    {
        var autoLogin = "autoLogin";
		var accName = "accName";
		var accPassword = "accPassword";

		PlayerPrefs.DeleteKey(accName);
		PlayerPrefs.DeleteKey(accPassword);
		PlayerPrefs.DeleteKey(autoLogin);
    }
}
