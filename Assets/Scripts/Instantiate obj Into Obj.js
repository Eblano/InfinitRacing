var brick : GameObject;

function Start () {

  var object = Instantiate(brick);
  object.transform.parent = transform;
}