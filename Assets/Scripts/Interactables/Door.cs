using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	public GameObject room1, room2;
	private GameObject currentRoom;

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.GetComponent<RubyController>() != null) {
			Debug.Log("Entered");
			Vector3 position = Camera.main.transform.position;

			if (position.x == room1.transform.position.x && position.y == room1.transform.position.y) {
				Camera.main.GetComponent<CameraController>().MoveCamera(room2, room2.transform.position);
				currentRoom = room2;
			}
			else {
				Camera.main.GetComponent<CameraController>().MoveCamera(room1, room1.transform.position);
				currentRoom = room1;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		RubyController ruby = collision.GetComponent<RubyController>();

		if (ruby != null) {
			Debug.Log("Exited");
			Vector3 rubyPosition = ruby.transform.position;
			Vector3 room1Position = room1.transform.position;
			Vector3 room2Position = room2.transform.position;

			if (Camera.main.GetComponent<CameraController>().CurrentRoom == room1) {
				if (Vector3.Distance(rubyPosition, room1Position) > Vector3.Distance(rubyPosition, room2Position)) {
					Camera.main.GetComponent<CameraController>().MoveCamera(room2, room2Position);
				}
			}
			else {
				if (Vector3.Distance(rubyPosition, room2Position) > Vector3.Distance(rubyPosition, room1Position)) {
					Camera.main.GetComponent<CameraController>().MoveCamera(room1, room1Position);
				}
			}
		}
	}
}

