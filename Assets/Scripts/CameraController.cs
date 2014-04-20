using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;
	public GameObject thisCamera;
	private Vector3 offset = new Vector3(0.0f,.7f,3.0f);
	private int whoseTurn;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void LateUpdate () {
		if (thisCamera.activeInHierarchy) {
			setWhoseTurn ();
			Vector3 newPos = setView();
			Vector3 oldPos = player.transform.position;
			newPos.x = PlayerController.Clamp(oldPos.x + newPos.x,-1 * PlayerController.TABLE_WIDTH,PlayerController.TABLE_WIDTH);
			newPos.y = PlayerController.Clamp(newPos.y + oldPos.y,PlayerController.HEIGHT,PlayerController.HEIGHT + 3.0f);
			newPos.z = oldPos.z + newPos.z;
			thisCamera.transform.position = newPos;
		}
	}
	Vector3 setView(){
		Vector3 temp = offset;
		temp.z = temp.z * whoseTurn;
		print (whoseTurn.ToString() + " : " + temp.ToString());
		return temp;
	}
	void setWhoseTurn(){
		if (thisCamera.name == "Camera1") {
			whoseTurn = -1;
		}else{
			whoseTurn = 1;
		}
	}
}