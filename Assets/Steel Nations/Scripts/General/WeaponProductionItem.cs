using TMPro;
using UnityEngine;
using WorldMapStrategyKit;
using UnityEngine.UI;

public class WeaponProductionItem : MonoBehaviour
{
    WeaponTemplate weapon = null;
    Production production = null;

    public RawImage weaponIcon;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponCost;
    public TextMeshProUGUI productionTime;
    public TextMeshProUGUI leftWeapon;
    public TextMeshProUGUI totalWeapon;

    public Button produceButton;
    public GameObject generation;
    public GameObject producing;


    public void SetWeapon(WeaponTemplate tempWeapon)
    {
        weapon = tempWeapon;
        weaponIcon.texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weapon.weaponID);
        weaponName.text = tempWeapon.weaponName;
        weaponCost.text = "$ " + string.Format("{0:#,0}", tempWeapon.weaponCost.ToString()) + " M";
        productionTime.text = tempWeapon.weaponProductionTime.ToString() + " day";

        produceButton.GetComponent<Button>().onClick.AddListener(() => StartProduction());

        produceButton.interactable = true;
    }

    public void StartProduction()
    {
        production = CountryManager.Instance.ProductWeapon(GameEventHandler.Instance.GetPlayer().GetMyCountry(), weapon);

        if (production != null)
        {
            producing.SetActive(true);
        }
    }
    public void CompleteProduction()
    {
        produceButton.interactable = true;
        producing.SetActive(false);

        Debug.Log("Completed");

        production = null;
    }

    // Update is called once per frame
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
                producing.SetActive(true);
            }
        }
        else
        {
            producing.SetActive(false);
        }
    }
}
