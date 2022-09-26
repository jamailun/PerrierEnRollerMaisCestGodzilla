using UnityEngine;
using UnityEngine.Events;

public abstract class WindowUI : MonoBehaviour {

	[Tooltip("Events to call when the windows is toggled on.")]
	[SerializeField] public UnityEvent effector;

}
