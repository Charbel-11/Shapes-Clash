using UnityEngine.UI;
using UnityEngine;

public class ShapeStart : MonoBehaviour
{
    int ShapeChosen = 0;
    public GameObject AreYouSurePanel;
    public Button[] buttons;
    public void ChooseShape(int shapeID)
    {
        ShapeChosen = shapeID;
        foreach (Button b in buttons)
            b.interactable = false;
        AreYouSurePanel.SetActive(true);
    }

    public void SendChoiceToServer()
    {
        int[] Lvl = new int[4] { 0, 0, 0, 0 };
        Lvl[ShapeChosen] = 1;
        PlayerPrefsX.SetIntArray("Level", Lvl);
        PlayerPrefs.SetInt("ShapeSelectedID", ShapeChosen);
        PlayerPrefs.SetInt("2ShapeSelectedID", ShapeChosen);
        PlayerPrefs.Save();
        ClientTCP.PACKAGE_DEBUG("ShapeChoice", PlayerPrefs.GetString("Username"), ShapeChosen);
    }
}
