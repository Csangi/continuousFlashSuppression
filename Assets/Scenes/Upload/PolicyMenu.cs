using UnityEngine;

public class PolicyMenu : MonoBehaviour
{
    //package plugin from https://github.com/yasirkula/UnitySimpleGDPRConsent

    private string policyKey = "policy";

    void Start()
    {
        var accepted = PlayerPrefs.GetInt(policyKey, 0) == 1;

        if (accepted)
            return;
        else
        {
            SimpleGDPR.ShowDialog(new TermsOfServiceDialog().
                    SetTermsOfServiceLink("https://policies.google.com/terms?hl=en-US").
                    SetPrivacyPolicyLink("https://policies.google.com/privacy?hl=en-US"),
                    onMenuClosed);
        }
    }

    private void onMenuClosed()
    {
        Debug.LogWarning("User Accepted Terms and conditions");
        PlayerPrefs.SetInt(policyKey, 1);
    }
}
