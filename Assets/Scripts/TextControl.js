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
	if(renderer.gameObject.name == "Play"){
		 Application.LoadLevel("MiniGames");
	}
	else{
		Application.Quit();
	}
}