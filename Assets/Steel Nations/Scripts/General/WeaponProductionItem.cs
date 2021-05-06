using TMPro;
using UnityEngine;
using WorldMapStrategyKit;
using UnityEngine.UI;
using System;
using Michsky.UI.Frost;

public class WeaponProductionItem : MonoBehaviour
{
    WeaponTemplate weapon = null;
    Production production = null;

    public RawImage weaponIcon;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponCost;
    public TextMeshProUGUI productionTime;
    public TMP_InputField weaponNumberText;

    public Button produceButton;
    public GameObject generation;
    public GameObject producing;


    public void SetWeapon(WeaponTemplate tempWeapon)
    {
        weapon = tempWeapon;
        weaponIcon.texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weapon.weaponID);
        weaponName.text = tempWeapon.weaponName;
        weaponCost.text = "$ " + string.Format("{0:#,0}", tempWeapon.weaponCost.ToString()) + " M";

        produceButton.GetComponent<Button>().onClick.AddListener(() => StartProduction());

        produceButton.interactable = true;

        productionTime.text = CountryManager.Instance.GetWeaponProductionDay(GameEventHandler.Instance.GetPlayer().GetMyCountry(), weapon).ToString() + " day";
    }

    public void StartProduction()
    {
        if(weaponNumberText.text != string.Empty)
        {
            int weaponNumber = 0;

            weaponNumber = Int32.Parse(weaponNumberText.text);

            if(weaponNumber > 0)
            {
                production = CountryManager.Instance.ProductWeapon(GameEventHandler.Instance.GetPlayer().GetMyCountry(), weapon, weaponNumber);

                if (production != null)
                {
                    producing.SetActive(true);

                    producing.GetComponent<TimedProgressBar>().currentPercent = 0;
                }
            }
        }
    }

    public void CompleteProduction()
    {
        produceButton.interactable = true;
        producing.SetActive(false);

        production = null;
    }

    public void UpdateProduction()
    {
        if (production != null)
        {
            if (production.IsCompleted() == true)
            {
                CompleteProduction();
            }
            else
            {
                producing.GetComponent<TimedProgressBar>().currentPercent = production.GetPercent();
            }
        }
        else
        {
            producing.SetActive(false);
        }
    }
}
