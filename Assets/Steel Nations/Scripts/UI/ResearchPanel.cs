using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit
{
    public class ResearchPanel : MonoBehaviour
    {
        private static ResearchPanel instance;
        public static ResearchPanel Instance
        {
            get { return instance; }
        }

        public GameObject researchPanel;
        public GameObject weaponItem;
        public GameObject weaponContentItem;

        public GameObject redWeaponLine;
        public GameObject orangeWeaponLine;
        public GameObject yellowWeaponLine;
        public GameObject purpleWeaponLine;

        public GameObject weaponContent;

        public GameObject weaponNameGO;

        public GameObject landLine;
        public GameObject airLine;
        public GameObject navalLine;
        public GameObject missileLine;

        public GameObject landFirstLine;
        public GameObject airFirstLine;
        public GameObject navalFirstLine;
        public GameObject missileFirstLine;

        public GameObject selectedResearchPanel;
        public GameObject researchSlider;

        public TextMeshProUGUI weaponName;
        public TextMeshProUGUI researchCost;
        public TextMeshProUGUI researchDuring;
        public TextMeshProUGUI universityNumber;
        public TextMeshProUGUI researchSpeed;
        public TextMeshProUGUI totalResearchSpeed;

        Research selectedResearch;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void AddWeapon(WeaponTemplate weapon, GameObject lineGOs)
        {
            GameObject temp = null;
            GameObject content = null;

            foreach (Transform eachChild in weaponContent.transform)
            {
                if (eachChild.name == weapon.weaponName)
                {
                    content = eachChild.gameObject;
                    break;
                }
            }         

            if (content == null)
            {
                content = Instantiate(weaponContentItem, weaponContent.transform);
                content.name = weapon.weaponName;

                if(weapon.weaponTerrainType == 0 || weapon.weaponTerrainType == 1)
                    Instantiate(landFirstLine, content.transform);
                if (weapon.weaponTerrainType == 2)
                    Instantiate(navalFirstLine, content.transform);
                if (weapon.weaponTerrainType == 3)
                    Instantiate(airFirstLine, content.transform);
                if (weapon.weaponTerrainType == 4)
                    Instantiate(missileFirstLine, content.transform);

                GameObject weaponNameText = Instantiate(weaponNameGO, content.transform);
                weaponNameText.GetComponent<TextMeshProUGUI>().text = weapon.weaponName;              
            }
            else
            {
                GameObject GO = Instantiate(lineGOs, content.transform);

                if (GameEventHandler.Instance.GetPlayer().GetMyCountry().IsWeaponProducible(weapon.weaponID) == false)
                {
                    GO.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.3f);
                }
            }


            temp = Instantiate(weaponItem, content.transform);

            if (GameEventHandler.Instance.GetPlayer().GetMyCountry().IsWeaponProducible(weapon.weaponID) == false)
            {
                temp.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            }

            if (temp != null && weapon != null)
            {
                string tooltipText = string.Empty;

                temp.gameObject.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => UpdateSelectedWeapon(weapon));

                tooltipText = weapon.weaponName + "\n" + "Cost : " + weapon.weaponCost.ToString() + "\n" + "Attack Range : " + weapon.weaponAttackRange.ToString() + "\n" + "Defense : " + weapon.weaponDefense.ToString() + "\n";
                temp.gameObject.GetComponent<SimpleTooltip>().infoLeft = tooltipText;

                temp.gameObject.transform.GetChild(1).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weapon.weaponID);
            }
        }

        public void ShowLandUnits()
        {
            ClearUnits();

            HideAllLines();

            weaponContent.SetActive(true);
            landLine.SetActive(true);

            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if (weapon.weaponTerrainType == 0 || weapon.weaponTerrainType == 1)
                {
                    AddWeapon(weapon, redWeaponLine);
                }  
            }
        }

        public void ShowNavalUnits()
        {
            ClearUnits();

            HideAllLines();

            weaponContent.SetActive(true);
            navalLine.SetActive(true);

            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if (weapon.weaponTerrainType == 2)
                {
                    AddWeapon(weapon, orangeWeaponLine);
                }
            }
        }

        public void ShowAirUnits()
        {
            ClearUnits();

            HideAllLines();

            weaponContent.SetActive(true);
            airLine.SetActive(true);

            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if (weapon.weaponTerrainType == 3)
                {
                    AddWeapon(weapon, yellowWeaponLine);
                }
            }
        }

        public void ShowMissileUnits()
        {
            ClearUnits();

            HideAllLines();

            weaponContent.SetActive(true);
            missileLine.SetActive(true);

            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                if (weapon.weaponTerrainType == 4)
                {
                    AddWeapon(weapon, purpleWeaponLine);
                }
            }
        }

        public void HideAllLines()
        {
            landLine.SetActive(false);
            airLine.SetActive(false);
            navalLine.SetActive(false);
            missileLine.SetActive(false);
        }

        public void HideResearchPanel()
        {
            researchPanel.SetActive(false);

            ClearUnits();

            selectedResearchPanel.SetActive(false);
        }

        public void ShowResearchPanel()
        {
            researchPanel.SetActive(true);

            ClearUnits();

            HideAllLines();

            selectedResearchPanel.SetActive(false);
        }

        void UpdateSelectedWeapon(WeaponTemplate weapon)
        {
            selectedResearchPanel.SetActive(true);

            int researchSpeedRotio = (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetTotalBuildings(BUILDING_TYPE.UNIVERSITY) / 10 ) + (GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Research Speed"]);
            weaponName.text = weapon.weaponName;
            researchCost.text = weapon.weaponResearchCost.ToString();
            researchDuring.text = weapon.weaponResearchTime.ToString();
            universityNumber.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetTotalBuildings(BUILDING_TYPE.UNIVERSITY).ToString();
            researchSpeed.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetResearchSpeed().ToString();

            int weaponResearchTime = (weapon.weaponResearchTime - ( (weapon.weaponResearchTime * researchSpeedRotio) / 100 ) );
            if (weaponResearchTime <= 1)
                weaponResearchTime = 1;

            totalResearchSpeed.text = weaponResearchTime.ToString();

            selectedResearchPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weapon.weaponID);
            selectedResearchPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>().gameObject.SetActive(true);

            if(GameEventHandler.Instance.GetPlayer().GetMyCountry().IsWeaponProducible(weapon.weaponID) == false)
                selectedResearchPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => GameEventHandler.Instance.GetPlayer().GetMyCountry().ResearchWeapon(weapon));
        }

        public void ShowResearchProgress()
        {
            if (researchPanel.activeSelf == true)
            {
                foreach (Research research in GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAllResearchsInProgress())
                {
                    if (research.researchCountries.Contains(GameEventHandler.Instance.GetPlayer().GetMyCountry()))
                    {
                        selectedResearch = research;
                        break;
                    }
                }

                if (selectedResearch == null)
                    return;

                if (selectedResearch.leftDays >= 0)
                {
                    researchSlider.GetComponent<Slider>().value = 100f - (selectedResearch.leftDays * 100f) / selectedResearch.totalResearchDay;
                }
                else
                {
                    researchSlider.GetComponent<Slider>().value = 0f;
                }
            }
        }

        void ClearUnits()
        {
            foreach (Transform child in weaponContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}