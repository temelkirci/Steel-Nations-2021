using TMPro;
using UnityEngine;
using WorldMapStrategyKit;
using UnityEngine.UI;

public class WeaponResearchItem : MonoBehaviour
{
    WeaponTemplate weapon = null;
    Research research = null;

    public RawImage weaponIcon;
    public Button researchButton;

    public void SetWeapon(WeaponTemplate tempWeapon)
    {
        weapon = tempWeapon;
        weaponIcon.texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weapon.weaponID);
        researchButton.GetComponent<Button>().onClick.AddListener(() => ShowReserach());        
    }
    
    public WeaponTemplate GetWeaponTemplate()
    {
        return weapon;
    }

    public void ShowReserach()
    {
        if (weapon != null)
        {
            ResearchPanel.Instance.UpdateSelectedWeapon(this);
        }
        else
        {
            Debug.Log("budget or resource are not enough");
        }
    }

    public void StartReserach()
    {
        research = CountryManager.Instance.ResearchWeapon(GameEventHandler.Instance.GetPlayer().GetMyCountry(), weapon);

        if (research != null)
        {
            researchButton.interactable = true;
            ResearchPanel.Instance.researchSlider.SetActive(true);

            ResearchPanel.Instance.selectedResearch = research;
            Debug.Log(weapon.weaponName + " starting");
        }
        else
        {
            Debug.Log("budget or resource are not enough");
        }
    }

    public void CancelResearch()
    {
        if (research != null)
        {
            GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAllResearchsInProgress().Remove(research);

            ResearchPanel.Instance.selectedResearch = null;
            Debug.Log(weapon.weaponName + " was canceled");
        }
        else
        {
            Debug.Log("budget or resource are not enough");
        }
    }

    public Research GetResearch()
    {
        return research;
    }

}
