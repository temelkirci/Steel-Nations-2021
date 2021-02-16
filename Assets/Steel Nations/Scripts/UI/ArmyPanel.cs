using UnityEngine;
using TMPro;

namespace WorldMapStrategyKit
{
    public class ArmyPanel : MonoBehaviour
    {
        private static ArmyPanel instance;
        public static ArmyPanel Instance
        {
            get { return instance; }
        }

        public GameObject armyPanel;
        public TextMeshProUGUI militaryPersonnel;
        public TextMeshProUGUI defenseBudget;
        public TextMeshProUGUI militaryRank;
        public TextMeshProUGUI dockyard;
        public TextMeshProUGUI militaryFactory;

        public GameObject starSprite;
        public GameObject weaponTechItem;
        public GameObject weaponTechContent;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void ShowArmyPanel()
        {
            armyPanel.SetActive(true);

            ClearWeaponTechContent();

            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            militaryPersonnel.text = myCountry.GetArmy().GetSoldierNumber().ToString();

            if( myCountry.GetArmy() != null )
                defenseBudget.text = "$ " + string.Format("{0:#,0}", float.Parse(myCountry.GetArmy().GetDefenseBudget().ToString())) + " M";

            militaryRank.text = myCountry.GetMilitaryRank().ToString();
            militaryFactory.text = myCountry.GetTotalBuildings(BUILDING_TYPE.MILITARY_FACTORY).ToString();
            dockyard.text = myCountry.GetTotalBuildings(BUILDING_TYPE.DOCKYARD).ToString();

            // weapon tech

            CreateWeaponButton("Tank", myCountry.attrib["Tank Tech"]);
            CreateWeaponButton("Armored Vehicle", myCountry.attrib["Armored Vehicle Tech"]);
            CreateWeaponButton("Self-Propelled Artillery", myCountry.attrib["Self-Propelled Artillery Tech"]);
            CreateWeaponButton("Towed Artillery", myCountry.attrib["Towed Artillery Tech"]);
            CreateWeaponButton("Rocket Projector", myCountry.attrib["Rocket Projector Tech"]);

            CreateWeaponButton("Helicopter", myCountry.attrib["Helicopter Tech"]);
            CreateWeaponButton("Fighter", myCountry.attrib["Fighter Tech"]);
            CreateWeaponButton("Drone", myCountry.attrib["Drone Tech"]);

            CreateWeaponButton("Frigate", myCountry.attrib["Frigate Tech"]);
            CreateWeaponButton("Corvette", myCountry.attrib["Corvette Tech"]);
            CreateWeaponButton("Coastal Patrol", myCountry.attrib["Coastal Patrol Tech"]);
            CreateWeaponButton("Destroyer", myCountry.attrib["Destroyer Tech"]);
            CreateWeaponButton("Aircraft Carrier", myCountry.attrib["Aircraft Carrier Tech"]);
            CreateWeaponButton("Submarine", myCountry.attrib["Submarine Tech"]);

            CreateWeaponButton("Missile", myCountry.attrib["Missile Tech"]);
        }

        void CreateWeaponButton(string text, int tech)
        {
            GameObject GO = Instantiate(weaponTechItem, weaponTechContent.transform);
            GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;

            for(int i=0; i<tech; i++)
                Instantiate(starSprite, GO.transform.GetChild(4).transform);
        }

        public void HidePanel()
        {
            armyPanel.SetActive(false);

            ClearWeaponTechContent();
        }

        void ClearWeaponTechContent()
        {
            foreach (Transform eachChild in weaponTechContent.transform)
            {
                Destroy(eachChild.gameObject);
            }
        }
    }
}