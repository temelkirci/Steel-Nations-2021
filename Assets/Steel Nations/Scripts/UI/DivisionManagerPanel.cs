using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit
{
    public class DivisionManagerPanel : MonoBehaviour
    {
        private static DivisionManagerPanel instance;
        public static DivisionManagerPanel Instance
        {
            get { return instance; }
        }

        public GameObject divisionPanel;
        public GameObject divisionPrefab;
        public RectTransform divisionRect;
        public TextMeshProUGUI divisionName;
        public TextMeshProUGUI divisionAction;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }
        
        
        public void ShowDivisionPanel(Division division)
        {
            if (division == null)
                return;

            divisionName.text = division.divisionName;

            foreach (Transform child in divisionRect.transform)
            {
                Destroy(child.gameObject);
            }

            //foreach (Weapon weapon in showDivision.GetDivision().GetWeaponsInDivision())
            {
                GameObject temp = Instantiate(divisionPrefab, divisionRect.transform);
                temp.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(division.MainWeapon).weaponName;
                temp.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(division.MainWeapon);

                int mainUnitNumber = division.GetWeaponListByWeaponIDInDivision(division.MainWeapon).Count;

                temp.gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = mainUnitNumber.ToString();

                GameObject temp1 = Instantiate(divisionPrefab, divisionRect.transform);
                temp1.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(division.SecondWeapon).weaponName;
                temp1.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(division.SecondWeapon);

                int secondUnitNumber = division.GetWeaponListByWeaponIDInDivision(division.SecondWeapon).Count;

                temp1.gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = secondUnitNumber.ToString();

                GameObject temp2 = Instantiate(divisionPrefab, divisionRect.transform);
                temp2.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(division.ThirdWeapon).weaponName;
                temp2.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(division.ThirdWeapon);

                int thirdUnitNumber = division.GetWeaponListByWeaponIDInDivision(division.ThirdWeapon).Count;

                temp2.gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = thirdUnitNumber.ToString();
            }

            divisionPanel.SetActive(true);
        }
        
    }
}