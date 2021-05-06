using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

        public GameObject landContent;
        public GameObject airContent;
        public GameObject navalContent;
        public GameObject missileContent;

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
        public GameObject researchButton;
        public GameObject cancelResearchButton;

        public TextMeshProUGUI weaponName;
        public TextMeshProUGUI researchCost;
        public TextMeshProUGUI researchDuring;
        public TextMeshProUGUI universityNumber;
        public TextMeshProUGUI researchSpeed;
        public TextMeshProUGUI totalResearchSpeed;

        public Research selectedResearch;

        List<GameObject> landWeaponResearchList = new List<GameObject>();
        List<GameObject> airWeaponResearchList = new List<GameObject>();
        List<GameObject> navalWeaponResearchList = new List<GameObject>();
        List<GameObject> missileWeaponResearchList = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void Init()
        {
            foreach (WeaponTemplate weapon in WeaponManager.Instance.GetWeaponTemplateList())
            {
                GameObject weaponContent = null;

                if (weapon.weaponTerrainType == 0 || weapon.weaponTerrainType == 1)
                    weaponContent = landContent;
                if (weapon.weaponTerrainType == 2)
                    weaponContent = navalContent;
                if (weapon.weaponTerrainType == 3)
                    weaponContent = airContent;
                if (weapon.weaponTerrainType == 4)
                    weaponContent = missileContent;

                AddWeapon(weapon, redWeaponLine, weaponContent);
            }
        }

        public void AddWeapon(WeaponTemplate weapon, GameObject lineGOs, GameObject weaponContent)
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

            if (temp != null && weapon != null)
            {
                UpdateResearch(weapon, temp);

                temp.GetComponent<WeaponResearchItem>().SetWeapon(weapon);

                if (weapon.weaponTerrainType == 0 || weapon.weaponTerrainType == 1)
                    landWeaponResearchList.Add(temp);
                if (weapon.weaponTerrainType == 2)
                    navalWeaponResearchList.Add(temp);
                if (weapon.weaponTerrainType == 3)
                    airWeaponResearchList.Add(temp);
                if (weapon.weaponTerrainType == 4)
                    missileWeaponResearchList.Add(temp);
            }
        }

        public void UpdateResearch(WeaponTemplate weapon, GameObject weaponGO)
        {
            if (GameEventHandler.Instance.GetPlayer().GetMyCountry().IsWeaponProducible(weapon.weaponID) == false)
            {
                weaponGO.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            }

            weaponGO.name = weapon.weaponID.ToString();

            string tooltipText = string.Empty;

            tooltipText = weapon.weaponName + "\n" +
                "Cost : $ " + string.Format("{0:#,0}", float.Parse(weapon.weaponCost.ToString())) + " M" + "\n" +
                "Attack Range : " + weapon.weaponAttackRange.ToString() + " km" + "\n" +
                "Defense : " + weapon.weaponDefense.ToString() + "\n";

            weaponGO.gameObject.GetComponent<SimpleTooltip>().infoLeft = tooltipText;
        }

        public void ShowLandUnits()
        {
            HideAllLines();

            landContent.SetActive(true);
            airContent.SetActive(false);
            navalContent.SetActive(false);
            missileContent.SetActive(false);

            landLine.SetActive(true);

            foreach(GameObject GO in landWeaponResearchList)
            {
                WeaponTemplate weapon = GO.GetComponent<WeaponResearchItem>().GetWeaponTemplate();

                UpdateResearch(weapon, GO);
            }
        }

        public void ShowNavalUnits()
        {
            HideAllLines();

            landContent.SetActive(false);
            airContent.SetActive(false);
            navalContent.SetActive(true);
            missileContent.SetActive(false);
            
            navalLine.SetActive(true);

            foreach (GameObject GO in navalWeaponResearchList)
            {
                WeaponTemplate weapon = GO.GetComponent<WeaponResearchItem>().GetWeaponTemplate();

                UpdateResearch(weapon, GO);
            }
        }

        public void ShowAirUnits()
        {
            HideAllLines();

            landContent.SetActive(false);
            airContent.SetActive(true);
            navalContent.SetActive(false);
            missileContent.SetActive(false);
            
            airLine.SetActive(true);

            foreach (GameObject GO in airWeaponResearchList)
            {
                WeaponTemplate weapon = GO.GetComponent<WeaponResearchItem>().GetWeaponTemplate();

                UpdateResearch(weapon, GO);
            }
        }

        public void ShowMissileUnits()
        {
            HideAllLines();

            landContent.SetActive(false);
            airContent.SetActive(false);
            navalContent.SetActive(false);
            missileContent.SetActive(true);
            
            missileLine.SetActive(true);

            foreach (GameObject GO in missileWeaponResearchList)
            {
                WeaponTemplate weapon = GO.GetComponent<WeaponResearchItem>().GetWeaponTemplate();

                UpdateResearch(weapon, GO);
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

            HideAllLines();

            selectedResearchPanel.SetActive(false);
        }

        public void UpdateSelectedWeapon(WeaponResearchItem weaponResearch)
        {
            if (weaponResearch == null)
                return;

            WeaponTemplate selectedWeapon = weaponResearch.GetWeaponTemplate();

            if (selectedWeapon == null)
                return;

            selectedResearchPanel.SetActive(true);

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            float researchSpeedRotio = myCountry.Research_Speed;
            weaponName.text = selectedWeapon.weaponName;
            researchCost.text = selectedWeapon.weaponResearchCost.ToString();
            researchDuring.text = selectedWeapon.weaponResearchTime.ToString();
            universityNumber.text = CountryManager.Instance.GetTotalBuildings(myCountry, BUILDING_TYPE.UNIVERSITY).ToString();
            researchSpeed.text = myCountry.Research_Speed.ToString();

            int weaponResearchTime = (int)(selectedWeapon.weaponResearchTime - ( (selectedWeapon.weaponResearchTime * researchSpeedRotio) / 100 ) );
            if (weaponResearchTime <= 1)
                weaponResearchTime = 1;

            totalResearchSpeed.text = weaponResearchTime.ToString();

            selectedResearchPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(selectedWeapon.weaponID);
            selectedResearchPanel.transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>().gameObject.SetActive(true);

            if (myCountry.IsWeaponProducible(selectedWeapon.weaponID) == false) // araştırabilirsin
            {
                if(weaponResearch.GetResearch() == null)
                {
                    cancelResearchButton.SetActive(false);
                    researchButton.SetActive(true);
                    researchSlider.SetActive(false);

                    researchButton.GetComponent<Button>().onClick.AddListener(() => weaponResearch.StartReserach());
                    cancelResearchButton.GetComponent<Button>().onClick.AddListener(() => weaponResearch.CancelResearch());
                }
                else
                {
                    if (weaponResearch.GetResearch().IsResearching())
                    {
                        cancelResearchButton.SetActive(true);
                        researchButton.SetActive(false);
                        researchSlider.SetActive(true);
                    }
                }

            }
            else
            {
                cancelResearchButton.SetActive(false);
                researchButton.SetActive(true);
                researchSlider.SetActive(false);
            }
        }

        public void ShowResearchProgress()
        {
            if (selectedResearch == null)
            {
                return;
            }

            if (researchPanel.activeSelf == true)
            {
                if (selectedResearch.IsCompleted() == false)
                {
                    researchSlider.GetComponent<Slider>().value = selectedResearch.GetProgress();
                }
                else
                {
                    researchSlider.SetActive(false);
                }
            }          
        }
        
        void ClearUnits()
        {
            foreach (Transform child in landContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in airContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in navalContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in missileContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}