using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

[RequireComponent(typeof(Button))]
public class AdsButton : MonoBehaviour, IUnityAdsListener
{

#if UNITY_IOS
    private string gameId = "3547392";
#elif UNITY_ANDROID
    private string gameId = "3547393";
#else
    private string gameId = "";
#endif

    Button myButton;
    public string myPlacementId = "rewardedVideo";
    bool testmode = false;

    private ValuesChange MenuManager;

    void Start()
    {
        myButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = Advertisement.IsReady(myPlacementId);


        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testmode);

        if (name == "Button2") { Advertisement.RemoveListener(this); }  //To avoid looping over all of them each time

        MenuManager = GameObject.Find("Menu Manager").GetComponent<ValuesChange>();
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        Advertisement.Show(myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId)
        {
            myButton.interactable = true;
        }
    }

    //Loops over all listeners!!!!!
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            if (MenuManager.usedP1) { MenuManager.watchedFirstAd(); }
            else { MenuManager.watchedSecondAd(); }
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
            MenuManager.ShowMessage("Rewards weren't given because you skipped the ad");
        }
        else if (showResult == ShowResult.Failed)
        {
            MenuManager.ShowMessage("The ad did not finish due to an error");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    public void OnDestroy()
    {
        print("Dead call 2");
        if (name != "Button2") { Advertisement.RemoveListener(this); }
    }
}
