﻿using UnityEngine;
using System.Collections;

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