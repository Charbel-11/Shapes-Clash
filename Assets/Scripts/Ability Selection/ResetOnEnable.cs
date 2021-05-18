using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ResetOnEnable : MonoBehaviour {

    private SelectionManager SM;

    private void OnEnable()
    {
        SM = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();

        SM.IDOfSelectedAbility = -1;
        if (SM.goOfSelectedAb != null)
        {
            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
            SM.goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
            SM.goOfSelectedAb.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            SM.goOfSelectedAb.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 255f);
        }

        //Used to correctly update selected abs in the main screen
        SM.UpdatePoolOfAbilities();
    }
}
