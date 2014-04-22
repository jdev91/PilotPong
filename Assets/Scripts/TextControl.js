#pragma strict
var  c;
function Start () {
	c = renderer.material.color;
}

function Update () { 
}
function OnMouseEnter(){
	//fuck off bro
	renderer.material.color = Color.red;
	
}
function OnMouseExit(){
	renderer.material.color = c;
}
function OnMouseDown(){
	if(renderer.gameObject.name == "Exit"){
		Application.Quit();
		print("shit");
	}
	if(renderer.gameObject.name == "Door"){
		Application.LoadLevel("MiniGames");
	}
}