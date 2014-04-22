//http://www.youtube.com/watch?v=vfL7kQeZtic

var smooth = 2.0;
var DoorOpenAngle = 90.0;
private var open = true;
private var enter : boolean;

private var defaultRot : Vector3;
private var openRot : Vector3;

var hit : RaycastHit;

function Start(){
	defaultRot = transform.eulerAngles;
	openRot = new Vector3 (defaultRot.x, defaultRot.y + DoorOpenAngle, defaultRot.z);
}

//Main function
function Update (){
	if(open){
		//Open door
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
	}

	if(Input.GetMouseButtonDown(0) && 
	   collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), hit, Mathf.Infinity) && enter){
		print("fack");
		open = !open;
	}
}

function OnGUI(){
	if(enter){
		GUI.Label(new Rect(Screen.width/2 - 75, Screen.height - 100, 150, 30), "Press 'F' to open the door");
	}
}
