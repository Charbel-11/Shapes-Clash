using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChange : MonoBehaviour
{
    public int QueueNumber;
    public Sprite[] Sprites;

    private GameObject Lock;
    private Button ThisBut;

    private void Awake()
    {
        ThisBut = GetComponent<Button>();
        Lock = transform.Find("Lock").gameObject;
        Lock.SetActive(false);
    }
    private void OnEnable()
    {
        int[] Queue = PlayerPrefsX.GetIntArray("Queue");
        int WS = Queue[QueueNumber];
        int Arena = Queue[QueueNumber + 1];

        if(WS == -1)
        {
            Lock.SetActive(true);
            ThisBut.image.sprite = Sprites[5];
            return;
        }

        Chest TheChest = new Chest(WS, Arena);
        ThisBut.image.sprite = Sprites[TheChest.SpriteNum];
    }
}
