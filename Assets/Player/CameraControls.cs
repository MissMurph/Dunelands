using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControls : MonoBehaviour {

	public int camMoveSpeed;

	public int zoomMax, zoomMin, zoomSpeed;
	float zoomHeight;

	public GameObject player;

	void Start() {
		zoomHeight = transform.position.y;
	}

	void Update() {
		float x = transform.position.x; float y = transform.position.z;

		if (Input.GetKey("w")) y += camMoveSpeed;
		if (Input.GetKey("a")) x -= camMoveSpeed;
		if (Input.GetKey("s")) y -= camMoveSpeed;
		if (Input.GetKey("d")) x += camMoveSpeed;

		zoomHeight -= Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
		zoomHeight = Mathf.Clamp(zoomHeight, zoomMin, zoomMax);

		transform.position = Vector3.Lerp(transform.position, new Vector3(x, zoomHeight, y), Time.deltaTime);
	}
}