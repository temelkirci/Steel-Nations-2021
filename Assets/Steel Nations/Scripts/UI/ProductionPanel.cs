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

        public GameObject weaponItem;

        public GameObject landContent;
        public GameObject airContent;
        public GameObject missileContent;

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

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void Init()
        {

        }

        public void HidePanel()
        {
            productionPanel.SetActive(false);

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
 
                
                if(weaponTemplate != null )
                {
                    productionPanel.SetActive(true);

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
                    temp.GetComponent<WeaponProductionItem>().SetWeapon(weaponTemplate);

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

                    for(int index=0; index < weaponTemplate.weaponLevel; index++)
                    {
                        Instantiate(starSprite, temp.GetComponent<WeaponProductionItem>().generation.transform);
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
                foreach (Transform go in landContent.transform)
                {
                    go.GetComponent<WeaponProductionItem>().UpdateProduction();
                }
                foreach (Transform go in airContent.transform)
                {
                    go.GetComponent<WeaponProductionItem>().UpdateProduction();
                }
                foreach (Transform go in missileContent.transform)
                {
                    go.GetComponent<WeaponProductionItem>().UpdateProduction();
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
        }
    }
}
