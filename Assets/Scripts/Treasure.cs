using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour {
    private Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null) {
            player.Win();
            anim.SetBool("Opened", true);
        }
    }
}
