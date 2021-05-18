using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesPanels : MonoBehaviour
{
    public bool selectionScreen;

    private Transform Common, Specific, Special, Passives;
    private Transform CommonP, SpecificP, SpecialP, PassivesP;
    private ShopManager SM;
    private SelectionManager SM2;
    private bool initialized = false;

    private void Start()
    {     
        openCommon();
    }

    public void openCommon()
    {
        if (!initialized)
        {
            initialized = true;
            Common = gameObject.transform.Find("Common");
            Specific = gameObject.transform.Find("Specific");
            Special = gameObject.transform.Find("Special");

            CommonP = gameObject.transform.Find("CommonP");
            SpecificP = gameObject.transform.Find("SpecificP");
            SpecialP = gameObject.transform.Find("SpecialP");

            if (!selectionScreen)
            {
                Passives = gameObject.transform.Find("Passives");
                PassivesP = gameObject.transform.Find("PassivesP");
                SM = GameObject.Find("Shop Manager").GetComponent<ShopManager>();
            }
            else
            {
                SM2 = GameObject.Find("SelectionManager").GetComponent<SelectionManager>();
            }          
        }

        if (!selectionScreen && !SM.canChange) { return; }
        if (DragHandler.abilityDragged != null) { return; }
        if (!selectionScreen)
        {
            SM.Describer.SetActive(false);
            SM.passiveDescription.SetActive(false);
        }

        Common.gameObject.GetComponent<Image>().fillCenter = true;
        Color temp = new Color(.5843f, .5843f, .5843f); temp.a = 1f;
        Common.gameObject.GetComponent<Image>().color = temp;
        var colT = Common.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(.5843f, .5843f, .5843f);
        Common.gameObject.GetComponent<Button>().colors = colT;
        Common.gameObject.GetComponent<Button>().interactable = false;
        Common.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 20);

        temp = new Color(1f, 1f, 1f); temp.a = 0.7f;
        colT = Specific.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(1f, 1f, 13f);
        Specific.gameObject.GetComponent<Image>().fillCenter = false;
        Specific.gameObject.GetComponent<Image>().color = temp;
        Specific.gameObject.GetComponent<Button>().colors = colT;
        Specific.gameObject.GetComponent<Button>().interactable = true;
        Specific.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        Special.gameObject.GetComponent<Image>().fillCenter = false;
        Special.gameObject.GetComponent<Image>().color = temp;
        Special.gameObject.GetComponent<Button>().colors = colT;
        Special.gameObject.GetComponent<Button>().interactable = true;
        Special.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        CommonP.gameObject.SetActive(true);
        SpecificP.gameObject.SetActive(false);
        SpecialP.gameObject.SetActive(false);

        if (!selectionScreen)
        {
            Passives.gameObject.GetComponent<Image>().fillCenter = false;
            Passives.gameObject.GetComponent<Image>().color = temp;
            Passives.gameObject.GetComponent<Button>().colors = colT;
            Passives.gameObject.GetComponent<Button>().interactable = true;
            Passives.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            PassivesP.gameObject.SetActive(false);

            SM.panelOpen = 1;
            SM.resetSelected(true);
        }
        else
        {
            SM2.StopTheHighlight();
            if (SM2.goOfSelectedAb != null)
            {
                SM2.goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
                SM2.goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
                SM2.goOfSelectedAb.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                SM2.goOfSelectedAb.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                SM2.IDOfSelectedAbility = -1;
            }
        }
    }

    public void openSpecific()
    {
        if (!selectionScreen && !SM.canChange) { return; }
        if (DragHandler.abilityDragged != null) { return; }
        if (!selectionScreen)
        {
            SM.Describer.SetActive(false);
            SM.passiveDescription.SetActive(false);
            SM.CommonInfo.SetActive(false);
        }

        Specific.gameObject.GetComponent<Image>().fillCenter = true;
        Color temp = new Color(.5843f, .5843f, .5843f); temp.a = 1f;
        Specific.gameObject.GetComponent<Image>().color = temp;
        var colT = Specific.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(.5843f, .5843f, .5843f);
        Specific.gameObject.GetComponent<Button>().colors = colT;
        Specific.gameObject.GetComponent<Button>().interactable = false;
        Specific.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 20);

        temp = new Color(1f, 1f, 1f); temp.a = 0.7f;
        colT = Common.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(1f, 1f, 13f);
        Common.gameObject.GetComponent<Image>().fillCenter = false;
        Common.gameObject.GetComponent<Image>().color = temp;
        Common.gameObject.GetComponent<Button>().colors = colT;
        Common.gameObject.GetComponent<Button>().interactable = true;
        Common.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        Special.gameObject.GetComponent<Image>().fillCenter = false;
        Special.gameObject.GetComponent<Image>().color = temp;
        Special.gameObject.GetComponent<Button>().colors = colT;
        Special.gameObject.GetComponent<Button>().interactable = true;
        Special.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        CommonP.gameObject.SetActive(false);
        SpecificP.gameObject.SetActive(true);
        SpecialP.gameObject.SetActive(false);

        if (!selectionScreen)
        {
            Passives.gameObject.GetComponent<Image>().fillCenter = false;
            Passives.gameObject.GetComponent<Image>().color = temp;
            Passives.gameObject.GetComponent<Button>().colors = colT;
            Passives.gameObject.GetComponent<Button>().interactable = true;
            Passives.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

            PassivesP.gameObject.SetActive(false);

            SM.panelOpen = 2;
            SM.resetSelected(true);
            SM.openSpecific();
        }
        else
        {
            SM2.StopTheHighlight();
            if (SM2.goOfSelectedAb != null)
            {
                SM2.goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
                SM2.goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
                SM2.goOfSelectedAb.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                SM2.goOfSelectedAb.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                SM2.IDOfSelectedAbility = -1;
            }
        }
    }

    public void openSpecial()
    {
        if (!selectionScreen && !SM.canChange) { return; }
        if (DragHandler.abilityDragged != null) { return; }
        if (!selectionScreen)
        {
            SM.Describer.SetActive(false);
            SM.passiveDescription.SetActive(false);
            SM.CommonInfo.SetActive(false);
        }
        Special.gameObject.GetComponent<Image>().fillCenter = true;
        Color temp = new Color(.5843f, .5843f, .5843f); temp.a = 1f;
        Special.gameObject.GetComponent<Image>().color = temp;
        var colT = Special.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(.5843f, .5843f, .5843f);
        Special.gameObject.GetComponent<Button>().colors = colT;
        Special.gameObject.GetComponent<Button>().interactable = false;
        Special.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 20);

        temp = new Color(1f, 1f, 1f); temp.a = 0.7f;
        colT = Common.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(1f, 1f, 13f);
        Common.gameObject.GetComponent<Image>().fillCenter = false;
        Common.gameObject.GetComponent<Image>().color = temp;
        Common.gameObject.GetComponent<Button>().colors = colT;
        Common.gameObject.GetComponent<Button>().interactable = true;
        Common.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        Specific.gameObject.GetComponent<Image>().fillCenter = false;
        Specific.gameObject.GetComponent<Image>().color = temp;
        Specific.gameObject.GetComponent<Button>().colors = colT;
        Specific.gameObject.GetComponent<Button>().interactable = true;
        Specific.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        CommonP.gameObject.SetActive(false);
        SpecificP.gameObject.SetActive(false);
        SpecialP.gameObject.SetActive(true);

        if (!selectionScreen)
        {
            Passives.gameObject.GetComponent<Image>().fillCenter = false;
            Passives.gameObject.GetComponent<Image>().color = temp;
            Passives.gameObject.GetComponent<Button>().colors = colT;
            Passives.gameObject.GetComponent<Button>().interactable = true;
            Passives.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

            PassivesP.gameObject.SetActive(false);

            SM.panelOpen = 3;
            SM.resetSelected(true);
            SM.openSpecial();
        }
        else
        {
            SM2.StopTheHighlight();
            if (SM2.goOfSelectedAb != null)
            {
                SM2.goOfSelectedAb.GetComponent<AbilityToSelect>().SetCurrentlySelected(false);
                SM2.goOfSelectedAb.GetComponent<AbilityToSelect>().SetClickedOnce(false);
                SM2.goOfSelectedAb.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                SM2.goOfSelectedAb.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                SM2.IDOfSelectedAbility = -1;
            }
        }
    }

    public void openPassives()
    {
        if (!SM.canChange) { return; }
        if (DragHandler.abilityDragged != null) { return; }
        if (!selectionScreen)
        {
            SM.Describer.SetActive(false);
            SM.passiveDescription.SetActive(false);
            SM.CommonInfo.SetActive(false);
        }
        Passives.gameObject.GetComponent<Image>().fillCenter = true;
        Color temp = new Color(.5843f, .5843f, .5843f); temp.a = 1f;
        Passives.gameObject.GetComponent<Image>().color = temp;
        var colT = Passives.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(.5843f, .5843f, .5843f);
        Passives.gameObject.GetComponent<Button>().colors = colT;
        Passives.gameObject.GetComponent<Button>().interactable = false;
        Passives.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 20);

        temp = new Color(1f, 1f, 1f); temp.a = 0.7f;
        colT = Common.gameObject.GetComponent<Button>().colors; colT.normalColor = new Color(1f, 1f, 13f);
        Common.gameObject.GetComponent<Image>().fillCenter = false;
        Common.gameObject.GetComponent<Image>().color = temp;
        Common.gameObject.GetComponent<Button>().colors = colT;
        Common.gameObject.GetComponent<Button>().interactable = true;
        Common.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        Specific.gameObject.GetComponent<Image>().fillCenter = false;
        Specific.gameObject.GetComponent<Image>().color = temp;
        Specific.gameObject.GetComponent<Button>().colors = colT;
        Specific.gameObject.GetComponent<Button>().interactable = true;
        Specific.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        Special.gameObject.GetComponent<Image>().fillCenter = false;
        Special.gameObject.GetComponent<Image>().color = temp;
        Special.gameObject.GetComponent<Button>().colors = colT;
        Special.gameObject.GetComponent<Button>().interactable = true;
        Special.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        CommonP.gameObject.SetActive(false);
        SpecificP.gameObject.SetActive(false);
        SpecialP.gameObject.SetActive(false);
        PassivesP.gameObject.SetActive(true);

        SM.panelOpen = 4;
        SM.resetSelected(true);
        SM.openPassives();
    }
}
