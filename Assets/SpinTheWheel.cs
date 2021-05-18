using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpinTheWheel : MonoBehaviour
{
    private int[][] wheelrewards = new int[8][] { new int[3] { 25, 41, 91 } , new int[3] { 25, 140, 183 }, new int[3] { 25, 225, 271 } , new int[3] { 25, 314, 360 },
    new int[3] { 50, 0, 41 }, new int[3] { 75, 272, 314 }, new int[3] { 100, 183, 225 }, new int[3] { 150, 93, 138 }};

    System.Random R = new System.Random();

    public float smooth = 5.0f;
    private float time = 0f;

    public Text[] texts;

    Quaternion target;
    Quaternion oldrot;
    private int counter = -1;
    private int additionalrew;

    public ValuesChange v;

    public Button spinbut;

    private int index;

    float timeclamp = 0f;


    private void OnEnable()
    {
        spinbut.interactable = true;
        foreach (Text t in texts)
        {
            Int32.TryParse(t.text, out int i);
            additionalrew = PlayerPrefs.GetInt("ConsDays") * 25;
            // Add rewards for days > 20
            if (additionalrew > 500) { additionalrew = 500; Debug.LogError("Reached maximum rewards, the increase is capped at 500. Thank you for playing Shapes Clash !"); };
            i += additionalrew;
            t.text = i.ToString();
        }
    }

    public void Spin()
    {
        spinbut.interactable = false;
        index = R.Next(0, 8);
        int rotVal = R.Next(wheelrewards[index][1], wheelrewards[index][2]);
        target = Quaternion.Euler(0, 0, rotVal);
        oldrot = transform.rotation;
        time = 0f;
        counter = 0;
    }

    private void Update()
    {
        if (counter >= 0 && counter < 2)
        {
            transform.rotation = Quaternion.Slerp(oldrot, Quaternion.Euler(oldrot.eulerAngles.x, oldrot.eulerAngles.y, oldrot.eulerAngles.z + 180), time * smooth);
            time += Time.deltaTime;
            if (transform.rotation == Quaternion.Euler(oldrot.eulerAngles.x, oldrot.eulerAngles.y, oldrot.eulerAngles.z + 180) || transform.rotation == Quaternion.Euler(oldrot.eulerAngles.x, oldrot.eulerAngles.y, oldrot.eulerAngles.z - 180))
            {
                counter++;
                oldrot = transform.rotation;
                time = 0f;
            }
        }
        else if (counter >= 2 && transform.rotation != target && timeclamp!=1f)
        {
            timeclamp = Mathf.Clamp(time * smooth, 0f, 1f);
            transform.rotation = Quaternion.Slerp(oldrot, target, timeclamp);
            time += Time.deltaTime;
        }
        else if ((transform.rotation == target || (timeclamp == 1 && counter >=2)) && counter > 0)
        {
            counter = -1;
            v.playResourcesAnim(0);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + wheelrewards[index][0]);
            PlayerPrefs.Save();
            ValuesChange.coins = PlayerPrefs.GetInt("Gold");
            v.topBar.transform.Find("Coins").GetComponentInChildren<Text>().text = ValuesChange.coins.ToString();
            PlayerPrefs.SetInt("DailyReward", 0);
            ClientTCP.PACKAGE_ChestOpening(true);
        }
    }
}
