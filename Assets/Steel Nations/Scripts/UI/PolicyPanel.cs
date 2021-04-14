using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

namespace WorldMapStrategyKit
{
    public class PolicyPanel : MonoBehaviour
    {
        private static PolicyPanel instance;
        public static PolicyPanel Instance
        {
            get { return instance; }
        }

        List<Policy> policyList = new List<Policy>();

        public GameObject policyPanel;
        public GameObject policyItem;
        public GameObject policyVerticalContent;
        public GameObject PolicyContent;
        public GameObject policyCategoryTitle;

        List<Trait> traitList = new List<Trait>();

        public void Start()
        {
            instance = this;
        }

        public void AddPolicy(Policy policy)
        {
            policyList.Add(policy);
        }
        void AddPolicyItem(Policy policy)
        {
            GameObject temp = null;
            GameObject content = null;

            foreach (Transform eachChild in PolicyContent.transform)
            {
                if (eachChild.name == policy.policyName)
                {
                    content = eachChild.gameObject;
                    break;
                }
            }

            bool isFirstItem = false;

            if (content == null)
            {
                content = Instantiate(policyVerticalContent, PolicyContent.transform);
                content.name = policy.policyName;

                GameObject categoryText = Instantiate(policyCategoryTitle, content.transform);
                categoryText.GetComponent<TextMeshProUGUI>().text = policy.policyName;

                isFirstItem = true;
            }

            temp = Instantiate(policyItem, content.transform);

            if(isFirstItem)
            {
                temp.transform.GetChild(3).gameObject.SetActive(false);
                temp.transform.GetChild(4).gameObject.SetActive(false);
            }

            if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAcceptedPolicyList().Contains(policy) == false)
            {
                temp.transform.GetChild(1).gameObject.SetActive(true);
                temp.transform.GetChild(2).gameObject.SetActive(false);

                temp.transform.GetChild(3).gameObject.SetActive(false);
                temp.transform.GetChild(4).gameObject.SetActive(true); // alpha value is low
            }
            else
            {
                temp.transform.GetChild(1).gameObject.SetActive(false);
                temp.transform.GetChild(2).gameObject.SetActive(true);

                temp.transform.GetChild(3).gameObject.SetActive(true);
                temp.transform.GetChild(4).gameObject.SetActive(false); // alpha value is 255
            }

            if (temp != null && policy != null)
            {
                temp.GetComponent<Button>().onClick.AddListener(() => ApplyLaw(temp, policy));

                EventTrigger trigger = temp.GetComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { UpdateTooltip(temp, policy); });
                trigger.triggers.Add(entry);

                UpdateTooltip(temp, policy);
            }
        }

        void UpdateTooltip(GameObject temp, Policy policy)
        {
            string policyToolTip = policy.policyName + "\n" + "\n";

            int requiredDefenseBudget = policy.requiredDefenseBudget;
            int requiredGSYH = policy.requiredGSYH;
            int costPermenant = policy.costPermenant;
            

            if (requiredDefenseBudget > 0)
                policyToolTip += "Required Defense Budget : " + " $ " + string.Format("{0:#,0}", float.Parse(requiredDefenseBudget.ToString())) + "M" + "\n";

            if(requiredGSYH > 0)
                policyToolTip += "Required GDP : " + " $ " + string.Format("{0:#,0}", float.Parse(requiredGSYH.ToString())) + "M" + "\n";

            if(costPermenant > 0)
                policyToolTip += "Cost : " + " $ " + string.Format("{0:#,0}", float.Parse(costPermenant.ToString())) + "M" + "\n";

            foreach(var traitEnum in policy.GetTraits())
            {
                Trait trait = GetTraitByTraitType(traitEnum.Key);

                policyToolTip += trait.Trait_Name + " : " + traitEnum.Value + "%" + "\n";
            }

            //temp.gameObject.transform.GetChild(3).GetComponent<RawImage>().texture = weapon.weaponIcon;

            if (CountryManager.Instance.IsPolicyAcceptable(GameEventHandler.Instance.GetPlayer().GetMyCountry(), policy) == false)
                policyToolTip = policyToolTip + "\n" + ColorString("Not Enough Money", Color.red) + "\n";

            temp.gameObject.GetComponent<SimpleTooltip>().infoLeft = policyToolTip + "\n";
        }

        public string ColorString(string text, Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
        }

        void ApplyLaw(GameObject GO, Policy policy)
        {
            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            if (CountryManager.Instance.IsPolicyAcceptable(myCountry, policy) == false)
                return;

            CountryManager.Instance.AcceptPolicy(myCountry, policy);

            GO.transform.GetChild(1).gameObject.SetActive(false);
            GO.transform.GetChild(2).gameObject.SetActive(true);

            GO.transform.GetChild(3).gameObject.SetActive(true);
            GO.transform.GetChild(4).gameObject.SetActive(false); // alpha value is 255
        }

        public void ShowConstruction()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.CONSTRUCTION)
                {
                    AddPolicyItem(policy);
                }
            }
        }

        public void ShowEconomy()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.ECONOMY)
                {
                    AddPolicyItem(policy);
                }
            }
        }
        public void ShowHealth()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.HEALTH)
                {
                    AddPolicyItem(policy);
                }
            }
        }
        public void ShowReligion()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.RELIGION)
                {
                    AddPolicyItem(policy);
                }
            }
        }
        public void ShowPopulation()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.POPULATION)
                {
                    AddPolicyItem(policy);
                }
            }
        }
        public void ShowMilitary()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.MILITARY)
                {
                    AddPolicyItem(policy);
                }
            }
        }
        public void ShowEducation()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.EDUCATION)
                {
                    AddPolicyItem(policy);
                }
            }
        }
        public void ShowDiplomacy()
        {
            ClearAllContents();

            foreach (Policy policy in policyList)
            {
                if (policy.policyType == POLICY_TYPE.DIPLOMACY)
                {
                    AddPolicyItem(policy);
                }
            }
        }

        public void HidePolicyPanel()
        {
            ClearAllContents();
            policyPanel.SetActive(false);
        }

        public void ShowPolicyPanel()
        {
            ClearAllContents();
            policyPanel.SetActive(true);
        }

        void ClearAllContents()
        {
            foreach (Transform child in PolicyContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public TRAIT GetTraitTypeByName(string traitName)
        {
            if (traitName == "War Support")
                return TRAIT.WAR_SUPPORT;

            if (traitName == "Political Power Gain")
                return TRAIT.POLITICAL_POWER_GAIN;

            if (traitName == "Army Morale")
                return TRAIT.ARMY_MORALE;

            if (traitName == "Experience Gain Air Factor")
                return TRAIT.EXPERIENCE_GAIN_AIR_FACTOR;

            if (traitName == "Construction Speed")
                return TRAIT.CONSTRUCTION_SPEED;

            if (traitName == "Oil Production Speed")
                return TRAIT.OIL_PRODUCTION_SPEED;

            if (traitName == "Iron Production Speed")
                return TRAIT.IRON_PRODUCTION_SPEED;

            if (traitName == "Steel Production Speed")
                return TRAIT.STEEL_PRODUCTION_SPEED;

            if (traitName == "Uranium Production Speed")
                return TRAIT.URANIUM_PRODUCTION_SPEED;

            if (traitName == "Aluminium Production Speed")
                return TRAIT.ALUMINIUM_PRODUCTION_SPEED;

            if (traitName == "Air Vehicle Production Speed")
                return TRAIT.AIR_PRODUCTION_SPEED;

            if (traitName == "Land Vehicle Production Speed")
                return TRAIT.LAND_PRODUCTION_SPEED;

            if (traitName == "Naval Vehicle Production Speed")
                return TRAIT.NAVAL_PRODUCTION_SPEED;

            if (traitName == "Research Speed")
                return TRAIT.RESEARCH_SPEED;

            if (traitName == "Birth Rate")
                return TRAIT.BIRTH_RATE;

            if (traitName == "Pandemic Dead Rate")
                return TRAIT.PANDEMIC_DEAD_RATE;

            if (traitName == "Tension")
                return TRAIT.TENSION;

            if (traitName == "Unemployment Rate")
                return TRAIT.UNEMPLOYMENT_RATE;

            return TRAIT.NONE;
        }

        public void AddTrait(Trait trait)
        {
            traitList.Add(trait);
        }

        public Trait GetTraitByTraitType(TRAIT traitType)
        {
            foreach (Trait trait in traitList)
                if (trait.Trait_Type == traitType)
                    return trait;

            return null;
        }

        public Trait GetTraitByTraitName(string traitName)
        {
            foreach (Trait trait in traitList)
            {
                if (trait.Trait_Name == traitName)
                {
                    return trait;
                }
            }
            return null;
        }
    }
}