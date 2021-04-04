using UnityEngine;
using System;

namespace WorldMapStrategyKit
{
	public class QuestionManager : Singleton<QuestionManager>
	{
		WMSK map;
		Question question = null;

		public void Init()
        {
			map = WMSK.instance;
		}

		public void CreateQuestion()
		{
			if(question == null)
				question = new Question();

			question.SetQuestionType(GetRandomQuestionType());
			question.SetLeftTime(10);

			question.GetOptionList().Clear();

			while (question.GetOptionList().Count != 4)
			{
				Country[] countryList = map.countries;
				int countryIndex = UnityEngine.Random.Range(0, countryList.Length);
				string tempCountry = countryList[countryIndex].name;

				if (question.GetOptionList().Contains(tempCountry) == false && CountryManager.Instance.GetAvailableManpower(countryList[countryIndex]) > 0 && tempCountry != string.Empty)
				{
					question.AddOptionForQuestion(tempCountry);
				}
			}

			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_HIGH_POPULATION)
			{
				question.SetQuestionText("Which country has more population ?");
			}
			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_LOW_POPULATION)
			{
				question.SetQuestionText("Which country has less population ?");
			}
			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_POOR)
			{
				question.SetQuestionText("Which country is poorer ?");
			}
			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_RICH)
			{
				question.SetQuestionText("Which country is richer ?");
			}
		}

		public Question GetQuestion()
        {
			return question;
        }

		public bool IsAnswerCorrect(Question question)
        {
			string rightAnswer = string.Empty;

			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_HIGH_POPULATION)
			{
				rightAnswer = question.GetOptionList()[0];

				foreach (string option in question.GetOptionList())
				{
					if (CountryManager.Instance.GetAvailableManpower(map.GetCountry(rightAnswer)) < CountryManager.Instance.GetAvailableManpower(map.GetCountry(option)))
					{
						rightAnswer = option;
					}
				}
			}
			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_LOW_POPULATION)
			{
				rightAnswer = question.GetOptionList()[0];

				foreach (string option in question.GetOptionList())
				{
					if (CountryManager.Instance.GetAvailableManpower(map.GetCountry(rightAnswer)) > CountryManager.Instance.GetAvailableManpower(map.GetCountry(option)))
					{
						rightAnswer = option;
					}
				}
			}
			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_POOR)
			{
				rightAnswer = question.GetOptionList()[0];

				foreach (string option in question.GetOptionList())
				{
					if (map.GetCountry(rightAnswer).Previous_GDP > map.GetCountry(option).Previous_GDP)
					{
						rightAnswer = option;
					}
				}
			}
			if (question.GetQuestionType() == QUESTION_TYPE.COUNTRY_RICH)
			{
				rightAnswer = question.GetOptionList()[0];

				foreach (string option in question.GetOptionList())
				{
					if (map.GetCountry(rightAnswer).Previous_GDP < map.GetCountry(option).Previous_GDP)
					{
						rightAnswer = option;
					}
				}
			}

			if (question.GetAnswer() == rightAnswer)
			{
				return true;			
			}
			else
			{
				return false;
			}
        }

		QUESTION_TYPE GetRandomQuestionType()
		{
			var rnd = new System.Random();
			return (QUESTION_TYPE)rnd.Next(Enum.GetNames(typeof(QUESTION_TYPE)).Length);
		}

	}
}