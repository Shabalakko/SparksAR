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
        new Question("Qual è il pianeta più grande del Sistema Solare?", new string[] { "Terra", "Saturno", "Giove" }, 2),
        new Question("Qual è il pianeta più vicino al Sole?", new string[] { "Mercurio", "Venere", "Terra" }, 0),
        new Question("Quale pianeta è noto per i suoi spettacolari anelli?", new string[] { "Saturno", "Nettuno", "Marte" }, 0),
        new Question("Come viene definita una stella che esplode al termine della sua vita?", new string[] { "Supernova", "Cometa", "Nebulosa" }, 0),
        new Question("In quale galassia si trova il nostro Sistema Solare?", new string[] { "Andromeda", "Via Lattea", "Triangulum" }, 1),
        new Question("Quale pianeta è conosciuto come il 'pianeta rosso'?", new string[] { "Marte", "Giove", "Urano" }, 0),
        new Question("Cos'è una cometa?", new string[] { "Una stella", "Un pianeta", "Un corpo celeste composto da ghiaccio e polvere" }, 2),
        new Question("Qual è il satellite naturale della Terra?", new string[] { "Io", "Luna", "Europa" }, 1),
        new Question("Cos'è una nebulosa?", new string[] { "Un pianeta", "Un buco nero", "Una nube di gas e polvere nello spazio" }, 2),
        new Question("Quale pianeta possiede la velocità orbitale più alta?", new string[] { "Mercurio", "Venere", "Terra" }, 0),
        new Question("Quale di questi non è un pianeta del Sistema Solare?", new string[] { "Venere", "La Luna", "Saturno" }, 1),
        new Question("Cos'è un buco nero?", new string[] { "Un corpo celeste con una gravità così intensa che neanche la luce può sfuggire", "Un tipo di stella", "Una galassia" }, 0),
        new Question("Quale missione spaziale ha portato il primo uomo sulla Luna?", new string[] { "Apollo 11", "Gemini 4", "Vostok 1" }, 0),
        new Question("Cosa studia l'astronomia?", new string[] { "Il comportamento umano", "I corpi celesti e l'universo", "La vita marina" }, 1),
        new Question("Qual è il pianeta più caldo del Sistema Solare?", new string[] { "Venere", "Mercurio", "Marte" }, 0),
        new Question("Quale corpo celeste è famoso per la sua grande macchia rossa?", new string[] { "Giove", "Saturno", "Urano" }, 0),
        new Question("Cos'è la Via Lattea?", new string[] { "Una nebulosa", "La nostra galassia", "Un buco nero" }, 1),
        new Question("Cos'è un asteroide?", new string[] { "Un satellite", "Un piccolo corpo roccioso che orbita intorno al Sole", "Una cometa" }, 1),
        new Question("Quale pianeta del Sistema Solare ha il maggior numero di satelliti naturali confermati?", new string[] { "Saturno", "Giove", "Urano" }, 1),
        new Question("Quale di questi oggetti celesti si trova nel nostro Sistema Solare?", new string[]{ "La Nebulosa dell'Orsa Maggiore", "La Luna", "La Galassia di Andromeda"}, 1),
        new Question("Cosa rappresenta il termine \"esopianeta\"?", new string[]{ "Un pianeta all'interno del nostro Sistema Solare", "Un pianeta che orbita attorno a una stella diversa dal Sole", "Un satellite naturale"}, 1),
        new Question("Quale di questi corpi celesti è composto principalmente di ghiaccio?", new string[]{ "Un asteroide", "Una cometa", "Un pianeta"}, 1),
        new Question("Cosa si intende per \"orbita\"?", new string[]{ "La traiettoria che un corpo celeste segue intorno a un altro", "Una stella in esplosione", "Un tipo di nebulosa"}, 0),
        new Question("Qual è la principale differenza tra una stella e un pianeta?", new string[]{ "Le stelle emettono luce propria, i pianeti no", "I pianeti sono più grandi delle stelle", "Le stelle si trovano solo nelle galassie, i pianeti no"}, 0),
        new Question("Che cos'è una costellazione?", new string[]{ "Un gruppo di stelle che formano una figura nel cielo", "Una galassia", "Un tipo di pianeta"}, 1),
        new Question("Quale missione spaziale ha esplorato per la prima volta il pianeta Marte?", new string[]{ "Voyager 1", "Mars Pathfinder", "Apollo 11"}, 1),
        new Question("Cosa studia la cosmologia?", new string[]{ "Le caratteristiche dei pianeti", "I fenomeni atmosferici", "Le origini e l'evoluzione dell'universo"}, 2),
        new Question("Cos'è l'Equatore Celeste?", new string[]{ "La linea che separa il giorno dalla notte", "Un'orbita dei pianeti", "La linea immaginaria che divide il cielo in due emisferi"}, 2),
        new Question("Quale fenomeno è causato dall'interazione tra il Sole e il campo magnetico terrestre?", new string[]{ "Le eclissi", "I crateri lunari", "Le aurore boreali"}, 2),
        new Question("Qual è il principale componente del Sole?", new string[]{ "Elio", "Ossigeno", "Idrogeno"}, 2)
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

        yield return new WaitForSecondsRealtime(3);

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
