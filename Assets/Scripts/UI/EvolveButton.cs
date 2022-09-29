using UnityEngine;
using UnityEngine.UI;

public class EvolveButton : MonoBehaviour {

	[SerializeField] private Image background;
	[SerializeField] private Image bannerImage;
	[SerializeField] private TMPro.TMP_Text bannerText;
	private EvolveScreen owner;

	private PlayerForm currentForm;

	private void Start() {
		owner = GetComponentInParent<EvolveScreen>();
		if(owner == null) {
			Debug.LogError("Could not find EvolveScreen parent for EvolveButton in " + gameObject.name);
			enabled = false;
		}
	}

	public void SetPlayerForm(PlayerForm form) {
		this.currentForm = form;
		// update graphics
		if(form == null) {
			bannerImage.sprite = null;
			bannerImage.color = Color.gray;
			bannerText.text = "";
		} else {
			bannerImage.sprite = form.BannerImage;
			bannerImage.color = Color.white;
			background.color = form.Color;
			bannerText.text = form.Name;
		}
	}

	public void FormClicked() {
		owner.FormChose(currentForm);
	}

}