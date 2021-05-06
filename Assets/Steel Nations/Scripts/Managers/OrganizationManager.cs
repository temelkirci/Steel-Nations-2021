using UnityEngine;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class OrganizationManager : Singleton<OrganizationManager>
    {
        List<Organization> organizationList = new List<Organization>();

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

            organization.oilBonus = 0;
            organization.miningBonus = 0;
            organization.gunBonus = 0;
            organization.organizationDescription = description;
            organization.isTerroristOrganization = (isTerroristOrganization == 0) ? false : true;
            organization.isAttackForMember = (isAttackToProtect == 0) ? false : true;

            organization.organizationLogo = ResourceManager.Instance.LoadTexture(RESOURCE_TYPE.ORGANIZATION, logoPath);

            string[] founderList = founder.Split('~');

            string[] fullMemberList = fullMember.Split('~');
            string[] observerList = observer.Split('~');
            string[] dialoguePartnerList = dialoguePartner.Split('~');

            string[] appliedAsFullMemberList = appliedForFullMember.Split('~');
            string[] appliedAsObserverList = appliedForObservor.Split('~');
            string[] appliedAsDialoguePartnerList = appliedForDialoguePartner.Split('~');

            foreach(string countryName in founderList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                    {
                        organization.AddFounder(tempCountry, false);
                    }
                }
            }

            foreach (string countryName in fullMemberList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                    {
                        organization.AddFullMember(tempCountry, false);
                    }
                }
            }

            foreach (string countryName in observerList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                        organization.AddObserver(tempCountry, false);
                }
            }

            foreach (string countryName in dialoguePartnerList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                        organization.AddDialoguePartner(tempCountry, false);
                }
            }

            foreach (string countryName in appliedAsFullMemberList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                        organization.ApplyForFullMember(tempCountry, false);
                }
            }

            foreach (string countryName in appliedAsObserverList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                        organization.ApplyForObserver(tempCountry, false);
                }
            }

            foreach (string countryName in appliedAsDialoguePartnerList)
            {
                if (countryName != string.Empty)
                {
                    Country tempCountry = CountryManager.Instance.GetCountryByName(countryName);

                    if (tempCountry != null)
                        organization.ApplyForDialoguePartner(tempCountry, false);
                }
            }

            organizationList.Add(organization);
        }

        public List<Organization> GetAllOrganizations()
        {
            return organizationList;
        }

        public int GetOrganizationsPower(Country country)
        {
            int power = 0;

            foreach (Organization organization in GetAllOrganizations())
                if (organization.isAttackForMember && organization.isFullMemberCountry(country) == false)
                    foreach (Country member in organization.GetFullMemberList())
                        if (member.GetArmy() != null)
                            power += member.GetArmy().GetArmyPower();

            return power;
        }

        public void DeclareWarToDefense(Country country, Country target)
        {
            if (country == target)
                return;

            foreach (Organization organization in GetAllOrganizations())
            {
                if (organization.isFullMemberCountry(target) && organization.isAttackForMember)
                {
                    foreach(Country member in organization.GetFullMemberList())
                    {
                        if(country != target && member != country && member != target && CountryManager.Instance.GetAtWarCountryList(member).Contains(country) == false)
                        {
                            ActionManager.Instance.CreateAction(
                            member,
                            country,
                            ACTION_TYPE.Declare_War,
                            MINERAL_TYPE.NONE,
                            0,
                            null,
                            0,
                            null,
                            null,
                            0,
                            0,
                            0);

                            return;
                        }
                    }
                }
            }
        }

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