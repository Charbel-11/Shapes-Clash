using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalRew : MonoBehaviour
{
    public Text RewardExplanation;
    public Text GemsAmount;
    public Button Redeem;
    private int Amount;
    public ValuesChange v;
    private int ConsDays;

    void OnEnable()
    {
        Redeem.interactable = true;
        ConsDays = PlayerPrefs.GetInt("ConsDays");
        RewardExplanation.text = "Thank you and Congratulations on reaching " + ConsDays + " consecutive days in Shapes Clash ! We're extremely happy to see " +
            "that you're enjoying our game ! As a token of our gratitude, here are some diamonds ! Every time you reach a new milestone of 10 more consecutive days, you'll be " +
            "rewarded as well, capping out at 100 ! Your daily rewards won't increase anymore, however this should definitely make up for it ! Thank you for playing Shapes Clash !";
        Amount = PlayerPrefsX.GetIntArray("AdditionalRewardsStats")[(int)(ConsDays / 10) - 2];
        GemsAmount.text = Amount.ToString();
    }

    public void RedeemGems()
    {
        Redeem.interactable = false;
        v.playResourcesAnim(2);
        PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + Amount);        
        int[] r = PlayerPrefsX.GetIntArray("AdditionalRewards");
        r[(int)(ConsDays / 10) - 2] = 0;
        PlayerPrefsX.SetIntArray("AdditionalRewards", r);
        PlayerPrefs.Save();
        ValuesChange.diamonds = PlayerPrefs.GetInt("Diamonds");
        v.topBar.transform.Find("Diamonds").GetComponentInChildren<Text>().text = ValuesChange.diamonds.ToString();
        ClientTCP.PACKAGE_ChestOpening(true);
    }

}
