using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using AwesomeCharts;
using System.Collections.Generic;

namespace WorldMapStrategyKit
{
    public class EconomyPanel : MonoBehaviour
    {
        private static EconomyPanel instance;
        public static EconomyPanel Instance
        {
            get { return instance; }
        }

        public GameObject economyPanel;
        public TextMeshProUGUI previousNationalIncome;
        public TextMeshProUGUI currentNationalIncome;
        public TextMeshProUGUI exportPerWeek;
        public TextMeshProUGUI importPerWeek;
        public TextMeshProUGUI taxes;
        public TextMeshProUGUI tradeRatio;

        public TextMeshProUGUI oilIncome;
        public TextMeshProUGUI miningIncome;
        public TextMeshProUGUI gunIncome;
        public TextMeshProUGUI taxesPerWeek;
        //public TextMeshProUGUI factoryIncomePerWeek;

        public Slider taxesSlider;

        public BarChart incomeBar;
        public BarChart expenseBar;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        public void ChangedTaxes()
        {
            int newValue = Convert.ToInt32(taxesSlider.value);

            GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Taxes"] = newValue;

            taxes.text = "$ " + GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Taxes"];
        }

        public void ShowEconomyPanel()
        {
            economyPanel.SetActive(true);

            UpdateEconomyPanel();
        }

        public void UpdateEconomyPanel()
        {
            if (economyPanel.activeSelf == true)
            {
                Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

                previousNationalIncome.text = "$ " + string.Format("{0:#,0}", float.Parse(myCountry.GetPreviousGDP().ToString())) + "M";
                currentNationalIncome.text = "$ " + string.Format("{0:#,0}", float.Parse(myCountry.GetCurrentGDP().ToString())) + "M";
                taxes.text = "$ " + myCountry.GetIndividualTax().ToString();
                exportPerWeek.text = "$ " + string.Format("{0:#,0}", float.Parse(myCountry.GetExportMonthly().ToString())) + "M";
                importPerWeek.text = "$ " + string.Format("{0:#,0}", float.Parse(myCountry.GetImportMonthly().ToString())) + "M";
                tradeRatio.text = "% " + myCountry.GetTradeRatio();

                SetIncomeBarChart();
                SetExpenseBarChart();

                // monthly balance
                //gunIncome.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Gun Income Per Year"])) + "M";
                //miningIncome.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Mining Income Per Year"])) + "M";
                //oilIncome.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Oil Income Per Year"])) + "M";
                //taxesPerWeek.text = "$ " + string.Format("{0:#,0}", float.Parse(GameEventHandler.Instance.GetPlayer().GetMyCountry().attrib["Taxes Per Week"])) + "M";
                //factoryIncomePerWeek.text = "$ " + string.Format("{0:#,0}", float.Parse(tempCountry.attrib["Factory Income Per Week"])) + "M";
            }
        }

        public void SetIncomeBarChart()
        {
            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            incomeBar.GetChartData().DataSets.Clear();

            //	Create	data	set	for	entries
            BarDataSet set = new BarDataSet();
            //	Add	entries	to	data	set									
            set.AddEntry(new BarEntry(0, myCountry.GetMineralIncome()));
            set.AddEntry(new BarEntry(1, myCountry.GetGunIncome()));
            set.AddEntry(new BarEntry(2, myCountry.GetIndividualTaxIncomeMonthly()));
            set.AddEntry(new BarEntry(3, myCountry.GetCorporationTaxIncomeMonthly()));
            set.AddEntry(new BarEntry(4, myCountry.GetOilIncome()));

            //	Set	bars	color
            set.BarColors.Add(Color.red);
            set.BarColors.Add(Color.gray);
            set.BarColors.Add(Color.green);
            set.BarColors.Add(Color.yellow);
            set.BarColors.Add(Color.blue);

            List<string> labels = new List<string>();
            labels.Add("Mineral Income");
            labels.Add("Gun Income");
            labels.Add("Individual Tax Income");
            labels.Add("Corporation Tax Income");
            labels.Add("Oil Income");

            incomeBar.GetChartData().CustomLabels = labels;

            //	Add	data	set	to	chart	data
            incomeBar.GetChartData().DataSets.Add(set);

            //	Refresh	chart	after	data	change							
            incomeBar.SetDirty();
        }
        public void SetExpenseBarChart()
        {
            Country myCountry = GameEventHandler.Instance.GetPlayer().GetMyCountry();

            expenseBar.GetChartData().DataSets.Clear();

            //	Create	data	set	for	entries
            BarDataSet set = new BarDataSet();
            //	Add	entries	to	data	set		
            
            if(myCountry.GetArmy() == null)
                set.AddEntry(new BarEntry(0, 0));

            set.AddEntry(new BarEntry(0, myCountry.GetArmy().GetArmyExpenseMonthly()));
            set.AddEntry(new BarEntry(1, myCountry.GetIntelligenceAgency().GetIntelligenceAgencyBudget()/12));
            set.AddEntry(new BarEntry(2, myCountry.GetDebtMonthly()));
            set.AddEntry(new BarEntry(3, 130)); // building maintanence


            //	Set	bars	color
            set.BarColors.Add(Color.red);
            set.BarColors.Add(Color.gray);
            set.BarColors.Add(Color.green);
            set.BarColors.Add(Color.yellow);

            List<string> labels = new List<string>();
            labels.Add("Army Maintanence");
            labels.Add("Intelligence Agency");
            labels.Add("Debt");
            labels.Add("Building Maintanence");

            expenseBar.GetChartData().CustomLabels = labels;

            //	Add	data	set	to	chart	data
            expenseBar.GetChartData().DataSets.Add(set);

            //	Refresh	chart	after	data	change							
            expenseBar.SetDirty();
        }

        public void HideEconomyPanel()
        {
            economyPanel.SetActive(false);
        }
    }
}