using UnityEngine;
using UnityEngine.UI;

public class ChangeSound : MonoBehaviour {
    public ChangeVolume Sound;
    private void Start()
    {
        gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Volume");
    }
    public void Change()
    {
        float Val = gameObject.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("Volume",Val);
        Sound.ChangeIt();
    }
}
