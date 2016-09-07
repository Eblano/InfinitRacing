var namePers : String;
var target : Transform; 

function Start () { 
if (networkView.isMine){
   
   var go = GameObject.Find("Network");
   namePers = go.GetComponent("NetworkConnection").playerName;
   networkView.RPC("NamePerson", RPCMode.AllBuffered, namePers);
   
}
}

function LateUpdate() {
    
    target = GameObject.Find("Car Camera").transform;
    transform.LookAt(target);
    }



@RPC
function NamePerson(nameP : String){

   GetComponent(TextMesh).text = nameP;
   

}