using UnityEngine;
using UnityEngine.UI;

public class ChangeAbSound : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Slider>().value = PlayerPrefs.GetFloat("AbVolume");
    }
    public void Change()
    {
        float Val = gameObject.GetComponent<Slider>().value;
        PlayerPrefs.SetFloat("AbVolume", Val);
    }
}
