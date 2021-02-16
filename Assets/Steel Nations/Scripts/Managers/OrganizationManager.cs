using UnityEngine;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class OrganizationManager : Singleton<OrganizationManager>
    {
        List<Organization> organizationList = new List<Organization>();

        // Start is called before the first frame update
        public void Start()
        {
        }

        public void CreateOrganization(string organizationName,
            int isActive,
            int isTradeBonus,
            int isMilitaryBonus,
            int isTerroristOrganization,
            int isAttackToProtect,
            string description,
            string logoPath,
            string founder,
            string fullMember,
            string observer,
            string dialoguePartner,
            string appliedForFullMember,
            string appliedForObservor,
            string appliedForDialoguePartner)
        {
            Organization organization = new Organization();
            organization.organizationName = organizationName;
            organization.isActive = (isActive == 0) ? false : true;
            organization.isTrade = (isTradeBonus == 0) ? false : true;
            organization.isMilitary = (isMilitaryBonus == 0) ? false : true;

            organization.tradeBonusPerWeek = 0;
            organization.oilBonus = 0;
            organization.miningBonus = 0;
            organization.gunBonus = 0;
            organization.organizationDescription = description;
            organization.isTerroristOrganization = (isTerroristOrganization == 0) ? false : true;
            organization.isAttackForMember = (isAttackToProtect == 0) ? false : true;

            organization.organizationLogo = Resources.Load("organizations/" + logoPath) as Texture2D;

            string[] founderList = founder.Split('~');

            string[] fullMemberList = fullMember.Split('~');
            string[] observerList = observer.Split('~');
            string[] dialoguePartnerList = dialoguePartner.Split('~');

            string[] appliedAsFullMemberList = appliedForFullMember.Split('~');
            string[] appliedAsObserverList = appliedForObservor.Split('~');
            string[] appliedAsDialoguePartnerList = appliedForDialoguePartner.Split('~');

            foreach (string countryName in fullMemberList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = GameEventHandler.Instance.GetCountryByName(countryName);

                    organization.AddFullMember(tempCountry);

                    if (organization.isTrade == true)
                        organization.tradeBonusPerWeek = organization.tradeBonusPerWeek + tempCountry.attrib["Trade Ratio"];

                }
            }

            foreach (string countryName in observerList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = GameEventHandler.Instance.GetCountryByName(countryName);
                    organization.AddObserver(tempCountry);
                }
            }

            foreach (string countryName in dialoguePartnerList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = GameEventHandler.Instance.GetCountryByName(countryName);
                    organization.AddDialoguePartner(tempCountry);
                }
            }

            foreach (string countryName in appliedAsFullMemberList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = GameEventHandler.Instance.GetCountryByName(countryName);
                    organization.ApplyForFullMember(tempCountry);
                }
            }
            foreach (string countryName in appliedAsObserverList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = GameEventHandler.Instance.GetCountryByName(countryName);
                    organization.ApplyForObserver(tempCountry);
                }
            }
            foreach (string countryName in appliedAsDialoguePartnerList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = GameEventHandler.Instance.GetCountryByName(countryName);
                    organization.ApplyForDialoguePartner(tempCountry);
                }
            }
            //Debug.Log(organization.organizationName + " = " + organization.tradeBonusPerWeek);
            organizationList.Add(organization);
        }

        public List<Organization> GetAllOrganizations()
        {
            return organizationList;
        }

        // Update is called once per frame
        public Texture2D GetOrganizationLogoByName(string organizationName)
        {
            foreach (Organization org in organizationList)
            {
                if (org.organizationName == organizationName)
                    return org.organizationLogo;
            }
            return null;
        }

        public Organization GetOrganizationByName(string organizationName)
        {
            foreach (Organization org in organizationList)
            {
                if (org.organizationName == organizationName)
                    return org;
            }
            return null;
        }
    }
}