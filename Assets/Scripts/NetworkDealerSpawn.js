

var vehiclePrefab : Transform;
var CarClone : String;


function Start(){
         if (Network.isServer){
		 Network.Instantiate(vehiclePrefab, transform.position, transform.rotation, 0);
}

} 
