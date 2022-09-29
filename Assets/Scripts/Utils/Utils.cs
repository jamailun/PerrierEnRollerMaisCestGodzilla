using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class offering static utils methods.
/// </summary>
public class Utils {
	private Utils() { }

	public const float PI_2 = Mathf.PI / 2f;

	/// <summary>
	/// Destroy a gamebject, if still valid, after some duration.
	/// </summary>
	/// <param name="gameObject">The game object to destroy.</param>
	/// <param name="duration">Duration before the deletion, in seconds.</param>
	/// <returns>#StartCoroutine answer.</returns>
	public static IEnumerator DestroyAfter(GameObject gameObject, float duration) {
		yield return new WaitForSeconds(duration);
		if(gameObject != null)
			Object.Destroy(gameObject);
	}

	// Moi j'aime bien le Java. C'est juste une 'fonction' qui prend aucun argument et qui renvoie void.
	public delegate void Runnable();

	public static IEnumerator DoAfter(float duration, Runnable runnable) {
		yield return new WaitForSeconds(duration);
		runnable();
	}
	
}

// Quel langage de merde...
static class CSharpExtension {
	public static void Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		while(n > 1) {
			n--;
			int k = Random.Range(0, n);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}