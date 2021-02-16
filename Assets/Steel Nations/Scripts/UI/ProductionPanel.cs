using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace WorldMapStrategyKit
{
    public class ProductionPanel : MonoBehaviour
    {
        private static ProductionPanel instance;
        public static ProductionPanel Instance
        {
            get { return instance; }
        }

        public GameObject productionPanel;
        public GameObject dockyardPanel;

        public GameObject weaponItem;

        public GameObject landContent;
        public GameObject airContent;
        public GameObject missileContent;
        public GameObject navalContent;

        public GameObject weaponInformationPanel;

        public RawImage weaponImage;
        public TextMeshProUGUI windowTitle;

        public TextMeshProUGUI weaponName;
        public TextMeshProUGUI weaponDescription;

        public TextMeshProUGUI weaponLevel;
        public TextMeshProUGUI weaponSpeed;
        public TextMeshProUGUI weaponCost;
        public TextMeshProUGUI weaponAttackPoint;
        public TextMeshProUGUI weaponDefense;
        public TextMeshProUGUI weaponAttackRange;
        public TextMeshProUGUI weaponProductionTime;
        public TextMeshProUGUI weaponTechYear;

        public GameObject starSprite;

        bool isOpenDockyard = false;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void OpenDockyard()
        {
            isOpenDockyard = true;

            ShowProductionPanel();           
        }

        public void OpenMilitaryFactory()
        {
            isOpenDockyard = false;

            ShowProductionPanel();
        }

        public void HidePanel()
        {
            productionPanel.SetActive(false);
            dockyardPanel.SetActive(false);

            GameEventHandler.Instance.GetPlayer().SetSelectedBuilding(null);
        }

        public void ShowProductionPanel()
        {
            ClearUnits();

            GameEventHandler.Instance.GetPlayer().SetSelectedBuilding(null);

            foreach (int i in GameEventHandler.Instance.GetPlayer().GetMyCountry().GetProducibleWeapons())
            {
                WeaponTemplate weaponTemplate = WeaponManager.Instance.GetWeaponTemplateByID(i);
                GameObject temp = null;
                Debug.Log(weaponTemplate.weaponName);
                if (weaponTemplate == null)
                    return;

                
                if( isOpenDockyard == true )
                {
                    dockyardPanel.SetActive(true);
                    productionPanel.SetActive(false);

                    if (weaponTemplate.weaponTerrainType == 2)
                    {
                        temp = Instantiate(weaponItem, navalContent.transform);
                    }
                }
                else
                {
                    productionPanel.SetActive(true);
                    dockyardPanel.SetActive(false);

                    if (weaponTemplate.weaponTerrainType == 1)
                    {
                        temp = Instantiate(weaponItem, landContent.transform);
                    }
                    if (weaponTemplate.weaponTerrainType == 3)
                    {
                        temp = Instantiate(weaponItem, airContent.transform);
                    }
                    if (weaponTemplate.weaponTerrainType == 4)
                    {
                        temp = Instantiate(weaponItem, missileContent.transform);
                    }
                }


                if(temp != null)
                {
                    int researchSpeedRotio = 10;// GameEventHandler.Instance.GetPlayer().GetMyCountry().GetProductionSpeed();

                    int weaponProductionTime = (weaponTemplate.weaponProductionTime - ( (weaponTemplate.weaponProductionTime * researchSpeedRotio ) / 100 ) );
                    if (weaponProductionTime <= 1)
                        weaponProductionTime = 1;

                    temp.transform.GetChild(1).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = weaponTemplate.weaponName;

                    EventTrigger trigger = temp.transform.GetChild(0).transform.GetChild(0).GetComponent<EventTrigger>();

                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerEnter;
                    entry.callback.AddListener((data) => { ShowWeaponInfo(weaponTemplate); });
                    trigger.triggers.Add(entry);

                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    entry2.eventID = EventTriggerType.PointerExit;
                    entry2.callback.AddListener((data) => { HideInfoPanel(); });
                    trigger.triggers.Add(entry2);

                    temp.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(i);

                    temp.gameObject.transform.GetChild(1).transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => GameEventHandler.Instance.GetPlayer().GetSelectedCountry().ProductWeapon(weaponTemplate));

                    temp.gameObject.transform.GetChild(1).transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "$ " + string.Format("{0:#,0}", weaponTemplate.weaponCost.ToString()) + " M";
                    temp.gameObject.transform.GetChild(1).transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = weaponProductionTime.ToString() + " day";

                    for(int index=0; index < weaponTemplate.weaponLevel; index++)
                    {
                        Instantiate(starSprite, temp.gameObject.transform.GetChild(1).transform.GetChild(6).transform);
                    }
                }
            }
        }

        
        void HideInfoPanel()
        {
            weaponInformationPanel.SetActive(false);
        }

        void ShowWeaponInfo(WeaponTemplate weaponTemplate)
        {
            weaponInformationPanel.SetActive(true);

            weaponName.text = weaponTemplate.weaponName.ToString();
            weaponDescription.text = weaponTemplate.weaponDescription.ToString();

            weaponImage.GetComponent<RawImage>().texture = WeaponManager.Instance.GetWeaponTemplateIconByID(weaponTemplate.weaponID);

            weaponLevel.text = weaponTemplate.weaponLevel.ToString();
            weaponSpeed.text = weaponTemplate.weaponSpeed.ToString() + " mph";
            weaponCost.text = "$ " + string.Format("{0:#,0}", float.Parse(weaponTemplate.weaponCost.ToString()));
            weaponAttackPoint.text = weaponTemplate.weaponAttack.ToString();
            weaponDefense.text = weaponTemplate.weaponDefense.ToString();
            weaponAttackRange.text = weaponTemplate.weaponAttackRange.ToString() + " km";
            weaponProductionTime.text = weaponTemplate.weaponProductionTime.ToString() + " days";
            weaponTechYear.text = weaponTemplate.weaponResearchYear.ToString();
        }

        public void ShowProductionProgress()
        {
            if (productionPanel.activeSelf == true)
            {
                foreach (Production production in GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAllProductionsInProgress())
                {
                    if (production.techWeapon.weaponTerrainType == 1)
                    {
                        foreach (Transform go in landContent.transform)
                        {
                            Slider slider = go.transform.GetChild(1).transform.GetChild(3).transform.GetChild(2).GetComponent<Slider>();
                            Text progress = go.transform.GetChild(1).transform.GetChild(3).transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
                            Text weaponName = go.gameObject.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>();

                            if ((production.techWeapon.weaponName + " " + production.techWeapon.weaponLevel) == weaponName.text)
                            {
                                if (production.leftDays > 0)
                                {
                                    slider.value = 100f - (production.leftDays * 100f) / production.techWeapon.weaponProductionTime;
                                }
                                else
                                {
                                    slider.value = 100f;
                                }

                                progress.text = slider.value.ToString();
                            }
                        }
                    }

                    else if (production.techWeapon.weaponTerrainType == 3)
                    {
                        foreach (Transform go in airContent.transform)
                        {
                            Slider slider = go.transform.GetChild(1).transform.GetChild(3).transform.GetChild(2).GetComponent<Slider>();
                            Text progress = go.transform.GetChild(1).transform.GetChild(3).transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
                            Text weaponName = go.gameObject.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>();

                            if ((production.techWeapon.weaponName + " " + production.techWeapon.weaponLevel) == weaponName.text)
                            {
                                if (production.leftDays > 0)
                                {
                                    slider.value = 100f - (production.leftDays * 100f) / production.techWeapon.weaponProductionTime;
                                }
                                else
                                {
                                    slider.value = 100f;
                                }

                                progress.text = slider.value.ToString();
                            }
                        }
                    }
                    else if (production.techWeapon.weaponTerrainType == 4)
                    {
                        foreach (Transform go in missileContent.transform)
                        {
                            Slider slider = go.transform.GetChild(1).transform.GetChild(3).transform.GetChild(2).GetComponent<Slider>();
                            Text progress = go.transform.GetChild(1).transform.GetChild(3).transform.GetChild(3).transform.GetChild(0).GetComponent<Text>();
                            Text weaponName = go.gameObject.transform.GetChild(1).transform.GetChild(2).GetComponent<Text>();

                            if ((production.techWeapon.weaponName + " " + production.techWeapon.weaponLevel) == weaponName.text)
                            {
                                if (production.leftDays > 0)
                                {
                                    slider.value = 100f - (production.leftDays * 100f) / production.techWeapon.weaponProductionTime;
                                }
                                else
                                {
                                    slider.value = 100f;
                                }

                                progress.text = slider.value.ToString();
                            }
                        }
                    }
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
            foreach (Transform child in missileContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in navalContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
