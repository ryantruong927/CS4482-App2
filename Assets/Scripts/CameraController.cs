using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject startingRoom;
	private GameObject currentRoom;
	public GameObject CurrentRoom { get { return currentRoom; } }

	private Vector3 targetPosition;

	private bool isMoving;
	public float moveTime = 1;

	private void Awake() {
		MoveCamera(startingRoom, startingRoom.transform.position);
		targetPosition = transform.position;
		isMoving = false;
	}

	private void Update() {
		if (isMoving) {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Vector3.Distance(transform.position, targetPosition) / moveTime * Time.unscaledDeltaTime);

			Time.timeScale = 0;

			if (Mathf.Abs(Vector3.Distance(transform.position, targetPosition)) <= 0.05f) {
				Time.timeScale = 1;
				transform.position = targetPosition;
				isMoving = false;
			}
		}
	}

	public void MoveCamera(GameObject newRoom, Vector3 position) {
		currentRoom = newRoom;
		targetPosition = new Vector3(position.x, position.y, -10);
		isMoving = true;
	}
}
