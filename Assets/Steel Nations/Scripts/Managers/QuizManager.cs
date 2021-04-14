using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace WorldMapStrategyKit 
{
	public class QuizManager : MonoBehaviour
	{
		private static QuizManager instance;
		public static QuizManager Instance
		{
			get { return instance; }
		}

		public GameObject QuizGovernmentPanel;
		public GameObject attackButton;
		public GameObject QuestionPanel;
		public RawImage countryImage;

		public TextMeshProUGUI questionText;
		public TextMeshProUGUI leftTimeText;

		public TextMeshProUGUI attackCountry;
		public TextMeshProUGUI defenseCountry;
		public TextMeshProUGUI selectedRegionText;
		public TextMeshProUGUI selectedCountryText;

		public RawImage attackCountryImage;
		public RawImage defenseCountryImage;

		public GameObject selectionButtonList;

		public GameObject governmentItem;
		public GameObject overviewContent;

		WMSK map;

		IEnumerator timerCoroutine = null;

		public GameObject startButton;

		void Start()
		{
			map = WMSK.instance;
			instance = this;

			startButton.GetComponent<Button>().onClick.AddListener(delegate { SelectCountry.Instance.ChooseCountry(); });
			attackButton.GetComponent<Button>().onClick.AddListener(delegate { AttackCountry(); });
		}

		public void Init()
        {
			map.OnProvinceClick += OnProvinceClick;

			QuestionManager.Instance.Init();

			HidePanel();
		}

		void OnProvinceClick(int provinceIndex, int regionIndex, int buttonIndex)
		{
			Province province = map.GetProvince(provinceIndex);
			int countryIndex = province.countryIndex;

			Country selectedCountry = map.GetCountry(countryIndex);

			GameEventHandler.Instance.GetPlayer().SetSelectedCountry(selectedCountry);
			GameEventHandler.Instance.GetPlayer().SetSelectedProvince(province);

			ShowQuizGovernmentPanel();
		}

        void TransferRegion(Country country, Province province)
		{
			int targetCountryIndex = map.GetCountryIndex(country);

			map.CountryTransferProvinceRegion(targetCountryIndex, province.mainRegion, true);

			MapManager.Instance.ColorizeCountry(GameEventHandler.Instance.GetPlayer().GetMyCountry());
			//WriteProvinceNames(country);
		}

		public void WriteProvinceNames(Country country)
        {
			Debug.Log("-----------------------------------");
			foreach(Province province in map.GetProvinces(country))
            {
				Debug.Log(province.name);
            }
			Debug.Log("-----------------------------------");
		}

		void RandomTransferRegion(Country country, List<Province> provinceList)
		{
			if (provinceList.Count == 0)
				return;

			int index = Random.Range(0, provinceList.Count);

			Province province = provinceList[index];

			if (province != null)
            {
				if (province.mainRegion != null)
				{
					TransferRegion(country, province);

					SetInfoText(country.name + " has seized control of " + province.name);
				}
			}
		}

		bool HasBorder()
		{
			int countryIndex_1 = map.GetCountryIndex(GameEventHandler.Instance.GetPlayer().GetMyCountry().name);
			int countryIndex_2 = map.GetCountryIndex(GameEventHandler.Instance.GetPlayer().GetSelectedCountry().name);
			List<Vector2> points = map.GetCountryFrontierPoints(countryIndex_1, countryIndex_2);

			if (points.Count == 0)
				return false;
			return true;
		}

		public void AttackCountry()
        {
			if (GameEventHandler.Instance.GetPlayer().GetMyCountry() != GameEventHandler.Instance.GetPlayer().GetSelectedCountry())
			{
				if (HasBorder())
				{
					HidePanel();
					ShowQuestionPanel();
					CreateRandomQuestion();
				}
				else
                {
					SetInfoText("No Border");
                }
			}
		}

		public void ShowQuestionPanel()
        {
			QuestionPanel.SetActive(true);
			attackButton.SetActive(false);

			attackCountry.text = GameEventHandler.Instance.GetPlayer().GetMyCountry().name;
			defenseCountry.text = GameEventHandler.Instance.GetPlayer().GetSelectedCountry().name;

			attackCountryImage.texture = GameEventHandler.Instance.GetPlayer().GetMyCountry().GetCountryFlag();
			defenseCountryImage.texture = GameEventHandler.Instance.GetPlayer().GetSelectedCountry().GetCountryFlag();
		}

		public void HidePanel()
        {
			QuizGovernmentPanel.SetActive(false);
			ClearOverviewContent();
		}

		public void ShowQuizGovernmentPanel()
        {
			QuizGovernmentPanel.SetActive(true);
			ClearOverviewContent();

			Country selectedCountry = null;

			if(GameEventHandler.Instance.IsGameStarted() == false)
            {
				selectedCountry = SelectCountry.Instance.GetSelectedCountry();
				attackButton.SetActive(false);
			}
			else
            {
				selectedRegionText.text = GameEventHandler.Instance.GetPlayer().GetSelectedProvince().name;
				selectedCountry = GameEventHandler.Instance.GetPlayer().GetSelectedCountry();
				attackButton.SetActive(true);

				MapManager.Instance.FindBorderProvinces(GameEventHandler.Instance.GetPlayer().GetMyCountry(), selectedCountry);
			}

			countryImage.texture = selectedCountry.GetCountryFlag();
			selectedCountryText.text = selectedCountry.name;

			CreateOverviewButton("Manpower", string.Format("{0:#,0}", CountryManager.Instance.GetAvailableManpower(selectedCountry)));
			CreateOverviewButton("Religion", CountryManager.Instance.GetReligionNameByReligionType(selectedCountry.Religion));
			CreateOverviewButton("System Of Government", selectedCountry.System_Of_Government);
			CreateOverviewButton("Unemployment Rate", selectedCountry.Unemployment_Rate.ToString());
			CreateOverviewButton("Birth Rate", selectedCountry.Fertility_Rate_PerWeek.ToString());
			CreateOverviewButton("Rank", selectedCountry.Military_Rank.ToString());
		}

		GameObject CreateOverviewButton(string text, string value)
		{
			GameObject GO = Instantiate(governmentItem, overviewContent.transform);
			GO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = text;
			GO.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = value;

			return GO;
		}

		void ClearOverviewContent()
		{
			foreach (Transform child in overviewContent.transform)
			{
				Destroy(child.gameObject);
			}
		}

		void CreateRandomQuestion()
        {
			QuestionManager.Instance.CreateQuestion();

			SetQuestionText(QuestionManager.Instance.GetQuestion().GetQuestionText());
			timerCoroutine = StartTimer(1);
			StartCoroutine(timerCoroutine);

			RandomButtons(QuestionManager.Instance.GetQuestion());
		}

		void RandomButtons(Question question)
        {
			int index = -1;

			selectionButtonList.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
			selectionButtonList.transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
			selectionButtonList.transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
			selectionButtonList.transform.GetChild(3).GetComponent<Button>().onClick.RemoveAllListeners();

			foreach (string option in question.GetOptionList())
			{
				index++;
				SetSelection(index, option);
			}
		}

		void SetSelection(int index, string option)
        {
			selectionButtonList.transform.GetChild(index).GetComponent<Button>().onClick.AddListener(() => Selection(option));
			selectionButtonList.transform.GetChild(index).transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>().text = option;
		}

		void Selection(string selectedAnswer)
        {
			QuestionManager.Instance.GetQuestion().SetAnswer(selectedAnswer);
 
			ShowRightAnswer();
		}

		IEnumerator StartTimer(float waitTime)
		{
			while (map.paused == false)
			{
				yield return new WaitForSeconds(waitTime);

				TimeCount();
			}
		}

		void TimeCount()
        {
			if (QuestionManager.Instance.GetQuestion().GetLeftTime() == 0)
			{
				TimeIsOver();
			}
			else
			{
				QuestionManager.Instance.GetQuestion().DecreaseTime();

				SetLeftTime(QuestionManager.Instance.GetQuestion().GetLeftTime().ToString());
			}
		}

		void ClearQuestionPanel()
        {
			StopCoroutine(timerCoroutine);
			SetLeftTime("Time is over");

			QuestionPanel.SetActive(false);
		}

		void SetLeftTime(string leftTime)
        {
			leftTimeText.text = leftTime;
		}

		void SetQuestionText(string question)
        {
			questionText.text = question;
        }

		void TimeIsOver()
		{
			StopCoroutine(timerCoroutine);
			SetInfoText("Time is over");

			ShowRightAnswer();

			QuestionPanel.SetActive(false);
		}


		void ShowRightAnswer()
        {
			ClearQuestionPanel();

			if(QuestionManager.Instance.IsAnswerCorrect(QuestionManager.Instance.GetQuestion()) == true)
            {
				StopCoroutine(timerCoroutine);
				TransferRegion(GameEventHandler.Instance.GetPlayer().GetMyCountry(), GameEventHandler.Instance.GetPlayer().GetSelectedProvince());
				
				if (map.GetProvinces(GameEventHandler.Instance.GetPlayer().GetSelectedCountry()).Length == 0)
					SetInfoText(GameEventHandler.Instance.GetPlayer().GetMyCountry().name + " has destroyed " + GameEventHandler.Instance.GetPlayer().GetSelectedCountry());
				else
					SetInfoText("You Won");
			}
			else
            {
				StopCoroutine(timerCoroutine);

				List<Province> provinceList = MapManager.Instance.FindBorderProvinces(GameEventHandler.Instance.GetPlayer().GetMyCountry(), GameEventHandler.Instance.GetPlayer().GetSelectedCountry());

				if (provinceList.Count > 0)
				{
					if(provinceList.Count == 1)
						SetInfoText("Game Over");
					else
						RandomTransferRegion(GameEventHandler.Instance.GetPlayer().GetSelectedCountry(), provinceList);
				}
			}
		}

		void SetInfoText(string info)
        {
			UIManager.Instance.PublicNotification(info);
		}
	}
}

