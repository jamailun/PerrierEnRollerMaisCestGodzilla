using System.Collections;
using UnityEngine;

public abstract class Activable : MonoBehaviour {

	public abstract void Activate(float damages = 0, float radiusBonus = 0);
}