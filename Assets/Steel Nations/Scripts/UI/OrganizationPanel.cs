using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit
{
    public class OrganizationPanel : MonoBehaviour
    {
        private static OrganizationPanel instance;
        public static OrganizationPanel Instance
        {
            get { return instance; }
        }

        public GameObject organizationPanel;
        public GameObject organizationContent;
        public GameObject organizationItem;

        public TextMeshProUGUI organizationName;
        public TextMeshProUGUI tradeOrganization;
        public TextMeshProUGUI militaryOrganization;
        public TextMeshProUGUI terroristOrganization;
        public TextMeshProUGUI protectOtherMembers;
        public TextMeshProUGUI tradeValue;

        public GameObject flagLogo;

        public GameObject fullMemberContent;
        public GameObject dialoguePartnerContent;
        public GameObject observationContent;

        public Button applyButton;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void ShowOrganizationPanel()
        {
            organizationPanel.SetActive(true);

            foreach (Transform child in organizationContent.transform)
            {
                Destroy(child.gameObject);
            }


            foreach (Organization organization in OrganizationManager.Instance.GetAllOrganizations())
            {
                if (organization == null)
                    return;

                GameObject temp = Instantiate(organizationItem, organizationContent.transform);

                temp.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = organization.organizationName;
                temp.gameObject.transform.GetChild(1).GetComponent<RawImage>().texture = OrganizationManager.Instance.GetOrganizationLogoByName(organization.organizationName);

                temp.gameObject.GetComponent<Button>().onClick.AddListener(() => ShowOrganizationDetails(organization));

                temp.SetActive(true);
            }

            ShowOrganizationDetails(OrganizationManager.Instance.GetAllOrganizations()[0]);
        }

        public void ShowOrganizationDetails(Organization organization)
        {
            bool member = false;

            organizationName.text = organization.organizationName.ToString();

            foreach (Transform child in fullMemberContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in dialoguePartnerContent.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in observationContent.transform)
            {
                Destroy(child.gameObject);
            }

            tradeOrganization.text = organization.isTrade.ToString();
            militaryOrganization.text = organization.isMilitary.ToString();
            terroristOrganization.text = organization.isTerroristOrganization.ToString();
            protectOtherMembers.text = organization.isAttackForMember.ToString();
            tradeValue.text = organization.GetTradeBonus().ToString();

            foreach(Country country in organization.GetFullMemberList())
            {
                GameObject temp = Instantiate(flagLogo, fullMemberContent.transform);
                temp.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
                temp.GetComponent<SimpleTooltip>().infoLeft = country.name;
            }

            foreach (Country country in organization.GetObserverList())
            {
                GameObject temp = Instantiate(flagLogo, observationContent.transform);
                temp.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
                temp.GetComponent<SimpleTooltip>().infoLeft = country.name;
            }

            foreach (Country country in organization.GetDialoguePartnerList())
            {
                GameObject temp = Instantiate(flagLogo, dialoguePartnerContent.transform);
                temp.gameObject.transform.GetChild(0).GetComponent<RawImage>().texture = country.GetCountryFlag();
                temp.GetComponent<SimpleTooltip>().infoLeft = country.name;
            }

            if (member == false)
            {
                applyButton.onClick.AddListener(() => ApplyForOrganization(GameEventHandler.Instance.GetPlayer().GetMyCountry()));
            }
        }

        public void ApplyForOrganization(Country country)
        {
            Organization org = OrganizationManager.Instance.GetOrganizationByName(organizationName.text);

            org.ApplyForFullMember(country, true);
        }
    }
}