using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

using System.Linq;


public class PowerUpQuestionUI : MonoBehaviour
{
    public GameObject questionPanel;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public TMP_Text timerText;           // Testo del timer
    public TMP_Text expiredMessageText;  // Messaggio "Time's UP"

    [Header("Timer Settings")]
    public float questionTime = 10f; // Tempo a disposizione

    [Header("Timer Animation Settings")]
    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = Vector3.one * 2f;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float startAlpha = 1f;
    public float endAlpha = 0.2f;

    private Coroutine timerCoroutine;
    private PowerUp currentPowerUp;
    private int correctAnswerIndex;
    private bool answered = false;
    private Color defaultColor = Color.white;
    private Color correctColor = Color.green;
    private Color wrongColor = Color.red;

    public GameObject Lasers, PLaserR, PLaserB;
    private RectTransform timerRect;

    // Classe per la struttura della domanda
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

    // Lista delle domande
    private List<Question> questionList = new List<Question>
    {
        new Question("What is the smallest planet in the Solar System?", new string[] { "Mars", "Mercury", "Pluto" }, 1),
new Question("What is the hottest planet in the Solar System?", new string[] { "Mercury", "Venus", "Mars" }, 1),
new Question("Which planet is known as the Red Planet?", new string[] { "Venus", "Mars", "Jupiter" }, 1),
new Question("What celestial body orbits a planet?", new string[] { "Star", "Asteroid", "Moon" }, 2),
new Question("Which planet is famous for having a Great Red Spot?", new string[] { "Jupiter", "Saturn", "Neptune" }, 0),
new Question("What force keeps planets in orbit around the Sun?", new string[] { "Gravity", "Magnetism", "Tides" }, 0),
new Question("Which planet has the most moons?", new string[] { "Saturn", "Jupiter", "Mars" }, 1),
new Question("What is the closest star to Earth?", new string[] { "Proxima Centauri", "Alpha Centauri", "The Sun" }, 2),
new Question("What is the name of the galaxy closest to the Milky Way?", new string[] { "Andromeda", "Triangulum", "Whirlpool" }, 0),
new Question("What is a comet primarily made of?", new string[] { "Ice and dust", "Rock and metal", "Gas and plasma" }, 0),
new Question("Which planet has the highest mountain in the Solar System?", new string[] { "Mars", "Earth", "Venus" }, 0),
new Question("What is the name of the first human-made satellite?", new string[] { "Sputnik 1", "Apollo 11", "Voyager 1" }, 0),
new Question("Who was the first person to walk on the Moon?", new string[] { "Neil Armstrong", "Buzz Aldrin", "Yuri Gagarin" }, 0),
new Question("What is the name of the largest asteroid in the asteroid belt?", new string[] { "Vesta", "Ceres", "Pallas" }, 1),
new Question("Which planet has a day longer than its year?", new string[] { "Venus", "Neptune", "Mercury" }, 0),
new Question("What is the most common element in the Sun?", new string[] { "Oxygen", "Hydrogen", "Helium" }, 1),
new Question("Which planet rotates on its side?", new string[] { "Uranus", "Saturn", "Pluto" }, 0),
new Question("Which planet has the shortest year in the Solar System?", new string[] { "Jupiter", "Mercury", "Neptune" }, 1),
new Question("Which planet has the longest year in the Solar System?", new string[] { "Uranus", "Neptune", "Saturn" }, 1),
new Question("What is the Kuiper Belt?", new string[] { "A region beyond Neptune", "A ring around Saturn", "A group of comets around the Sun" }, 0),
new Question("Which planet has the fastest winds in the Solar System?", new string[] { "Neptune", "Saturn", "Mars" }, 0),
new Question("What is the Oort Cloud?", new string[] { "A region of icy bodies", "A black hole", "A planet's atmosphere" }, 0),
new Question("What is a light-year?", new string[] { "A unit of time", "A unit of distance", "A measure of brightness" }, 1),
new Question("What is the name of the first animal sent to space?", new string[] { "Laika", "Belka", "Strelka" }, 0),
new Question("Which space telescope has provided stunning images of deep space?", new string[] { "Hubble", "Kepler", "Chandra" }, 0),
new Question("What is the largest volcano in the Solar System?", new string[] { "Olympus Mons", "Mauna Loa", "Mount Everest" }, 0),
new Question("What is the name of the first space station?", new string[] { "Mir", "Skylab", "Salyut 1" }, 2),
new Question("Which planet is sometimes called Earth's twin?", new string[] { "Mars", "Venus", "Neptune" }, 1),
new Question("Which planet has the lowest density?", new string[] { "Saturn", "Uranus", "Jupiter" }, 0),
new Question("Which dwarf planet was once considered the ninth planet?", new string[] { "Ceres", "Eris", "Pluto" }, 2),
new Question("What is a pulsar?", new string[] { "A type of neutron star", "A small black hole", "A planet with rapid rotation" }, 0),
new Question("Which spacecraft was the first to land on the Moon?", new string[] { "Luna 2", "Apollo 11", "Viking 1" }, 1),
new Question("What is the name of NASA’s rover currently exploring Mars?", new string[] { "Curiosity", "Opportunity", "Voyager" }, 0),
new Question("What is dark matter?", new string[] { "Invisible mass in the universe", "A type of black hole", "A star in deep space" }, 0),
new Question("What is dark energy?", new string[] { "An unknown force causing expansion", "A type of radiation", "The energy of black holes" }, 0),
new Question("What is a quasar?", new string[] { "A black hole with extreme brightness", "A type of pulsar", "A small galaxy" }, 0),
new Question("Which mission first landed humans on the Moon?", new string[] { "Apollo 11", "Apollo 13", "Gemini 4" }, 0),
new Question("What is the name of the boundary around a black hole?", new string[] { "Event horizon", "Singularity", "Photon ring" }, 0),
new Question("What is the primary component of Saturn’s rings?", new string[] { "Ice and rock", "Dust and gas", "Iron and nickel" }, 0)


    };

    private void Start()
    {
        questionPanel.SetActive(false);
        timerText.text = "";
        expiredMessageText.text = "";
        expiredMessageText.raycastTarget = false;

        // Ottieni il RectTransform del timer per modificarne posizione e scala
        timerRect = timerText.GetComponent<RectTransform>();
        startPosition = timerRect.anchoredPosition;
    }

    public void ShowQuestion(PowerUp powerUp)
    {
        if (questionPanel.activeSelf)
            return;

        if (questionList.Count == 0)
        {
            Debug.LogError("La lista delle domande è vuota!");
            return;
        }

        answered = false;

        Time.timeScale = 0f;

        currentPowerUp = powerUp;

        int randomIndex = UnityEngine.Random.Range(0, questionList.Count);
        Question selectedQuestion = questionList[randomIndex];

        questionText.text = selectedQuestion.text;

        // Creiamo una lista temporanea per le risposte e i loro indici
        List<(string answer, int originalIndex)> shuffledAnswers = new List<(string, int)>();

        for (int i = 0; i < selectedQuestion.answers.Length; i++)
        {
            shuffledAnswers.Add((selectedQuestion.answers[i], i));
        }

        // Mescola la lista
        shuffledAnswers = shuffledAnswers.OrderBy(x => UnityEngine.Random.value).ToList();

        // Trova il nuovo indice della risposta corretta dopo la mescolazione
        for (int i = 0; i < shuffledAnswers.Count; i++)
        {
            if (shuffledAnswers[i].originalIndex == selectedQuestion.correctIndex)
            {
                correctAnswerIndex = i;
                break;
            }
        }

        // Configura i pulsanti con le risposte mescolate
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < shuffledAnswers.Count)
            {
                TMP_Text buttonText = answerButtons[i].GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = shuffledAnswers[i].answer;
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

        ResetTimerUI();

        // Disabilita temporaneamente altri componenti di gioco
        LaserGun compR = Lasers.GetComponent<LaserGun>();
        LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
        if (compB != null) compB.enabled = false;
        if (compR != null) compR.enabled = false;
        PLaserB.SetActive(false);
        PLaserR.SetActive(false);

        // Avvia il timer per la domanda
        timerCoroutine = StartCoroutine(QuestionTimerCoroutine());
    }


    // Coroutine per il countdown con animazione
    private IEnumerator QuestionTimerCoroutine()
    {
        float timeRemaining = questionTime;
        while (timeRemaining > 0f && !answered)
        {
            float t = 1 - (timeRemaining / questionTime); // Normalizza il tempo

            // Aggiorna il testo
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();

            // Cambia colore
            timerText.color = Color.Lerp(startColor, endColor, t);

            // Cambia scala (il valore massimo lo imposti tramite "endScale" nell'inspector)
            timerRect.localScale = Vector3.Lerp(startScale, endScale, t);

            // Rimuovi la modifica della posizione: il timer rimane fisso
            // timerRect.anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);

            // Cambia alpha
            Color currentColor = timerText.color;
            currentColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            timerText.color = currentColor;

            yield return new WaitForSecondsRealtime(0.1f);
            timeRemaining -= 0.1f;
        }

        if (!answered)
        {
            // Disabilita l'interazione dei bottoni delle risposte
            foreach (Button btn in answerButtons)
            {
                btn.interactable = false;
            }

            // Timer scaduto: mostra il messaggio di timeout
            timerText.text = "";
            expiredMessageText.text = "Time's UP!";
            yield return new WaitForSecondsRealtime(2f);

            expiredMessageText.text = "";
            StartCoroutine(HandleTimeout());
        }
    }


    private IEnumerator HandleTimeout()
    {
        if (answered) yield break;
        answered = true;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerText.text = "";
        }

        Destroy(currentPowerUp.gameObject);
        yield return new WaitForSecondsRealtime(0.1f);

        ResetUI();

    }

    private IEnumerator CheckAnswer(int index)
    {
        if (answered) yield break;
        answered = true;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerText.text = "";
        }

        if (index == correctAnswerIndex)
        {
            answerButtons[index].image.color = correctColor;
            currentPowerUp.GrantPowerUp();
        }
        else
        {
            answerButtons[index].image.color = wrongColor;
        }

        Destroy(currentPowerUp.gameObject);
        yield return new WaitForSecondsRealtime(2f);

        ResetUI();
    }

    private void ResetUI()
    {

        foreach (Button btn in answerButtons)
        {
            btn.image.color = defaultColor;
        }
        questionPanel.SetActive(false);

        foreach (Button btn in answerButtons) btn.image.color = defaultColor;

        LaserGun compR = Lasers.GetComponent<LaserGun>();
        LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
        if (compB != null) compB.enabled = true;
        if (compR != null) compR.enabled = true;
        PLaserB.SetActive(true);
        PLaserR.SetActive(true);
        questionPanel.SetActive(false);
        timerText.text = "";

        Time.timeScale = 1f;
    }

    private void ResetTimerUI()
    {
        timerText.color = startColor;
        timerRect.localScale = startScale;
        timerRect.anchoredPosition = startPosition;
    }
}
