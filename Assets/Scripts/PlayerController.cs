﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public GUIText CountText;
	public GUIText PowerBar;
	public GUIText ReleaseAngle;
	private int count;
	private int flicked;
	private float moveDelta = .1f;
	private float MAX_DRUNK_FACTOR = .1f;
	private float MAX_FORCE = 2000;
	private Vector3 StartLocation = Vector3.zero;
	private float time = 60.0f;
	private float power = 0.0f;
	//private int angle = 0;
	private int delta = 1;
	private float angle = 0.0f;
	void start() {
		count = 0;
		flicked = 0;
		reset ();

	}
	void OnGUI () {
		angle = GUI.HorizontalSlider (new Rect (10, 25, 100, 30), angle, 0.0f, 90.0f);
	}

	
	void Update(){
		if (StartLocation == Vector3.zero) {
			StartLocation = transform.position;
		}
		if (Input.GetKeyUp (KeyCode.N)) {
			power = 1400.0f;
			flickBall ();
		}
		//set postion
		if (flicked == 0) {
			if(Input.GetKey(KeyCode.UpArrow)){
				transform.position = new Vector3(transform.position.x,transform.position.y + moveDelta,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.RightArrow)){
				transform.position = new Vector3(transform.position.x + moveDelta,transform.position.y ,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.LeftArrow)){
				transform.position = new Vector3(transform.position.x - moveDelta,transform.position.y ,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.DownArrow)){
				transform.position = new Vector3(transform.position.x,transform.position.y - moveDelta,transform.position.z);
			} 
			else if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
				flicked = 1;
			}
			else{
				//apply drunken factor
				if(Random.Range(0,5) == 2){
				transform.position = new Vector3(transform.position.x + GetDrunk(),transform.position.y + GetDrunk(),transform.position.z);
				}
			}
		}
		//set power
		else if(flicked == 1){
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
				flicked = 3;
				delta = 1;
			}
			else{
				power =power +  (float) delta * 15.0f;
				if(power >= MAX_FORCE || power <= 0.0f){
					if(power > MAX_FORCE) power = MAX_FORCE;
					if(power < 0.0f) power = 0.0f;
					delta = delta * -1;

				}
			}

		}
		//set angle
//		else if(flicked == 2){
//			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
//				flicked = 3;
//			}
//			else{
//				angle = angle +  delta;
//				if(angle >= 90 || angle <= 0){
//					if(angle > 90) power = 90;
//					if(angle < 0.0f) power = 0.0f;
//					delta = delta * -1;
//					
//				}
//			}
//			
//		}
		else if(flicked == 3){
			flickBall ();
		}
		time -= Time.deltaTime;
		if (time < - 0 || Input.GetKey(KeyCode.R)) {
			Application.LoadLevel(0);
		}
		SetCountText ();
	}

	void OnTriggerEnter (Collider other) {
		//other.gameObject.SetActive(false);
		if (other.name == "HitBox") {
			print ("IN if statement");
			count++;
		}
		SetCountText ();
		print (other.name);
		reset ();

	}
	void OnMouseDown(){
		flickBall ();
	}
	void SetCountText() {
		CountText.text = "Score: " + count.ToString() + " Time: " + time;
		string dashStr = "";
		int temp = 0;
		while (temp < power) {
			temp += (int) MAX_FORCE/10;
			dashStr += "-";
		}
		PowerBar.text = "Power: " + power.ToString() + " " +dashStr;
		ReleaseAngle.text = "Release Angle: " + angle.ToString();
	}

	void flickBall() {
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;

		flicked = 4;
		float yForce = power*(Mathf.Abs(Mathf.Sin((angle * Mathf.PI)/180)));
		float zForce = power*(Mathf.Abs(Mathf.Cos((angle * Mathf.PI)/180)));                     
		rigidbody.AddForce (0,yForce,zForce);
		print ("Total force: " + power.ToString() + " Force applied: Y " + yForce.ToString() + " Z " +zForce.ToString() + " Angle: " + angle.ToString());
	}
	float GetDrunk()
	{ 
		float temp = MAX_DRUNK_FACTOR * (count + 1);
		return Random.Range(-temp ,temp);
	}
	void reset(){
		transform.position = StartLocation;
		if (rigidbody != null) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		rigidbody.isKinematic = true;
		flicked = 0;
		power = 0.0f;
		angle = 0;
		delta = 1;

	}
}