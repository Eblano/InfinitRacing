using UnityEngine;
using System.Collections;

public class ColorChangeGame : MonoBehaviour 
{
	
    NetworkConnection networkConnection;
	
    public int Element=0;
	
	
    void Update () 
    {
        networkConnection = NetworkConnection.GetInst();
        if(networkConnection != null && name == "Local Player")
        {
		    InputColorChange();
	    }
    }
	

    private void InputColorChange()
    {
        //if (networkConnection.ColorName == "Black")
        //    ChangeColorTo(new Vector3(0, 0, 0));
		
        //if (networkConnection.ColorName == "White")
        //    ChangeColorTo(new Vector3(1, 1, 1));
		
        //if (networkConnection.ColorName == "Red")
        //    ChangeColorTo(new Vector3(1, 0, 0));
		
        //if (networkConnection.ColorName == "Green")
        //    ChangeColorTo(new Vector3(0, 1, 0));
		
        //if (networkConnection.ColorName == "Blue")
        //    ChangeColorTo(new Vector3(0, 0, 1));
				
        //if (networkConnection.ColorName == "Yellow")
        //    ChangeColorTo(new Vector3(1, 0.92f, 0.016f));
        ChangeColorTo(new Vector3(networkConnection.CarColor.r, networkConnection.CarColor.g, networkConnection.CarColor.b));
    }

    [RPC] void ChangeColorTo(Vector3 color)
    {
        renderer.materials[Element].color = new Color(color.x, color.y, color.z, 1f);

        if (networkView.isMine)
            networkView.RPC("ChangeColorTo", RPCMode.OthersBuffered, color);
    }
}