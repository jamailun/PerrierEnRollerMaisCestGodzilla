using UnityEngine;

/// <summary>
/// Manage UIs. Define the keys and UI here in the scene.
/// </summary>
public class ManagerUI : MonoBehaviour {

	public static ManagerUI Instance { get; private set; }

	[Header("Config")]

	[Tooltip("The map of keys and windows.")]
	[SerializeField] private ManagerEntry[] windows;

	[Header("Screens")]

	[Tooltip("Reference to the death screen")]
	[SerializeField] private DeathScreen _deathScreen;
	public DeathScreen DeathScreen => _deathScreen;

	[Tooltip("Reference to the pause screen")]
	[SerializeField] private PauseScreen _pauseScreen;
	public PauseScreen PauseScreen => _pauseScreen;

	[Tooltip("Reference to the evolve screen")]
	[SerializeField] private EvolveScreen _evolveScreen;
	public EvolveScreen EvolveScreen => _evolveScreen;

	[Tooltip("Reference to the new-skill screen")]
	[SerializeField] private NewSkillScreen _newSkillScreen;
	public NewSkillScreen NewSkillScreen => _newSkillScreen;

	[Header("Elements")]

	[Tooltip("Reference to the skills displayer element")]
	[SerializeField] private SkillsDisplayer _skillsDisplayer;
	public SkillsDisplayer SkillsDisplayer => _skillsDisplayer;

	[Tooltip("Reference to the active skills handler element")]
	[SerializeField] private ActiveSkillButtonsHandler _activesButtons;
	public ActiveSkillButtonsHandler ActiveButtons => _activesButtons;

	[Tooltip("Reference to the timer element")]
	[SerializeField] private TimerUI _timer;
	public TimerUI Timer => _timer;

	[Tooltip("Reference to the score display element")]
	[SerializeField] private ScoreUI _score;
	public ScoreUI ScoreDisplay => _score;

	[Tooltip("Reference to the score label element")]
	[SerializeField] private TMPro.TMP_Text _scoreLabel;
	public TMPro.TMP_Text ScoreLabel => _scoreLabel;

	[Tooltip("Reference to the experience bar")]
	[SerializeField] private BarUI _experienceBar;
	public BarUI ExperienceBar => _experienceBar;

	[Tooltip("Reference to the experience label element")]
	[SerializeField] private TMPro.TMP_Text _experienceLabel;
	public TMPro.TMP_Text ExperienceLabel => _experienceLabel;

	[Header("Prefabs")]

	[Tooltip("Damage text prefab")]
	[SerializeField] private DamageText _damageText;
	public DamageText DamageTextPrefab => _damageText;

	[System.Serializable]
	public struct ManagerEntry {
		[SerializeField] public KeyCode keyCode;
		[SerializeField] public WindowUI window;
	}

	private void Awake() {
		if(Instance != null) {
			Destroy(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);
	}

	private void Update() {
		foreach(var w in windows) {
			if(Input.GetKeyDown(w.keyCode)) {
				bool newIsActive = !w.window.gameObject.activeSelf;
				//ClearPopups();
				w.window.gameObject.SetActive(newIsActive);
				if(newIsActive)
					w.window.effector?.Invoke();
			}
		}
	}

	public void Pause() {
		Time.timeScale = 0f;
		PauseScreen.gameObject.SetActive(true);
	}

	public void Unpause() {
		Time.timeScale = 1f;
		PauseScreen.gameObject.SetActive(false);
	}

}
