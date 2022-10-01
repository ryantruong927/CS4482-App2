using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    private Rigidbody2D rigidbody2D;

	// Start is called before the first frame update
	private void Awake() {
		rigidbody2D = GetComponent<Rigidbody2D>();
	}

	private void Update() {
		if (transform.position.magnitude > 1000.0f)
			Destroy(gameObject);
	}

	public void Launch(Vector2 direction, float force) {
        rigidbody2D.AddForce(direction * force);
    }

	private void OnCollisionEnter2D(Collision2D collision) {
		MrClockworkController mrClockwork = collision.gameObject.GetComponent<MrClockworkController>();

		if (mrClockwork != null)
			mrClockwork.Fix();

		Destroy(gameObject);
	}
}
