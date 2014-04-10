using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float speed;
	public GUIText CountText;
	public GUIText PowerBar;
	public GUIText ReleaseAngle;
	public Camera player1;
	public Camera player2;
	private int count;
	private int flicked;
	private float moveDelta = .01f;
	private float MAX_DRUNK_FACTOR = .01f;
	private float MAX_FORCE = 800;
	private Vector3 StartLocation = Vector3.zero;
	private float time = 60.0f;
	private float power = 0.0f;
	private float angle = 0.0f;
	private float [] lastAngle = {0.0f,0.0f};
	private int delta = 1;

	private int whoseTurn = 1;

	void start() {
		count = 0;
		flicked = 0;
		print ("Called start\n");

	}
	void OnGUI () {
		angle = GUI.HorizontalSlider (new Rect (10, 50, 300, 120), angle, 0.0f, 90.0f);
		PowerBar.fontSize = 30;
		CountText.fontSize = 30;
		ReleaseAngle.fontSize = 30;
	}
	void Update(){
		if (StartLocation == Vector3.zero) {
			StartLocation = transform.position;
			reset ();
		}
		if (Input.GetKeyUp (KeyCode.N)) {
			power = 700.0f;
			angle = 45.0f;
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
				power = power + (float) delta * 15.0f;
				if(power >= MAX_FORCE || power <= 0.0f){
					if(power > MAX_FORCE) power = MAX_FORCE;
					if(power < 0.0f) power = 0.0f;
					delta = delta * -1;

				}
			}

		}
		//set angle
		else if(flicked == 2){
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter) ) {
				flicked = 3;
			}
			else{
				angle = angle +  delta;
				if(angle >= 90 || angle <= 0){
					if(angle > 90) power = 90;
					if(angle < 0.0f) power = 0.0f;
					delta = delta * -1;
					
				}
			}
			
		}
		else if(flicked == 3){
			flickBall ();
		}
//		time -= Time.deltaTime;
//		if (time < - 0 || Input.GetKey(KeyCode.R)) {
//			Application.LoadLevel(0);
//		}
		SetCountText ();
	}

	void OnTriggerEnter (Collider other) {
		//other.gameObject.SetActive(false);
		if (other.name == "HitBox") {
			print ("IN if statement");
			count++;
			//other.gameObject.SetActive(false);
			other.transform.parent.gameObject.SetActive(false);
		}
		SetCountText ();
		print (other.name);
		reset ();

	}

	void SetCountText() {
		 CountText.text = "YOLOSWAG40";
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
		float zForce = whoseTurn *power*(Mathf.Abs(Mathf.Cos((angle * Mathf.PI)/180)));                     
		rigidbody.AddForce (0,yForce,zForce);
		print ("Total force: " + power.ToString() + " Force applied: Y " + yForce.ToString() + " Z " +zForce.ToString() + " Angle: " + angle.ToString());
	}
	float GetDrunk()
	{ 
		float temp = MAX_DRUNK_FACTOR * (count + 1);
		return Random.Range(-temp ,temp);
	}
	void reset(){
		StartLocation.z = StartLocation.z + 6 * whoseTurn;
		print ("Called rest:" + StartLocation.z.ToString() + "\n");
		transform.position = StartLocation;
		if (rigidbody != null) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		rigidbody.isKinematic = true;
		flicked = 0;
		power = 0.0f;
		//save the players angle 
		if (whoseTurn == -1) {
			lastAngle[0] = angle;
			angle = lastAngle[1] + GetDrunk();
		}
		else{
			lastAngle[1] = angle;
			angle = lastAngle[0]+ GetDrunk();
		}
		delta = 1;
		if (whoseTurn > 0) {
			player1.enabled = false;
			player2.enabled = true;
		}
		else{
			player2.enabled = false;
			player1.enabled = true;
		}
		whoseTurn = whoseTurn * -1;
	}
}