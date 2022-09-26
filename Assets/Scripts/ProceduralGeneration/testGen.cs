using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class testGen : MonoBehaviour {

	[SerializeField] Texture2D inputTexture;
	[SerializeField] int width;
	[SerializeField] int height;

	private ProceduralGenerator gene;
	private SpriteRenderer sr;

	void Start() {
		gene = new ProceduralGenerator(inputTexture);
		Debug.Log("ProceduralGenerator created.");
		sr = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update() {
		if(Input.GetKeyDown(KeyCode.Backspace)) {
			Debug.Log("Creating image...");
			Texture2D texture =  gene.GenerateTexture(width, height);
			Debug.Log("Image created.");
			sr.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.zero);
		}
			
	}
	
}