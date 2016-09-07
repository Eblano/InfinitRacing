var color : Material;

public var ElementNumber=0;

function Start () { 
   
if ( name == "Local Player" ){

       // NetMat();

       networkView.RPC ("NetMat", RPCMode.All);
   
}
}
@RPC
function NetMat(){

   var go = GameObject.Find("Network");
   color = go.GetComponent("NetworkConnection").NewMaterial;

   var mats = renderer.materials;
    mats[ElementNumber] = color ;
    renderer.materials = mats;
   

}

/*var color : String;
public var Color1 : Material ;
public var Color2 : Material ;
public var Color3 : Material ;

public var ElementNumber=0;


function Start () {
if (networkView.isMine){
 var go = GameObject.Find("Network");
   color = go.GetComponent("NetworkConnection").playerName;
   
    var mats = renderer.materials;
    mats[ElementNumber] = color ;
    renderer.materials = mats;

}
}
*/