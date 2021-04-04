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

        public void Start()
        {
            instance = this;
        }

        public void Init()
        {

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
            string policyToolTip = "\n" + " ";

            policyToolTip = policy.policyName + "\n" +
                "Policy Bonus : " + policy.policyBonus + "\n" +
                "Policy Bonus Value : %" + policy.policyBonusValue.ToString() + "\n" +
            "Required Defense Budget : " + " $ " + string.Format("{0:#,0}", float.Parse(policy.requiredDefenseBudget.ToString())) + "M" + "\n" +
            "Trade Bonus : " + " $ " + string.Format("{0:#,0}", float.Parse(policy.tradeBonus.ToString())) + "M" + "\n" +
            "Permenant Cost : " + " $ " + string.Format("{0:#,0}", float.Parse(policy.costPermenant.ToString())) + "M" + "\n" + " ";

            //temp.gameObject.transform.GetChild(3).GetComponent<RawImage>().texture = weapon.weaponIcon;

            if (IsAcceptable(policy) == false)
                policyToolTip = policyToolTip + "\n" + ColorString("Not Enough Money", Color.red) + "\n";

            temp.gameObject.GetComponent<SimpleTooltip>().infoLeft = policyToolTip;
        }

        public string ColorString(string text, Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
        }

        void ApplyLaw(GameObject GO, Policy policy)
        {
            if (IsAcceptable(policy) == false)
                return;

            int leftDefenseBudget = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().Defense_Budget - policy.requiredDefenseBudget;
            long leftBudget = GameEventHandler.Instance.GetPlayer().GetMyCountry().Budget - policy.costPermenant;

            GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().Defense_Budget = leftDefenseBudget;
            GameEventHandler.Instance.GetPlayer().GetMyCountry().Budget = leftBudget;

            GameEventHandler.Instance.GetPlayer().GetMyCountry().AddPolicy(policy);

            GO.transform.GetChild(1).gameObject.SetActive(false);
            GO.transform.GetChild(2).gameObject.SetActive(true);

            GO.transform.GetChild(3).gameObject.SetActive(true);
            GO.transform.GetChild(4).gameObject.SetActive(false); // alpha value is 255
        }

        bool IsAcceptable(Policy policy)
        {
            if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetArmy().Defense_Budget < policy.requiredDefenseBudget)
            {
                //Debug.Log("Defense Budget is not enough");
                return false;
            }

            if (GameEventHandler.Instance.GetPlayer().GetMyCountry().Budget < policy.costPermenant)
            {
                //Debug.Log("Budget is not enough");
                return false;
            }

            if (GameEventHandler.Instance.GetPlayer().GetMyCountry().GetAcceptedPolicyList().Contains(policy) == true)
            {
                //Debug.Log("You already have it");
                return false;
            }

            return true;
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
    }
}