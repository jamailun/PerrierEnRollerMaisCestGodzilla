using UnityEngine;

public class ScoreUI : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text text;

	public void UpdateScore(uint amount) {
		text.text = "Score: " + amount;
	}

}