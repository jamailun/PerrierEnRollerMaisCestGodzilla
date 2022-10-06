using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDisplayer : MonoBehaviour {

	public static DifficultyDisplayer Instance { get; private set; }

	[SerializeField] private RectTransform container;
	[SerializeField] private Image panelPrefab;
	[Header("Display configuration")]
	[SerializeField] private float secondsPerElement = 60f;
	[SerializeField] private DifficultyName[] difficulties;

	private DifficultyName[] runtime_diff;

	private int index = 0;

	private bool running = false;
	private float start;

	private RectTransform leftPanel;
	private RectTransform rightPanel;
	private RectTransform tempPanel;

	private void Awake() {
		if(Instance != null) {
			Destroy(this);
			return;
		}
		Instance = this;
	}

	public void Init(float start) {
		this.start = start;
		width = ((RectTransform) transform).sizeDelta.x;

		if(difficulties.Length < 2) {
			Debug.LogError("DifficultyDisplayer doesn't have enought difficulties.");
			return;
		}

		float init = start;
		int size = 0;
		foreach(var diff in difficulties)
			size += diff.number;

		runtime_diff = new DifficultyName[size];
		int n = 0;
		for(int i = 0; i < difficulties.Length; i++) {
			var source = difficulties[i];
			for (int j = 0; j < source.number; j++) {
				runtime_diff[n] = new DifficultyName { color = source.color, name = source.name, textInWhite = source.textInWhite, difficultyMultiplier = source.difficultyMultiplier };
				runtime_diff[n].elapsedStart = init;
				n++;
				init += secondsPerElement;
			}
		}

		container.DestroyChildren();
		StartCoroutine(Utils.DoAfter(0.5f, () => {
			leftPanel = CreateNewPanel(0);
			rightPanel = CreateNewPanel(1);
			running = true;
		}));

	}

	private float width;
	private float GetWidth() {
		return width;
	}

	private float GetWidthFor(float elapsedGlobal, DifficultyName difficulty, string debug = "<unset>") {
		//Debug.Log("[" + debug + "] is " + ((1f-(elapsedGlobal - difficulty.elapsedStart) / difficulty.durationSeconds)) + "%");
		return ((1f- (elapsedGlobal - difficulty.elapsedStart) / secondsPerElement) - 1f) * GetWidth();
	}

	private void Update() {
		if(!running)
			return;
		
		// Move panels

		float elapsed = Time.time - start;
		SetPanelX(tempPanel , elapsed, index - 1);
		SetPanelX(leftPanel , elapsed, index    );
		SetPanelX(rightPanel, elapsed, index + 1);

		// Try change difficulty

		if(elapsed > runtime_diff[index].elapsedStart) {
			// changement de difficultée
			index++;
			Debug.Log("-> DIFFICULTY " + runtime_diff[index].name);

			if(index < runtime_diff.Length - 1) {
				// we create a new panel
				if(tempPanel != null)
					Destroy(tempPanel.gameObject);

				tempPanel = leftPanel;
				leftPanel = rightPanel;
				rightPanel = CreateNewPanel(index + 1);
			} else {
				Destroy(this);
			}


		}
	}

	private RectTransform CreateNewPanel(int index) {
		var image = Instantiate(panelPrefab, container);
		image.color = runtime_diff[index].color;
		image.gameObject.name = "DIFF_PANEL(" + runtime_diff[index].name + ")";
		var tmp = image.GetComponentInChildren<TMPro.TMP_Text>();
		{
			tmp.text = runtime_diff[index].name;
			if(runtime_diff[index].textInWhite) {
				tmp.color = Color.white;
			}
		}
		
		var panel = image.rectTransform;
		panel.localPosition = new Vector2(GetWidth(), 0);
		return panel;
	}

	private void SetPanelX(RectTransform rt, float elapsed, int index) { // null safe !
		if(rt == null)
			return;
		if(index < 0 || index >= runtime_diff.Length)
			return;
		rt.localPosition = new Vector2(GetWidthFor(elapsed, runtime_diff[index], rt.gameObject.name), rt.localPosition.y);
	}

	[System.Serializable]
	public struct DifficultyName {
		public string name;
		public int number;
		public Color color;
		public bool textInWhite;
		public float difficultyMultiplier;

		[HideInInspector] public float elapsedStart;
	}

	public static float GetDifficultyMultiplier() {
		if(Instance == null)
			return 1f;
		if(Instance.runtime_diff == null) {
			Debug.LogWarning("init diff because of the difficulty multipleier !");
			Instance.Init(Time.time);
		}
		return Instance.runtime_diff[Instance.index].difficultyMultiplier;
	}
	
}