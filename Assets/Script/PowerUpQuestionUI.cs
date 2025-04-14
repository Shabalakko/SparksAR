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
    public TMP_Text timerText;
    public TMP_Text expiredMessageText;
    public CanvasGroup myCanvasGroup;

    [Header("Timer Settings")]
    public float questionTime = 10f;

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

    
    [System.Serializable]
    public class LocalizedQuestion
    {
        [TextArea(3, 10)]
        public string englishText;
        [TextArea(3, 10)]
        public string frenchText;
        public string[] englishAnswers;
        public string[] frenchAnswers;
        public int correctIndex;
        public LocalizedQuestion(string englishText, string frenchText, string[] englishAnswers, string[] frenchAnswers, int correctIndex)
        {
            this.englishText = englishText;
            this.frenchText = frenchText;
            this.englishAnswers = englishAnswers;
            this.frenchAnswers = frenchAnswers;
            this.correctIndex = correctIndex;
        }
    }

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

    [Header("Localized Questions")]
    public LocalizedQuestion[] localizedQuestions = new LocalizedQuestion[]
    {
        new LocalizedQuestion(
            "What is the smallest planet in the Solar System?",
            "Quelle est la plus petite plan�te du syst�me solaire ?",
            new string[] { "Mars", "Mercury", "Pluto" },
            new string[] { "Mars", "Mercure", "Pluton" },
            1),
        new LocalizedQuestion(
            "What is the hottest planet in the Solar System?",
            "Quelle est la plan�te la plus chaude du syst�me solaire ?",
            new string[] { "Mercury", "Venus", "Mars" },
            new string[] { "Mercure", "V�nus", "Mars" },
            1),
        new LocalizedQuestion(
            "Which planet is known as the Red Planet?",
            "Quelle plan�te est connue comme la plan�te rouge ?",
            new string[] { "Venus", "Mars", "Jupiter" },
            new string[] { "V�nus", "Mars", "Jupiter" },
            1),
        new LocalizedQuestion(
            "What celestial body orbits a planet?",
            "Quel corps c�leste orbite autour d'une plan�te ?",
            new string[] { "Star", "Asteroid", "Moon" },
            new string[] { "�toile", "Ast�ro�de", "Lune" },
            2),
        new LocalizedQuestion(
            "Which planet is famous for having a Great Red Spot?",
            "Quelle plan�te est c�l�bre pour avoir une Grande Tache Rouge ?",
            new string[] { "Jupiter", "Saturn", "Neptune" },
            new string[] { "Jupiter", "Saturne", "Neptune" },
            0),
        new LocalizedQuestion(
            "What force keeps planets in orbit around the Sun?",
            "Quelle force maintient les plan�tes en orbite autour du Soleil ?",
            new string[] { "Gravity", "Magnetism", "Tides" },
            new string[] { "Gravit�", "Magn�tisme", "Mar�es" },
            0),
        new LocalizedQuestion(
            "Which planet has the most moons?",
            "Quelle plan�te a le plus de lunes ?",
            new string[] { "Saturn", "Jupiter", "Mars" },
            new string[] { "Saturne", "Jupiter", "Mars" },
            1),
        new LocalizedQuestion(
            "What is the closest star to Earth?",
            "Quelle est l'�toile la plus proche de la Terre ?",
            new string[] { "Proxima Centauri", "Alpha Centauri", "The Sun" },
            new string[] { "Proxima Centauri", "Alpha Centauri", "Le Soleil" },
            2),
        new LocalizedQuestion(
            "What is the name of the galaxy closest to the Milky Way?",
            "Quel est le nom de la galaxie la plus proche de la Voie lact�e ?",
            new string[] { "Andromeda", "Triangulum", "Whirlpool" },
            new string[] { "Androm�de", "Triangulum", "Whirlpool" },
            0),
        new LocalizedQuestion(
            "What is a comet primarily made of?",
            "De quoi une com�te est-elle principalement constitu�e ?",
            new string[] { "Ice and dust", "Rock and metal", "Gas and plasma" },
            new string[] { "Glace et poussi�re", "Roche et m�tal", "Gaz et plasma" },
            0),
        new LocalizedQuestion(
            "Which planet has the highest mountain in the Solar System?",
            "Quelle plan�te a la plus haute montagne du syst�me solaire ?",
            new string[] { "Mars", "Earth", "Venus" },
            new string[] { "Mars", "Terre", "V�nus" },
            0),
        new LocalizedQuestion(
            "What is the name of the first human-made satellite?",
            "Quel est le nom du premier satellite fabriqu� par l'homme ?",
            new string[] { "Sputnik 1", "Apollo 11", "Voyager 1" },
            new string[] { "Spoutnik 1", "Apollo 11", "Voyager 1" },
            0),
        new LocalizedQuestion(
            "Who was the first person to walk on the Moon?",
            "Qui a �t� la premi�re personne � marcher sur la Lune ?",
            new string[] { "Neil Armstrong", "Buzz Aldrin", "Yuri Gagarin" },
            new string[] { "Neil Armstrong", "Buzz Aldrin", "Youri Gagarine" },
            0),
        new LocalizedQuestion(
            "What is the name of the largest asteroid in the asteroid belt?",
            "Quel est le nom du plus grand ast�ro�de de la ceinture d'ast�ro�des ?",
            new string[] { "Vesta", "Ceres", "Pallas" },
            new string[] { "Vesta", "C�r�s", "Pallas" },
            1),
        new LocalizedQuestion(
            "Which planet has a day longer than its year?",
            "Quelle plan�te a un jour plus long que son ann�e ?",
            new string[] { "Venus", "Neptune", "Mercury" },
            new string[] { "V�nus", "Neptune", "Mercure" },
            0),
        new LocalizedQuestion(
            "What is the most common element in the Sun?",
            "Quel est l'�l�ment le plus commun dans le Soleil ?",
            new string[] { "Oxygen", "Hydrogen", "Helium" },
            new string[] { "Oxyg�ne", "Hydrog�ne", "H�lium" },
            1),
        new LocalizedQuestion(
            "Which planet rotates on its side?",
            "Quelle plan�te tourne sur son c�t� ?",
            new string[] { "Uranus", "Saturn", "Pluto" },
            new string[] { "Uranus", "Saturne", "Pluton" },
            0),
        new LocalizedQuestion(
            "Which planet has the shortest year in the Solar System?",
            "Quelle plan�te a l'ann�e la plus courte du syst�me solaire ?",
            new string[] { "Jupiter", "Mercury", "Neptune" },
            new string[] { "Jupiter", "Mercure", "Neptune" },
            1),
        new LocalizedQuestion(
            "Which planet has the longest year in the Solar System?",
            "Quelle plan�te a l'ann�e la plus longue du syst�me solaire ?",
            new string[] { "Uranus", "Neptune", "Saturn" },
            new string[] { "Uranus", "Neptune", "Saturne" },
            1),
        new LocalizedQuestion(
            "What is the Kuiper Belt?",
            "Qu'est-ce que la ceinture de Kuiper ?",
            new string[] { "A region beyond Neptune", "A ring around Saturn", "A group of comets around the Sun" },
            new string[] { "Une r�gion au-del� de Neptune", "Un anneau autour de Saturne", "Un groupe de com�tes autour du Soleil" },
            0),
        new LocalizedQuestion(
            "Which planet has the fastest winds in the Solar System?",
            "Quelle plan�te a les vents les plus rapides du syst�me solaire ?",
            new string[] { "Neptune", "Saturn", "Mars" },
            new string[] { "Neptune", "Saturne", "Mars" },
            0),
        new LocalizedQuestion(
            "What is the Oort Cloud?",
            "Qu'est-ce que le nuage d'Oort ?",
            new string[] { "A region of icy bodies", "A black hole", "A planet's atmosphere" },
            new string[] { "Une r�gion de corps glac�s", "Un trou noir", "L'atmosph�re d'une plan�te" },
            0),
        new LocalizedQuestion(
            "What is a light-year?",
            "Qu'est-ce qu'une ann�e-lumi�re ?",
            new string[] { "A unit of time", "A unit of distance", "A measure of brightness" },
            new string[] { "Une unit� de temps", "Une unit� de distance", "Une mesure de luminosit�" },
            1),
        new LocalizedQuestion(
            "What is the name of the first animal sent to space?",
            "Quel est le nom du premier animal envoy� dans l'espace ?",
            new string[] { "Laika", "Belka", "Strelka" },
            new string[] { "La�ka", "Belka", "Strelka" },
            0),
        new LocalizedQuestion(
            "Which space telescope has provided stunning images of deep space?",
            "Quel t�lescope spatial a fourni des images �tonnantes de l'espace lointain ?",
            new string[] { "Hubble", "Kepler", "Chandra" },
            new string[] { "Hubble", "Kepler", "Chandra" },
            0),
        new LocalizedQuestion(
            "What is the largest volcano in the Solar System?",
            "Quel est le plus grand volcan du syst�me solaire ?",
            new string[] { "Olympus Mons", "Mauna Loa", "Mount Everest" },
            new string[] { "Olympus Mons", "Mauna Loa", "Mont Everest" },
            0),
        new LocalizedQuestion(
            "What is the name of the first space station?",
            "Quel est le nom de la premi�re station spatiale ?",
            new string[] { "Mir", "Skylab", "Salyut 1" },
            new string[] { "Mir", "Skylab", "Saliout 1" },
            2),
        new LocalizedQuestion(
            "Which planet is sometimes called Earth's twin?",
            "Quelle plan�te est parfois appel�e la jumelle de la Terre ?",
            new string[] { "Mars", "Venus", "Neptune" },
            new string[] { "Mars", "V�nus", "Neptune" },
            1),
        new LocalizedQuestion(
            "Which planet has the lowest density?",
            "Quelle plan�te a la plus faible densit� ?",
            new string[] { "Saturn", "Uranus", "Jupiter" },
            new string[] { "Saturne", "Uranus", "Jupiter" },
            0),
        new LocalizedQuestion(
            "Which dwarf planet was once considered the ninth planet?",
            "Quelle plan�te naine �tait autrefois consid�r�e comme la neuvi�me plan�te ?",
            new string[] { "Ceres", "Eris", "Pluto" },
            new string[] { "C�r�s", "�ris", "Pluton" },
            2),
        new LocalizedQuestion(
            "What is a pulsar?",
            "Qu'est-ce qu'un pulsar ?",
            new string[] { "A type of neutron star", "A small black hole", "A planet with rapid rotation" },
            new string[] { "Un type d'�toile � neutrons", "Un petit trou noir", "Une plan�te avec une rotation rapide" },
            0),
        new LocalizedQuestion(
            "Which spacecraft was the first to land on the Moon?",
            "Quel vaisseau spatial a �t� le premier � se poser sur la Lune ?",
            new string[] { "Luna 2", "Apollo 11", "Viking 1" },
            new string[] { "Luna 2", "Apollo 11", "Viking 1" },
            1),
        new LocalizedQuestion(
            "What is the name of NASA�s rover currently exploring Mars?",
            "Quel est le nom du rover de la NASA qui explore actuellement Mars ?",
            new string[] { "Curiosity", "Opportunity", "Voyager" },
            new string[] { "Curiosity", "Opportunity", "Voyager" },
            0),
        new LocalizedQuestion(
            "What is dark matter?",
            "Qu'est-ce que la mati�re noire ?",
            new string[] { "Invisible mass in the universe", "A type of black hole", "A star in deep space" },
            new string[] { "Masse invisible dans l'univers", "Un type de trou noir", "Une �toile dans l'espace lointain" },
            0),
        new LocalizedQuestion(
            "What is dark energy?",
            "Qu'est-ce que l'�nergie noire ?",
            new string[] { "An unknown force causing expansion", "A type of radiation", "The energy of black holes" },
            new string[] { "Une force inconnue provoquant l'expansion", "Un type de rayonnement", "L'�nergie des trous noirs" },
            0),
        new LocalizedQuestion(
            "What is a quasar?",
            "Qu'est-ce qu'un quasar ?",
            new string[] { "A black hole with extreme brightness", "A type of pulsar", "A small galaxy" },
            new string[] { "Un trou noir d'une luminosit� extr�me", "Un type de pulsar", "Une petite galaxie" },
            0),
        new LocalizedQuestion(
            "Which mission first landed humans on the Moon?",
            "Quelle mission a permis aux premiers humains de se poser sur la Lune ?",
            new string[] { "Apollo 11", "Apollo 13", "Gemini 4" },
            new string[] { "Apollo 11", "Apollo 13", "Gemini 4" },
            0),
        new LocalizedQuestion(
            "What is the name of the boundary around a black hole?",
            "Quel est le nom de la limite autour d'un trou noir ?",
            new string[] { "Event horizon", "Singularity", "Photon ring" },
            new string[] { "Horizon des �v�nements", "Singularit�", "Anneau de photons" },
            0),
        new LocalizedQuestion(
            "What is the primary component of Saturn�s rings?",
            "Quel est le composant principal des anneaux de Saturne ?",
            new string[] { "Ice and rock", "Dust and gas", "Iron and nickel" },
            new string[] { "Glace et roche", "Poussi�re et gaz", "Fer et nickel" },
            0)
    };
    private List<Question> currentQuestionList = new List<Question>();

    [Header("Localization Keys")]
    public string timeUpKey = "time_up";

    private void Awake()
    {
        UpdateQuestionList();
    }

    private void OnEnable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged += UpdateQuestionList;
            LanguageManager.onLanguageChanged += UpdateLocalizedUI;
        }
    }

    private void OnDisable()
    {
        if (LanguageManager.instance != null)
        {
            LanguageManager.onLanguageChanged -= UpdateQuestionList;
            LanguageManager.onLanguageChanged -= UpdateLocalizedUI;
        }
    }

    private void UpdateQuestionList()
    {
        currentQuestionList.Clear();
        if (LanguageManager.instance == null)
        {
            Debug.LogError("LanguageManager instance is null.  Make sure it exists in the scene.");
            return;
        }
        string currentLanguage = LanguageManager.instance.currentLanguage;

        if (localizedQuestions != null)
        {
            foreach (var localizedQuestion in localizedQuestions)
            {
                string questionText = (currentLanguage == "FRENCH") ? localizedQuestion.frenchText : localizedQuestion.englishText;
                string[] answers = (currentLanguage == "FRENCH") ? localizedQuestion.frenchAnswers : localizedQuestion.englishAnswers;
                currentQuestionList.Add(new Question(questionText, answers, localizedQuestion.correctIndex));
            }
        }
        else
        {
            Debug.LogError("localizedQuestions array is null.  Please populate it in the inspector.");
        }
    }

    private void Start()
    {
        questionPanel.SetActive(false);
        timerText.text = "";
        expiredMessageText.text = "";
        expiredMessageText.raycastTarget = false;

        timerRect = timerText.GetComponent<RectTransform>();
        if (timerRect != null)
        {
            startPosition = timerRect.anchoredPosition;
        }
        else
        {
            Debug.LogError("timerText's RectTransform is null.  Please assign the timerText object in the inspector.");
        }
        UpdateLocalizedUI();
    }

    public void ShowQuestion(PowerUp powerUp)
    {
        if (questionPanel.activeSelf)
            return;

        if (currentQuestionList.Count == 0)
        {
            Debug.LogError("La lista delle domande � vuota!");
            return;
        }

        if (myCanvasGroup != null)
        {
            myCanvasGroup.alpha = 0;
        }

        answered = false;
        Time.timeScale = 0f;
        currentPowerUp = powerUp;

        int randomIndex = UnityEngine.Random.Range(0, currentQuestionList.Count);
        Question selectedQuestion = currentQuestionList[randomIndex];

        questionText.text = selectedQuestion.text;

        List<(string answer, int originalIndex)> shuffledAnswers = new List<(string, int)>();

        for (int i = 0; i < selectedQuestion.answers.Length; i++)
        {
            shuffledAnswers.Add((selectedQuestion.answers[i], i));
        }

        shuffledAnswers = shuffledAnswers.OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < shuffledAnswers.Count; i++)
        {
            if (shuffledAnswers[i].originalIndex == selectedQuestion.correctIndex)
            {
                correctAnswerIndex = i;
                break;
            }
        }

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

        LaserGun compR = Lasers.GetComponent<LaserGun>();
        LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
        if (compB != null) compB.enabled = false;
        if (compR != null) compR.enabled = false;
        PLaserB.SetActive(false);
        PLaserR.SetActive(false);

        ParticleSystem psR = PLaserR.GetComponent<ParticleSystem>();
        if (psR != null)
        {
            psR.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        ParticleSystem psB = PLaserB.GetComponent<ParticleSystem>();
        if (psB != null)
        {
            psB.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        compR = Lasers.GetComponent<LaserGun>();
        compB = Lasers.GetComponent<LaserGunBlue>();
        if (compB != null) compB.enabled = false;
        if (compR != null) compR.enabled = false;

        timerCoroutine = StartCoroutine(QuestionTimerCoroutine());
    }


    private IEnumerator QuestionTimerCoroutine()
    {
        float timeRemaining = questionTime;
        while (timeRemaining > 0f && !answered)
        {
            // Aggiorna il testo del timer *qui*
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
            float t = 1 - (timeRemaining / questionTime);
            timerText.color = Color.Lerp(startColor, endColor, t);
            if (timerRect != null)
            {
                timerRect.localScale = Vector3.Lerp(startScale, endScale, t);
            }
            Color currentColor = timerText.color;
            currentColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            timerText.color = currentColor;

            yield return new WaitForSecondsRealtime(0.1f);
            timeRemaining -= 0.1f;
        }

        if (!answered)
        {
            foreach (Button btn in answerButtons)
            {
                btn.interactable = false;
            }
            timerText.text = ""; // Cancella il testo del timer anche qui
            expiredMessageText.text = LanguageManager.instance.GetLocalizedText(timeUpKey, "Time's UP!");
            expiredMessageText.raycastTarget = true;
            yield return new WaitForSecondsRealtime(2f);

            expiredMessageText.text = "";
            expiredMessageText.raycastTarget = false;
            StartCoroutine(HandleTimeout());
        }
    }

    private IEnumerator HandleTimeout() // Nuovo metodo per gestire il timeout
    {
        answered = true; // Imposta answered a true qui
        Destroy(currentPowerUp.gameObject);
        yield return new WaitForSecondsRealtime(0.1f);
        ResetUI();
    }


    private IEnumerator CheckAnswer(int index)
    {
        if (answered) yield break;
        answered = true; // Imposta answered a true qui

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

        foreach (Button btn in answerButtons)
            btn.image.color = defaultColor;

        LaserGun compR = Lasers.GetComponent<LaserGun>();
        LaserGunBlue compB = Lasers.GetComponent<LaserGunBlue>();
        if (compB != null) compB.enabled = true;
        if (compR != null) compR.enabled = true;
        PLaserB.SetActive(true);
        PLaserR.SetActive(true);
        questionPanel.SetActive(false);
        timerText.text = "";
        expiredMessageText.text = "";  //aggiunto anche qui
        expiredMessageText.raycastTarget = false; //assicuriamoci che sia disabilitato anche qui

        if (myCanvasGroup != null)
        {
            myCanvasGroup.alpha = 1;
        }

        Time.timeScale = 1f;
    }

    private void ResetTimerUI()
    {
        timerText.color = startColor;
        if (timerRect != null)
        {
            timerRect.localScale = startScale;
            timerRect.anchoredPosition = startPosition;
        }
    }

    public void UpdateLocalizedUI()
    {
        if (LanguageManager.instance != null)
        {
            expiredMessageText.text = ""; //anche qui
            expiredMessageText.raycastTarget = false;
        }
        UpdateQuestionList();
    }
}

