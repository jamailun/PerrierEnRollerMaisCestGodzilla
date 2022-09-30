using UnityEngine;

/// <summary>
/// Manage UIs. Define the keys and UI here in the scene.
/// </summary>
public class ManagerUI : MonoBehaviour {

	[Header("Config")]

	[Tooltip("The map of keys and windows.")]
	[SerializeField] private ManagerEntry[] windows;

	[Header("Screens")]

	[Tooltip("Reference to the death screen")]
	[SerializeField] private DeathScreen _deathScreen;
	public DeathScreen DeathScreen => _deathScreen;

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

	[Tooltip("Reference to the timer element")]
	[SerializeField] private TimerUI _timer;
	public TimerUI Timer => _timer;

	[Tooltip("Reference to the score label element")]
	[SerializeField] private TMPro.TMP_Text _scoreLabel;
	public TMPro.TMP_Text ScoreLabel => _scoreLabel;

	[Tooltip("Reference to the experience bar")]
	[SerializeField] private BarUI _experienceBar;
	public BarUI ExperienceBar => _experienceBar;

	[Tooltip("Reference to the experience label element")]
	[SerializeField] private TMPro.TMP_Text _experienceLabel;
	public TMPro.TMP_Text ExperienceLabel => _experienceLabel;


	[System.Serializable]
	public struct ManagerEntry {
		[SerializeField] public KeyCode keyCode;
		[SerializeField] public WindowUI window;
	}

	private void Awake() {
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


}
