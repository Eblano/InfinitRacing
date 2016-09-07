#pragma strict


public var x = 0.0;
public var y = 0.0;
public var z = 0.0;

	function Update() {


		transform.Rotate(x, y, z, Space.World);
		
	}