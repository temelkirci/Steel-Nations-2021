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

        public static void CreateDivisionTemplate(string mainUnit, 
            string secondUnit, 
            string thirdUnit, 
            int maxMainUnit, 
            int maxSecondUnit, 
            int maxThirdUnit, 
            int soldierMinimum, 
            int soldierMaximum, 
            string divisionType)
        {
            string[] mainUnitIDList = mainUnit.Split('~');
            string[] secondUnitIDList = secondUnit.Split('~');
            string[] thirdUnitIDList = thirdUnit.Split('~');


            DivisionTemplate divisionTemplate = new DivisionTemplate();
            divisionTemplate.SetDivisionTypeByDivisionName(divisionType);
            divisionTemplate.SetDivisionMainWeaponByWeaponName(mainUnitIDList, maxMainUnit);
            divisionTemplate.SetDivisionSecondWeaponByWeaponName(secondUnitIDList, maxSecondUnit);
            divisionTemplate.SetDivisionThirdWeaponByWeaponName(thirdUnitIDList, maxThirdUnit);

            divisionTemplate.minimumSoldier = soldierMinimum;
            divisionTemplate.maximumSoldier = soldierMaximum;

            DivisionManager.Instance.AddDivisionTemplate(divisionTemplate);
        }

        public static DivisionTemplate GetDivisionTemplateByType(DIVISION_TYPE tempDivisionType)
        {
            foreach (DivisionTemplate divisionTemplate in DivisionManager.Instance.GetDivisionTemplate())
            {
                if (divisionTemplate.divisionType == tempDivisionType)
                {                   
                    return divisionTemplate;
                }
            }

            return null;
        }
        
        
        public void ShowDivisionPanel()
        {
            GameObjectAnimator showDivision = GameEventHandler.Instance.GetPlayer().GetSelectedDivisions()[0];

            if (showDivision == null)
                return;

            divisionName.text = showDivision.GetDivision().divisionName;

            foreach (Transform child in divisionRect.transform)
            {
                Destroy(child.gameObject);
            }

            //foreach (Weapon weapon in showDivision.GetDivision().GetWeaponsInDivision())
            {
                GameObject temp = Instantiate(divisionPrefab, divisionRect.transform);
                temp.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(showDivision.GetDivision().divisionTemplate.mainUnitIDList[0]).weaponName;
                temp.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(showDivision.GetDivision().divisionTemplate.mainUnitIDList[0]);

                int mainUnitNumber = 0;
                foreach(int i in showDivision.GetDivision().divisionTemplate.mainUnitIDList)
                    mainUnitNumber += showDivision.GetDivision().GetWeaponNumberByWeaponIDInDivision(i).Count;

                temp.gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = mainUnitNumber.ToString();

                GameObject temp1 = Instantiate(divisionPrefab, divisionRect.transform);
                temp1.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(showDivision.GetDivision().divisionTemplate.secondUnitList[0]).weaponName;
                temp1.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(showDivision.GetDivision().divisionTemplate.secondUnitList[0]);

                int secondUnitNumber = 0;
                foreach (int i in showDivision.GetDivision().divisionTemplate.secondUnitList)
                    secondUnitNumber += showDivision.GetDivision().GetWeaponNumberByWeaponIDInDivision(i).Count;

                temp1.gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = secondUnitNumber.ToString();

                GameObject temp2 = Instantiate(divisionPrefab, divisionRect.transform);
                temp2.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = WeaponManager.Instance.GetWeaponTemplateByID(showDivision.GetDivision().divisionTemplate.thirdUnitList[0]).weaponName;
                temp2.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(showDivision.GetDivision().divisionTemplate.thirdUnitList[0]);

                int thirdUnitNumber = 0;
                foreach (int i in showDivision.GetDivision().divisionTemplate.thirdUnitList)
                    thirdUnitNumber += showDivision.GetDivision().GetWeaponNumberByWeaponIDInDivision(i).Count;

                temp2.gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = thirdUnitNumber.ToString();
            }

            divisionPanel.SetActive(true);
        }
        
    }
}