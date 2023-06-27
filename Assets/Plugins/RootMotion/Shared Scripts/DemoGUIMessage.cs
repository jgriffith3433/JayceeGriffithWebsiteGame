using UnityEngine;
using System.Collections;

namespace RootMotion {

	// Just for displaying a GUI text in the Game View.
	public class DemoGUIMessage : MonoBehaviour {

		public string text;
		public Color color = Color.white;
		public int size = 8;

		void OnGUI() {
			GUI.color = color;
			GUILayout.Label(text);
			GUI.color = Color.white;
			GUI.skin.label.fontSize = size;
		}
	}
}
