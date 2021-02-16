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

        public void AddFounder(Country country)
        {
            FoundersList.Add(country);
        }

        public void AddFullMember(Country country)
        {
            FullMemberList.Add(country);
        }

        public void AddObserver(Country country)
        {
            ObserverList.Add(country);
        }

        public void AddDialoguePartner(Country country)
        {
            DialoguePartnerList.Add(country);
        }

        public void ApplyForFullMember(Country country)
        {
            AppliedForFullMemberList.Add(country);
        }

        public void ApplyForObserver(Country country)
        {
            AppliedForObserverList.Add(country);
        }

        public void ApplyForDialoguePartner(Country country)
        {
            AppliedForDialoguePartnerList.Add(country);
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
            foreach (Country country in AppliedForFullMemberList)
            {
                int acceptChance = 100;
                /*
                foreach (Country member in FullMemberList)
                {
                    if (member.enemyList.Contains(country))
                    {
                        acceptChance = acceptChance - 10;
                    }
                    if (member.allyList.Contains(country))
                    {
                        acceptChance = acceptChance + 10;
                    }
                }

                foreach (Country member in ObserverList)
                {
                    if (member.enemyList.Contains(country))
                    {
                        acceptChance = acceptChance - 5;
                    }
                    if (member.allyList.Contains(country))
                    {
                        acceptChance = acceptChance + 5;
                    }
                }

                foreach (Country member in DialoguePartnerList)
                {
                    if (member.enemyList.Contains(country))
                    {
                        acceptChance = acceptChance - 1;
                    }
                    if (member.allyList.Contains(country))
                    {
                        acceptChance = acceptChance + 1;
                    }
                }
                */
                //Debug.Log(country.name + " -> Accept Chance : " + acceptChance);

            }
        }
    }
}