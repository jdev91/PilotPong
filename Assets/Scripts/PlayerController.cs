using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	//game objects
	public float speed;
	public GUIText CountText;
	public GUIText PowerBar;
	public GUIText ReleaseAngle;
	public Camera player1;
	public Camera player2;
	public GameObject[] blueCups;
	public GameObject[] redCups;
	public AudioClip fail;
	public AudioClip yay;

	//state of turn
	private int flicked = 0;

	//constants
	private float moveDelta = .01f;
	private float MAX_DRUNK_FACTOR = .005f;
	private float MAX_FORCE = 900;
	private float MIN_FORCE = 400;
	public  static float HEIGHT = 13.7f;
	private float HEIGHT_DELTA = .5f;
	public static float TABLE_WIDTH = .7f;
	private int MAX_ANGLE = 70;
	private bool madeCup = false;

	//ball status
	private Vector3 StartLocation = Vector3.zero;
	private float time = 60.0f;
	private float power = 0.0f;
	private float angle = 0.0f;
	private float [] lastAngle = {0.0f,0.0f};
	private bool bounced = false;
	private int delta = 1;
	private int checkRoll = 0;

	private int whoseTurn = 1;

	void OnGUI () {
		angle = GUI.VerticalSlider (new Rect (10, 50, 120, 200), angle, 70.0f, -70.0f);
		PowerBar.fontSize = 30;
		PowerBar.color = Color.blue;
		CountText.fontSize = 30;
		CountText.color = Color.white;
		ReleaseAngle.fontSize = 30;
		ReleaseAngle.color= Color.blue;
	}
	void Update(){
		if (StartLocation == Vector3.zero) {
			StartLocation = new Vector3(transform.position.x,HEIGHT,transform.position.z);
			CountText.text = "";
			reset ();
		}
		if (Input.GetKeyUp (KeyCode.K)) {
			GameObject [] cups = getOppCups();
			foreach(GameObject cup in cups){
				if(cup.activeInHierarchy){
					cup.SetActive(false);
					break;
				}
			}
			reset ();
		}
		if (Input.GetKeyUp (KeyCode.N)) {
			power = 700.0f;
			angle = 45.0f;
			flickBall ();
		}
		if (Input.GetKeyUp (KeyCode.R)) {
			Application.LoadLevel("MiniGames");
		}
		if(Input.GetKeyUp (KeyCode.Q)){
			Application.LoadLevel("main");
		}
		//set postion
		if (flicked == 0) {
			if (madeCup) {
				print ("I should be playing a sound");
				audio.PlayOneShot (yay);
				madeCup = false;
			}
			//apply drunken factor
			if(Random.Range(0,5) == 2){
				//force ball to be in middle of screen
				float yDrunk =  Clamp(transform.position.y + GetDrunk(),HEIGHT - HEIGHT_DELTA ,HEIGHT + HEIGHT_DELTA);
				//force ball to be on table
				float xDrunk =  Clamp(transform.position.x + GetDrunk(),TABLE_WIDTH * -1 ,TABLE_WIDTH);

				transform.position = new Vector3(xDrunk,yDrunk,transform.position.z);
			}
			if(Input.GetKey(KeyCode.UpArrow)){
				angle = Clamp (angle + 1, MAX_ANGLE * -1, MAX_ANGLE);
			}
			else if(Input.GetKey(KeyCode.RightArrow)){
				transform.position = new Vector3(transform.position.x + moveDelta * whoseTurn,transform.position.y ,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.LeftArrow)){
				transform.position = new Vector3(transform.position.x - moveDelta * whoseTurn,transform.position.y ,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.DownArrow)){
				angle = Clamp (angle - 1, MAX_ANGLE * -1, MAX_ANGLE);
			} 
			else if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
				flicked = 1;
			}
		}
		//set power
		else if(flicked == 1){
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
				flicked = 3;
				delta = 1;
			}
			else{
				power = power + (float) delta * 8.0f;
				if(power >= MAX_FORCE || power <= MIN_FORCE){
					if(power > MAX_FORCE) power = MAX_FORCE;
					if(power < MIN_FORCE) power = MIN_FORCE;
					delta = delta * -1;

				}
			}

		}
		//set angle
		else if(flicked == 2){
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter) ) {
				flicked = 3;
				bounced = false;
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
			other.transform.parent.gameObject.SetActive (false);
			if (bounced) {
				GameObject [] cups = getOppCups ();
				foreach (GameObject cup in cups) {
					if (cup.activeInHierarchy) {
						cup.SetActive (false);
						break;
					}
				}
			}
			madeCup = true;
		}
		else {
			audio.PlayOneShot (fail);
		}
		SetCountText ();
		print (other.name);
		reset ();

	}
	void OnCollisionEnter(Collision collision) {
		audio.Stop();
		ContactPoint contact = collision.contacts[0];
		print ("Got a collision here: " + contact.otherCollider.name + "---" + contact.thisCollider.name);
		if (contact.otherCollider.name == "table") {
			bounced = true;
		}
		audio.Play();
	}

	void SetCountText() {
		string dashStr = "";
		float temp = MIN_FORCE;
		while (temp < power) {
			temp += (int) (MAX_FORCE-MIN_FORCE)/10;
			dashStr += "-";
		}
		PowerBar.text = "Power: " + power.ToString() + " " +dashStr;
		ReleaseAngle.text = "Angle: " + angle.ToString();
	}

	void flickBall() {
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		
		flicked = 4;
		float yForce = power*(Mathf.Sin((angle * Mathf.PI)/180));
		float zForce = whoseTurn * power *(Mathf.Cos((angle * Mathf.PI)/180));                     
		rigidbody.AddForce (0,yForce,zForce);
		print ("Total force: " + power.ToString() + " Force applied: Y " + yForce.ToString() + " Z " +zForce.ToString() + " Angle: " + angle.ToString());
	}
	float GetDrunk()
	{ 
		float temp = MAX_DRUNK_FACTOR * (10-getMyCupsCount() + 1);
		return Random.Range(-temp ,temp);
	}
	void OnCollisionStay(Collision collisionInfo) {
		checkRoll += 1;
		if (checkRoll > 100){
			audio.PlayOneShot(fail);
			reset ();
		}
	}

	void reset(){
		if (checkGameOver ()) {
			return;
		}

		StartLocation.z = StartLocation.z + 6 * whoseTurn;
		transform.position = StartLocation;
		//if (rigidbody != null) {
			//rigidbody.velocity = Vector3.zero;
			//rigidbody.angularVelocity = Vector3.zero;
		//}
		rigidbody.isKinematic = true;
		flicked = 0;
		checkRoll = 0;
		power = 0.0f;
		//save the players angle 
		if (whoseTurn == -1) {
			lastAngle[0] = angle;
			angle = lastAngle[1];
		}
		else{
			lastAngle[1] = angle;
			angle = lastAngle[0];
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
	//cups on my side
	GameObject [] getMyCups(){
		GameObject [] cups = (whoseTurn > 0) ? blueCups : redCups;
		return cups;
	}
	//opponent cups
	GameObject [] getOppCups(){
		GameObject [] cups = (whoseTurn < 0) ? blueCups : redCups;
		return cups;
	}
	//num cups on opp side
	int getOppCupsCount(){
		GameObject [] cups = (whoseTurn < 0) ? blueCups : redCups;
		return getCount (cups);
	}
	//num cups on opp side
	int getMyCupsCount(){
		GameObject [] cups = (whoseTurn > 0) ? blueCups : redCups;
		return getCount (cups);
	}
	int getCount(GameObject [] cups){
		int num = 0;
		foreach(GameObject cup in cups){
			if(cup.activeInHierarchy){
				num += 1;
			}
		}
		print ("Num cups" + num.ToString ());
		if (num == 3) {
			int[] active = {4,7,8};
			int[] inactive = {0,1,2,3,5,6,9};
			foreach(int i in active){
				cups[i].SetActive(true);
			}
			foreach(int i in inactive){
				cups[i].SetActive(false);
			}
		}
		if(num == 6){
			print("Got a 6 here bro\n");
			int [] active = {9,8,7,5,4,2};
			int [] inactive = {0,1,3,6};
			foreach(int i in active){
				cups[i].SetActive(true);
			}
			foreach(int i in inactive){
				cups[i].SetActive(false);
			}
		}
		//print ("Num cups for: " + whoseTurn.ToString () + " : " + num.ToString ());
		return num;
	}
	bool checkGameOver(){
		if(getOppCupsCount() == 0){
			string winner = (whoseTurn > 0) ? " MFG" : "NUX";
			CountText.text = winner + " won";
			print ("WINNER IS " + whoseTurn.ToString());
			flicked = 4;//invalid state
			return true;

		}
		return false;
	}
	public static float Clamp( float value, float min, float max )
	{
		return (value < min) ? min : (value > max) ? max : value;
	}
	public static int Clamp( int value, int min, int max )
	{
		return (value < min) ? min : (value > max) ? max : value;
	}
}
