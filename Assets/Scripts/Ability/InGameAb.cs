using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameAb : MonoBehaviour {
    public bool canUseAb;

    private Button but;

    private void Start()
    {
        canUseAb = true;
        but = GetComponent<Button>();
        but.onClick.AddListener(click);
    }

    private void click()
    {
        if (canUseAb)
            GetComponent<Shape_Abilities>().UseAbility();
    }
}
