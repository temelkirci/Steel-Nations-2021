using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit
{
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager instance;
        public static LocalizationManager Instance
        {
            get { return instance; }
        }

        public enum LANGUAGE
        {
            ENGLISH,
            TURKISH
        }

        LANGUAGE selectedLanguage;

        public LANGUAGE GetLanguage()
        {
            return selectedLanguage;
        }

        public void SetLanguageAsTurkish()
        {
            selectedLanguage = LANGUAGE.TURKISH;
        }

        public void SetLanguageAsEnglish()
        {
            selectedLanguage = LANGUAGE.ENGLISH;
        }
    }
}