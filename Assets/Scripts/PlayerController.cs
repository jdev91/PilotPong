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
	private int flicked;
	private float moveDelta = .01f;
	private float MAX_DRUNK_FACTOR = .01f;
	private float MAX_FORCE = 1000;
	private float MIN_FORCE = 300;
	private Vector3 StartLocation = Vector3.zero;
	private float time = 60.0f;
	private float power = 0.0f;
	private float angle = 0.0f;
	private float [] lastAngle = {0.0f,0.0f};
	public GameObject[] blueCups;
	public GameObject[] redCups;
	private bool bounced = false;
	private int delta = 1;
	private int checkRoll = 0;

	private int whoseTurn = 1;

	void start() {
		flicked = 0;
		print ("Called start\n");

	}
	void OnGUI () {
		angle = GUI.HorizontalSlider (new Rect (10, 50, 300, 120), angle, 0.0f, 90.0f);
		PowerBar.fontSize = 30;
		PowerBar.color = Color.black;
		CountText.fontSize = 30;
		CountText.color = Color.white;
		ReleaseAngle.fontSize = 30;
		ReleaseAngle.color= Color.black;
	}
	void Update(){
		if (StartLocation == Vector3.zero) {
			StartLocation = transform.position;
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
			if(Input.GetKey(KeyCode.UpArrow)){
				transform.position = new Vector3(transform.position.x,transform.position.y + moveDelta,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.RightArrow)){
				transform.position = new Vector3(transform.position.x + moveDelta * whoseTurn,transform.position.y ,transform.position.z);
			}
			else if(Input.GetKey(KeyCode.LeftArrow)){
				transform.position = new Vector3(transform.position.x - moveDelta * whoseTurn,transform.position.y ,transform.position.z);
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
				power = power + (float) delta * 10.0f;
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
			other.transform.parent.gameObject.SetActive(false);
			if(bounced){
				GameObject [] cups = getOppCups();
				foreach(GameObject cup in cups){
					if (cup.activeInHierarchy){
						cup.SetActive(false);
						break;
					}
				}
			}
		}
		SetCountText ();
		print (other.name);
		reset ();

	}
	void OnCollisionEnter(Collision collision) {
		ContactPoint contact = collision.contacts[0];
		print ("Got a collision here: " + contact.otherCollider.name + "---" + contact.thisCollider.name);
		if (contact.otherCollider.name == "table") {
			bounced = true;
		}
	}

	void SetCountText() {
		string dashStr = "";
		int temp = 0;
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
		float yForce = power*(Mathf.Abs(Mathf.Sin((angle * Mathf.PI)/180)));
		float zForce = whoseTurn *power*(Mathf.Abs(Mathf.Cos((angle * Mathf.PI)/180)));                     
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
}