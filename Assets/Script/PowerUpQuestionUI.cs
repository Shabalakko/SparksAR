using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PowerUpQuestionUI : MonoBehaviour
{
    public GameObject questionPanel;
    public TMP_Text questionText;
    public Button[] answerButtons;
    private PowerUp currentPowerUp;
    private int correctAnswerIndex;
    private bool answered = false;
    private Color defaultColor = Color.white;
    private Color correctColor = Color.green;
    private Color wrongColor = Color.red;

    private class Question
    {
        public string text;
        public string[] answers;
        public int correctIndex;

        public Question(string text, string[] answers, int correctIndex)
        {
            this.text = text;
            this.answers = answers;
            this.correctIndex = correctIndex;
        }
    }

    private List<Question> questionList = new List<Question>
    {
        new Question("What is the largest planet in the Solar System?", new string[] { "Earth", "Saturn", "Jupiter" }, 2),
new Question("Which planet is closest to the Sun?", new string[] { "Mercury", "Venus", "Earth" }, 0),
new Question("Which planet is known for its spectacular rings?", new string[] { "Saturn", "Neptune", "Mars" }, 0),
new Question("What is the term for a star that explodes at the end of its life?", new string[] { "Supernova", "Comet", "Nebula" }, 0),
new Question("In which galaxy is our Solar System located?", new string[] { "Andromeda", "Milky Way", "Triangulum" }, 1),
new Question("Which planet is known as the 'red planet'?", new string[] { "Mars", "Jupiter", "Uranus" }, 0),
new Question("What is a comet?", new string[] { "A star", "A planet", "A celestial body made of ice and dust" }, 2),
new Question("What is Earth's natural satellite?", new string[] { "Io", "Moon", "Europa" }, 1),
new Question("What is a nebula?", new string[] { "A planet", "A black hole", "A cloud of gas and dust in space" }, 2),
new Question("Which planet has the highest orbital speed?", new string[] { "Mercury", "Venus", "Earth" }, 0),
new Question("Which of these is not a planet in the Solar System?", new string[] { "Venus", "The Moon", "Saturn" }, 1),
new Question("What is a black hole?", new string[] { "A celestial body with gravity so intense that not even light can escape", "A type of star", "A galaxy" }, 0),
new Question("Which space mission brought the first man to the Moon?", new string[] { "Apollo 11", "Gemini 4", "Vostok 1" }, 0),
new Question("What does astronomy study?", new string[] { "Human behavior", "Celestial bodies and the universe", "Marine life" }, 1),
new Question("Which is the hottest planet in the Solar System?", new string[] { "Venus", "Mercury", "Mars" }, 0),
new Question("Which celestial body is famous for its Great Red Spot?", new string[] { "Jupiter", "Saturn", "Uranus" }, 0),
new Question("What is the Milky Way?", new string[] { "A nebula", "Our galaxy", "A black hole" }, 1),
new Question("What is an asteroid?", new string[] { "A satellite", "A small rocky body that orbits the Sun", "A comet" }, 1),
new Question("Which planet in the Solar System has the greatest number of confirmed natural satellites?", new string[] { "Saturn", "Jupiter", "Uranus" }, 1),
new Question("Which of these celestial objects is found in our Solar System?", new string[]{ "The Great Bear Nebula", "The Moon", "The Andromeda Galaxy"}, 1),
new Question("What does the term \"exoplanet\" mean?", new string[]{ "A planet within our Solar System", "A planet that orbits a star other than the Sun", "A natural satellite"}, 1),
new Question("Which of these celestial bodies is primarily composed of ice?", new string[]{ "An asteroid", "A comet", "A planet"}, 1),
new Question("What is meant by \"orbit\"?", new string[]{ "The path that a celestial body follows around another", "An exploding star", "A type of nebula"}, 0),
new Question("What is the main difference between a star and a planet?", new string[]{ "Stars emit their own light, planets do not", "Planets are larger than stars", "Stars are found only in galaxies, planets are not"}, 0),
new Question("What is a constellation?", new string[]{ "A group of stars that form a pattern in the sky", "A galaxy", "A type of planet"}, 1),
new Question("Which space mission first explored the planet Mars?", new string[]{ "Voyager 1", "Mars Pathfinder", "Apollo 11"}, 1),
new Question("What does cosmology study?", new string[]{ "The characteristics of planets", "Atmospheric phenomena", "The origins and evolution of the universe"}, 2),
new Question("What is the Celestial Equator?", new string[]{ "The line that separates day from night", "An orbit of the planets", "The imaginary line that divides the sky into two hemispheres"}, 2),
new Question("Which phenomenon is caused by the interaction between the Sun and Earth's magnetic field?", new string[]{ "Eclipses", "Lunar craters", "Auroras"}, 2),
new Question("What is the main component of the Sun?", new string[]{ "Helium", "Oxygen", "Hydrogen"}, 2)

    };

    private void Start()
    {
        questionPanel.SetActive(false);
    }

    public void ShowQuestion(PowerUp powerUp)
    {
        if (questionPanel.activeSelf)
            return;

        answered = false;
        Time.timeScale = 0f;
        currentPowerUp = powerUp;

        Question selectedQuestion = questionList[Random.Range(0, questionList.Count)];
        questionText.text = selectedQuestion.text;
        correctAnswerIndex = selectedQuestion.correctIndex;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < selectedQuestion.answers.Length)
            {
                TMP_Text buttonText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = selectedQuestion.answers[i];
                }

                answerButtons[i].onClick.RemoveAllListeners();
                int index = i;
                answerButtons[i].onClick.AddListener(() => StartCoroutine(CheckAnswer(index)));
                answerButtons[i].interactable = true;
                answerButtons[i].image.color = defaultColor;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        questionPanel.SetActive(true);
    }

    private IEnumerator CheckAnswer(int index)
    {
        if (answered)
            yield break;

        answered = true;

        if (index == correctAnswerIndex)
        {
            answerButtons[index].image.color = correctColor;
            currentPowerUp.GrantPowerUp();
        }
        else
        {
            answerButtons[index].image.color = wrongColor;
        }

        // Distruggi il powerup indipendentemente dalla risposta
        Destroy(currentPowerUp.gameObject);

        yield return new WaitForSecondsRealtime(2);

        ResetUI();
    }

    private void ResetUI()
    {
        foreach (Button btn in answerButtons)
        {
            btn.image.color = defaultColor;
        }
        questionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
