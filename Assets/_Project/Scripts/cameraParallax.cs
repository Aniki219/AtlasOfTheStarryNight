using UnityEngine;
using System.Collections;

// For usage apply the script directly to the element you wish to apply parallaxing
// Based on Brackeys 2D parallaxing script http://brackeys.com/
public class cameraParallax : MonoBehaviour {
	private Camera your_camera;
	public float parallax_value;
	Vector2 length;

	Vector3 startposition;
	// Start is called before the first frame update
	void Start()
	{
			startposition=transform.position;      
			length=GetComponentInChildren<SpriteRenderer>().bounds.size;
			your_camera = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
			Vector3 relative_pos=your_camera.transform.position*parallax_value;   
			Vector3 dist=your_camera.transform.position-relative_pos;
			if(dist.x>startposition.x+length.x)
			{
					startposition.x+=length.x;
			}
			if(dist.x<startposition.x-length.x)
			{
					startposition.x-=length.x;
			}  
			relative_pos.z=startposition.z;
			transform.position=startposition+relative_pos;      
			
	}
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class cameraParallax : MonoBehaviour
// {
//     cameraController cc;
//     Collider2D roomBounds;
//     Collider2D col;

//     float minX, minY;
//     float maxX, maxY;

//     // Start is called before the first frame update
//     void Start()
//     {
//         cc = Camera.main.GetComponent<cameraController>();
//         roomBounds = GameObject.Find("RoomBounds").GetComponent<BoxCollider2D>();
//         col = GetComponent<BoxCollider2D>();
//         minX = roomBounds.bounds.center.x - roomBounds.bounds.extents.x + col.bounds.extents.x;
//         maxX = roomBounds.bounds.center.x + roomBounds.bounds.extents.x - col.bounds.extents.x;

//         minY = roomBounds.bounds.center.y - roomBounds.bounds.extents.y + col.bounds.extents.y;
//         maxY = roomBounds.bounds.center.y + roomBounds.bounds.extents.y - col.bounds.extents.y;

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         transform.position = new Vector3(cc.percentX * (maxX - minX) + minX, transform.position.y, transform.position.z);
//         transform.position = new Vector3(transform.position.x, cc.percentY * (maxY - minY) + minY, transform.position.z);
//     }
// }