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
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip timeUpSound;
    public AudioClip timeoutMessageSound;

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
            "Quelle est la plus petite planète du système solaire ?",
            new string[] { "Mars", "Mercury", "Pluto" },
            new string[] { "Mars", "Mercure", "Pluton" },
            1),
        new LocalizedQuestion(
            "What is the hottest planet in the Solar System?",
            "Quelle est la planète la plus chaude du système solaire ?",
            new string[] { "Mercury", "Venus", "Mars" },
            new string[] { "Mercure", "Vénus", "Mars" },
            1),
        new LocalizedQuestion(
            "Which planet is known as the Red Planet?",
            "Quelle planète est connue comme la planète rouge ?",
            new string[] { "Venus", "Mars", "Jupiter" },
            new string[] { "Vénus", "Mars", "Jupiter" },
            1),
        new LocalizedQuestion(
            "What celestial body orbits a planet?",
            "Quel corps céleste orbite autour d'une planète ?",
            new string[] { "Star", "Asteroid", "Moon" },
            new string[] { "Étoile", "Astéroïde", "Lune" },
            2),
        new LocalizedQuestion(
            "Which planet is famous for having a Great Red Spot?",
            "Quelle planète est célèbre pour avoir une Grande Tache Rouge ?",
            new string[] { "Jupiter", "Saturn", "Neptune" },
            new string[] { "Jupiter", "Saturne", "Neptune" },
            0),
        new LocalizedQuestion(
            "What force keeps planets in orbit around the Sun?",
            "Quelle force maintient les planètes en orbite autour du Soleil ?",
            new string[] { "Gravity", "Magnetism", "Tides" },
            new string[] { "Gravité", "Magnétisme", "Marées" },
            0),
        new LocalizedQuestion(
            "Which planet has the most moons?",
            "Quelle planète a le plus de lunes ?",
            new string[] { "Saturn", "Jupiter", "Mars" },
            new string[] { "Saturne", "Jupiter", "Mars" },
            1),
        new LocalizedQuestion(
            "What is the closest star to Earth?",
            "Quelle est l'étoile la plus proche de la Terre ?",
            new string[] { "Proxima Centauri", "Alpha Centauri", "The Sun" },
            new string[] { "Proxima Centauri", "Alpha Centauri", "Le Soleil" },
            2),
        new LocalizedQuestion(
            "What is the name of the galaxy closest to the Milky Way?",
            "Quel est le nom de la galaxie la plus proche de la Voie lactée ?",
            new string[] { "Andromeda", "Triangulum", "Whirlpool" },
            new string[] { "Andromède", "Triangulum", "Whirlpool" },
            0),
        new LocalizedQuestion(
            "What is a comet primarily made of?",
            "De quoi une comète est-elle principalement constituée ?",
            new string[] { "Ice and dust", "Rock and metal", "Gas and plasma" },
            new string[] { "Glace et poussière", "Roche et métal", "Gaz et plasma" },
            0),
        new LocalizedQuestion(
            "Which planet has the highest mountain in the Solar System?",
            "Quelle planète a la plus haute montagne du système solaire ?",
            new string[] { "Mars", "Earth", "Venus" },
            new string[] { "Mars", "Terre", "Vénus" },
            0),
        new LocalizedQuestion(
            "What is the name of the first human-made satellite?",
            "Quel est le nom du premier satellite fabriqué par l'homme ?",
            new string[] { "Sputnik 1", "Apollo 11", "Voyager 1" },
            new string[] { "Spoutnik 1", "Apollo 11", "Voyager 1" },
            0),
        new LocalizedQuestion(
            "Who was the first person to walk on the Moon?",
            "Qui a été la première personne à marcher sur la Lune ?",
            new string[] { "Neil Armstrong", "Buzz Aldrin", "Yuri Gagarin" },
            new string[] { "Neil Armstrong", "Buzz Aldrin", "Youri Gagarine" },
            0),
        new LocalizedQuestion(
            "What is the name of the largest asteroid in the asteroid belt?",
            "Quel est le nom du plus grand astéroïde de la ceinture d'astéroïdes ?",
            new string[] { "Vesta", "Ceres", "Pallas" },
            new string[] { "Vesta", "Cérès", "Pallas" },
            1),
        new LocalizedQuestion(
            "Which planet has a day longer than its year?",
            "Quelle planète a un jour plus long que son année ?",
            new string[] { "Venus", "Neptune", "Mercury" },
            new string[] { "Vénus", "Neptune", "Mercure" },
            0),
        new LocalizedQuestion(
            "What is the most common element in the Sun?",
            "Quel est l'élément le plus commun dans le Soleil ?",
            new string[] { "Oxygen", "Hydrogen", "Helium" },
            new string[] { "Oxygène", "Hydrogène", "Hélium" },
            1),
        new LocalizedQuestion(
            "Which planet rotates on its side?",
            "Quelle planète tourne sur son côté ?",
            new string[] { "Uranus", "Saturn", "Pluto" },
            new string[] { "Uranus", "Saturne", "Pluton" },
            0),
        new LocalizedQuestion(
            "Which planet has the shortest year in the Solar System?",
            "Quelle planète a l'année la plus courte du système solaire ?",
            new string[] { "Jupiter", "Mercury", "Neptune" },
            new string[] { "Jupiter", "Mercure", "Neptune" },
            1),
        new LocalizedQuestion(
            "Which planet has the longest year in the Solar System?",
            "Quelle planète a l'année la plus longue du système solaire ?",
            new string[] { "Uranus", "Neptune", "Saturn" },
            new string[] { "Uranus", "Neptune", "Saturne" },
            1),
        new LocalizedQuestion(
            "What is the Kuiper Belt?",
            "Qu'est-ce que la ceinture de Kuiper ?",
            new string[] { "A region beyond Neptune", "A ring around Saturn", "A group of comets around the Sun" },
            new string[] { "Une région au-delà de Neptune", "Un anneau autour de Saturne", "Un groupe de comètes autour du Soleil" },
            0),
        new LocalizedQuestion(
            "Which planet has the fastest winds in the Solar System?",
            "Quelle planète a les vents les plus rapides du système solaire ?",
            new string[] { "Neptune", "Saturn", "Mars" },
            new string[] { "Neptune", "Saturne", "Mars" },
            0),
        new LocalizedQuestion(
            "What is the Oort Cloud?",
            "Qu'est-ce que le nuage d'Oort ?",
            new string[] { "A region of icy bodies", "A black hole", "A planet's atmosphere" },
            new string[] { "Une région de corps glacés", "Un trou noir", "L'atmosphère d'une planète" },
            0),
        new LocalizedQuestion(
            "What is a light-year?",
            "Qu'est-ce qu'une année-lumière ?",
            new string[] { "A unit of time", "A unit of distance", "A measure of brightness" },
            new string[] { "Une unité de temps", "Une unité de distance", "Une mesure de luminosité" },
            1),
        new LocalizedQuestion(
            "What is the name of the first animal sent to space?",
            "Quel est le nom du premier animal envoyé dans l'espace ?",
            new string[] { "Laika", "Belka", "Strelka" },
            new string[] { "Laïka", "Belka", "Strelka" },
            0),
        new LocalizedQuestion(
            "Which space telescope has provided stunning images of deep space?",
            "Quel télescope spatial a fourni des images étonnantes de l'espace lointain ?",
            new string[] { "Hubble", "Kepler", "Chandra" },
            new string[] { "Hubble", "Kepler", "Chandra" },
            0),
        new LocalizedQuestion(
            "What is the largest volcano in the Solar System?",
            "Quel est le plus grand volcan du système solaire ?",
            new string[] { "Olympus Mons", "Mauna Loa", "Mount Everest" },
            new string[] { "Olympus Mons", "Mauna Loa", "Mont Everest" },
            0),
        new LocalizedQuestion(
            "What is the name of the first space station?",
            "Quel est le nom de la première station spatiale ?",
            new string[] { "Mir", "Skylab", "Salyut 1" },
            new string[] { "Mir", "Skylab", "Saliout 1" },
            2),
        new LocalizedQuestion(
            "Which planet is sometimes called Earth's twin?",
            "Quelle planète est parfois appelée la jumelle de la Terre ?",
            new string[] { "Mars", "Venus", "Neptune" },
            new string[] { "Mars", "Vénus", "Neptune" },
            1),
        new LocalizedQuestion(
            "Which planet has the lowest density?",
            "Quelle planète a la plus faible densité ?",
            new string[] { "Saturn", "Uranus", "Jupiter" },
            new string[] { "Saturne", "Uranus", "Jupiter" },
            0),
        new LocalizedQuestion(
            "Which dwarf planet was once considered the ninth planet?",
            "Quelle planète naine était autrefois considérée comme la neuvième planète ?",
            new string[] { "Ceres", "Eris", "Pluto" },
            new string[] { "Cérès", "Éris", "Pluton" },
            2),
        new LocalizedQuestion(
            "What is a pulsar?",
            "Qu'est-ce qu'un pulsar ?",
            new string[] { "A type of neutron star", "A small black hole", "A planet with rapid rotation" },
            new string[] { "Un type d'étoile à neutrons", "Un petit trou noir", "Une planète avec une rotation rapide" },
            0),
        new LocalizedQuestion(
            "Which spacecraft was the first to land on the Moon?",
            "Quel vaisseau spatial a été le premier à se poser sur la Lune ?",
            new string[] { "Luna 2", "Apollo 11", "Viking 1" },
            new string[] { "Luna 2", "Apollo 11", "Viking 1" },
            1),
        new LocalizedQuestion(
            "What is the name of NASA's rover currently exploring Mars?",
            "Quel est le nom du rover de la NASA qui explore actuellement Mars ?",
            new string[] { "Curiosity", "Opportunity", "Voyager" },
            new string[] { "Curiosity", "Opportunity", "Voyager" },
            0),
        new LocalizedQuestion(
            "What is dark matter?",
            "Qu'est-ce que la matière noire ?",
            new string[] { "Invisible mass in the universe", "A type of black hole", "A star in deep space" },
            new string[] { "Masse invisible dans l'univers", "Un type de trou noir", "Une étoile dans l'espace lointain" },
            0),
        new LocalizedQuestion(
            "What is dark energy?",
            "Qu'est-ce que l'énergie noire ?",
            new string[] { "An unknown force causing expansion", "A type of radiation", "The energy of black holes" },
            new string[] { "Une force inconnue provoquant l'expansion", "Un type de rayonnement", "L'énergie des trous noirs" },
            0),
        new LocalizedQuestion(
            "What is a quasar?",
            "Qu'est-ce qu'un quasar ?",
            new string[] { "A black hole with extreme brightness", "A type of pulsar", "A small galaxy" },
            new string[] { "Un trou noir d'une luminosité extrême", "Un type de pulsar", "Une petite galaxie" },
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
            new string[] { "Horizon des événements", "Singularité", "Anneau de photons" },
            0),
        new LocalizedQuestion(
            "What is the primary component of Saturn's rings?",
            "Quel est le composant principal des anneaux de Saturne ?",
            new string[] { "Ice and rock", "Dust and gas", "Iron and nickel" },
            new string[] { "Glace et roche", "Poussière et gaz", "Fer et nickel" },
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
            Debug.LogError("La lista delle domande ・vuota!");
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
            timerText.text = "";
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            if (timeoutMessageSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(timeoutMessageSound);
            }
            expiredMessageText.text = LanguageManager.instance.GetLocalizedText(timeUpKey, "Time's UP!");
            expiredMessageText.raycastTarget = true;
            yield return new WaitForSecondsRealtime(2f);

            expiredMessageText.text = "";
            expiredMessageText.raycastTarget = false;
            StartCoroutine(HandleTimeout());
        }
    }

    private IEnumerator HandleTimeout()
    {
        answered = true;

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (timeUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(timeUpSound);
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

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (index == correctAnswerIndex)
        {
            answerButtons[index].image.color = correctColor;
            currentPowerUp.GrantPowerUp();
            if (correctSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(correctSound);
            }
        }
        else
        {
            answerButtons[index].image.color = wrongColor;
            if (wrongSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(wrongSound);
            }
        }

        Destroy(currentPowerUp.gameObject);
        yield return new WaitForSecondsRealtime(2f);

        ResetUI();
    }

    private void ResetUI()
    {
        PlaySoundsOnActivation soundScript = GetComponent<PlaySoundsOnActivation>();
        if (soundScript != null)
        {
            soundScript.enabled = true;
            // Riproduci il suono di deattivazione standard SE configurato
            if (soundScript.deactivationSound != null)
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(soundScript.deactivationSound);
                }
                else
                {
                    Debug.LogWarning("AudioSource non trovato su " + gameObject.name);
                }
            }
        }
        else
        {
            Debug.LogWarning("Script PlaySoundsOnActivation non trovato su " + gameObject.name);
        }

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
        expiredMessageText.text = "";
        expiredMessageText.raycastTarget = false;

        if (myCanvasGroup != null)
        {
            myCanvasGroup.alpha = 1;
        }

        Time.timeScale = 1f;

        // Riproduci il timeUpSound ogni volta che il pannello si chiude
        AudioSource audioSourceTimeUp = GetComponent<AudioSource>();
        if (audioSourceTimeUp == null)
        {
            audioSourceTimeUp = gameObject.AddComponent<AudioSource>();
        }
        if (timeUpSound != null && audioSourceTimeUp != null)
        {
            audioSourceTimeUp.PlayOneShot(timeUpSound);
        }
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

