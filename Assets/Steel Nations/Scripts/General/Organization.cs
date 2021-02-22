using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace WorldMapStrategyKit
{
    public class Organization
    {
        public string organizationName;
        public string organizationDescription;

        public bool isActive;
        public bool isTrade;
        public bool isMilitary;

        public float tradeBonusPerWeek;
        public float miningBonus;
        public float oilBonus;
        public float gunBonus;

        public bool isTerroristOrganization;
        public bool isAttackForMember;

        List<Country> FoundersList = new List<Country>();

        List<Country> FullMemberList = new List<Country>();
        List<Country> ObserverList = new List<Country>();
        List<Country> DialoguePartnerList = new List<Country>();

        List<Country> AppliedForFullMemberList = new List<Country>();
        List<Country> AppliedForObserverList = new List<Country>();
        List<Country> AppliedForDialoguePartnerList = new List<Country>();

        List<string> TradeEmbargoList = new List<string>();
        List<string> MiningEmbargoList = new List<string>();
        List<string> GunEmbargoList = new List<string>();
        List<string> OilEmbargoList = new List<string>();

        public Texture2D organizationLogo;

        public Organization()
        {
            organizationName = string.Empty;
            organizationDescription = string.Empty;

            isActive = true;

            tradeBonusPerWeek = 0;
            miningBonus = 0;
            oilBonus = 0;
            gunBonus = 0;

            isAttackForMember = false;
            isTerroristOrganization = false;

            FoundersList.Clear();

            FullMemberList.Clear();
            ObserverList.Clear();
            DialoguePartnerList.Clear();

            AppliedForFullMemberList.Clear();
            AppliedForObserverList.Clear();
            AppliedForDialoguePartnerList.Clear();

            TradeEmbargoList.Clear();
            MiningEmbargoList.Clear();
            GunEmbargoList.Clear();
            OilEmbargoList.Clear();
        }

        public void AddFounder(Country country, bool showNotification)
        {
            FoundersList.Add(country);
        }

        public void AddFullMember(Country country, bool showNotification)
        {
            FullMemberList.Add(country);

            if (showNotification)
                NotificationManager.Instance.CreateNotification(country.name + " join to " + organizationName + " as full member");
        }

        public void AddObserver(Country country, bool showNotification)
        {
            ObserverList.Add(country);

            if (showNotification)
                NotificationManager.Instance.CreateNotification(country.name + " join to " + organizationName + " as observer");
        }

        public void AddDialoguePartner(Country country, bool showNotification)
        {
            DialoguePartnerList.Add(country);

            if (showNotification)
                NotificationManager.Instance.CreateNotification(country.name + " join to " + organizationName + " as dialogue partner");
        }

        public void ApplyForFullMember(Country country, bool showNotification)
        {
            AppliedForFullMemberList.Add(country);

            if (showNotification)
                NotificationManager.Instance.CreateNotification(country.name + " made a full membership application to " + organizationName);
        }

        public void ApplyForObserver(Country country, bool showNotification)
        {
            AppliedForObserverList.Add(country);

            if (showNotification)
                NotificationManager.Instance.CreateNotification(country.name + " applied to " + organizationName + " as observer");
        }

        public void ApplyForDialoguePartner(Country country, bool showNotification)
        {
            AppliedForDialoguePartnerList.Add(country);

            if (showNotification)
                NotificationManager.Instance.CreateNotification(country.name + " applied to " + organizationName + " as dialogue partner");
        }

        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                organizationName = value;
            }
        }

        public bool IsTerroristOrganization
        {
            get
            {
                return isTerroristOrganization;
            }
            set
            {
                if(value == true)
                {
                    isAttackForMember = false;
                }

                isTerroristOrganization = value;
            }
        }

        public bool isFullMemberCountry(Country country)
        {
            return FullMemberList.Contains(country);
        }

        public List<Country> GetFounderList()
        {
            return FoundersList;
        }

        public List<Country> GetFullMemberList()
        {
            return FullMemberList;
        }

        public List<Country> GetObserverList()
        {
            return ObserverList;
        }

        public List<Country> GetDialoguePartnerList()
        {
            return DialoguePartnerList;
        }

        public List<Country> GetApplyForFullMemberList()
        {
            return AppliedForFullMemberList;
        }

        public List<Country> GetApplyForObserverList()
        {
            return AppliedForObserverList;
        }

        public List<Country> GetApplyForDialoguePartnerList()
        {
            return AppliedForDialoguePartnerList;
        }

        public void ResultForApply()
        {
            ResultForFullMember();
            ResultForObserver();
            ResultForDialoguePartner();
        }

        void ResultForFullMember()
        {
            if (AppliedForFullMemberList.Count == 0)
                return;

            Country fullMemberCountry = AppliedForFullMemberList[0];
            int fullMemberCountryAcceptChance = GetRelationshipInOrganiation(fullMemberCountry);

            if (fullMemberCountryAcceptChance >= 50)
                AddFullMember(fullMemberCountry, true);
            else
                NotificationManager.Instance.CreateNotification(fullMemberCountry.name + " has been rejected by " + organizationName);

            AppliedForFullMemberList.Remove(fullMemberCountry);
        }

        void ResultForObserver()
        {
            if (AppliedForObserverList.Count == 0)
                return;

            Country country = AppliedForObserverList[0];
            int acceptChance = GetRelationshipInOrganiation(country);

            if (acceptChance >= 25)
                AddObserver(country, true);
            else
                NotificationManager.Instance.CreateNotification(country.name + "  has been rejected by " + organizationName);

            AppliedForObserverList.Remove(country);
        }

        void ResultForDialoguePartner()
        {
            if (AppliedForDialoguePartnerList.Count == 0)
                return;

            Country country = AppliedForDialoguePartnerList[0];
            int acceptChance = GetRelationshipInOrganiation(country);

            if (acceptChance >= 0)
                AddDialoguePartner(country, true);
            else
                NotificationManager.Instance.CreateNotification(country.name + " has been rejected by " + organizationName);

            AppliedForDialoguePartnerList.Remove(country);
        }

        int GetRelationshipInOrganiation(Country country)
        {
            int founder = 0;
            int member = 0;
            int observer = 0;
            int dialogue = 0;

            int total = 0;

            foreach (Country temp in FoundersList)
                founder += (temp.attrib[country.name]);

            foreach (Country temp in FullMemberList)
                member += (temp.attrib[country.name] / 2);

            foreach (Country temp in ObserverList)
                observer += (temp.attrib[country.name] / 3);

            foreach (Country temp in DialoguePartnerList)
                dialogue += (temp.attrib[country.name] / 4);

            total = founder + member + observer + dialogue;

            return total;
        }

    }
}