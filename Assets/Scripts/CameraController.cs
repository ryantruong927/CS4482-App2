using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject startingRoom;
	private GameObject currentRoom;
	public GameObject CurrentRoom { get { return currentRoom; } }

	private Vector3 targetPosition;

	private float screenWidth;
	private bool isMoving;
	public float moveTime = 1f;

	private void Awake() {
		ChangeSize();

		MoveCamera(startingRoom, startingRoom.transform.position);
		targetPosition = transform.position;
		isMoving = false;
	}

	private void Update() {
		if (Screen.width != screenWidth)
			ChangeSize();

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

	private void ChangeSize() {
		//float aspectRatio = Camera.main.aspect;
		screenWidth = Screen.width;

		//Rect cameraRect = Camera.main.rect;
		//cameraRect.width = Camera.main.aspect / (16 / 9);
		//cameraRect.y = (1 - cameraRect.width) / 2;
		//Camera.main.rect = cameraRect;

		Camera.main.orthographicSize = (18 / screenWidth) * Screen.height * 0.5f;
		//if (aspectRatio < 1.7) { // not 16:9
		//	if (aspectRatio >= 1.5) // 3:2
		//		Camera.main.orthographicSize = ((Screen.height) / 64) * 0.5f;
		//	else
		//		Camera.main.orthographicSize = 7;
		//}
	}

	public void MoveCamera(GameObject newRoom, Vector3 position) {
		currentRoom = newRoom;
		targetPosition = new Vector3(position.x, position.y, -10);
		isMoving = true;
	}
}
