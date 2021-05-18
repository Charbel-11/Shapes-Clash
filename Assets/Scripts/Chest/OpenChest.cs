using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenChest : MonoBehaviour
{
    public GameObject[] Chests;
    public GameObject[] Children;
    public Animator Anim;
    public Sprite[] Sprites;
    public Sprite[] Super100s;
    public Sprite[] Super200s;
    public Sprite[] ShapesSpr;
    public Image CommonAb;
    public Image RareAb;
    public Image MythicAb;
    public Image MajesticAb;
    public Image Shapes;

    public void Initialize(Chest TheChest)
    {
        for(int i = 0; i < Chests.Length; i++)
        {
            Chests[i].SetActive(i == TheChest.SpriteNum);
        }
        Anim = Chests[TheChest.SpriteNum].GetComponent<Animator>();
    }
}
