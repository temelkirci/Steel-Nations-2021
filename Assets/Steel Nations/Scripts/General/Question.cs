using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
	public enum QUESTION_TYPE
	{
		COUNTRY_HIGH_POPULATION,
		COUNTRY_LOW_POPULATION,

		COUNTRY_RICH,
		COUNTRY_POOR,
	}

	public class Question
	{

		QUESTION_TYPE questionType;
		int leftTime;
		string question;

		string answer;

		List<string> optionList;

		public Question()
        {
			leftTime = 10;
			question = string.Empty;
			answer = string.Empty;

			optionList = new List<string>();
		}

		public string GetAnswer()
        {
			return answer;
        }
		public void SetAnswer(string answer)
		{
			this.answer = answer;
		}

		public string GetQuestionText()
		{
			return question;
		}
		public void SetQuestionText(string question)
		{
			this.question = question;
		}

		public void SetLeftTime(int leftTime)
        {
			this.leftTime = leftTime;
		}

		public int GetLeftTime()
        {
			return leftTime;
        }

		public void DecreaseTime()
		{
			if(leftTime > 0)
				leftTime -=1;
		}

		public void SetQuestionType(QUESTION_TYPE questionType)
        {
			this.questionType = questionType;
		}
		public QUESTION_TYPE GetQuestionType()
		{
			return questionType;
		}

		public void AddOptionForQuestion(string answer)
		{
			Debug.Log(answer);
			optionList.Add(answer);
		}

		public List<string> GetOptionList()
        {
			return optionList;
        }
	}
}