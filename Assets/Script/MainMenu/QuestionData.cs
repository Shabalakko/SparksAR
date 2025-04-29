using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Quiz/Question Data", order = 1)]
public class QuestionData : ScriptableObject
{
    [System.Serializable]
    public class LocalizedQuestion
    {
        [TextArea(3, 10)]
        public string englishText;
        [TextArea(3, 10)]
        public string frenchText;
        public string correctAnswerEnglish;
        public string correctAnswerFrench;
        [TextArea(3, 10)]
        public string descriptionEnglish;
        [TextArea(3, 10)]
        public string descriptionFrench;
    }

    public LocalizedQuestion[] localizedQuestions;
}