using UnityEngine;
using System.Collections;

// For usage apply the script directly to the element you wish to apply parallaxing
// Based on Brackeys 2D parallaxing script http://brackeys.com/
public class cameraParallax : MonoBehaviour {
	Transform cam; // Camera reference (of its transform)
	Vector3 previousCamPos;

	public float smoothingX = 1f; // Smoothing factor of parrallax effect
	public float smoothingY = 1f;

	void Awake () {
		cam = Camera.main.transform;
	}
	
	void Update () {
			float parallaxX = (previousCamPos.x - cam.position.x) * transform.position.z;
			Vector3 backgroundTargetPosX = new Vector3(transform.position.x + parallaxX, 
			                                          transform.position.y, 
			                                          transform.position.z);
			
			// Lerp to fade between positions
			transform.position = Vector3.Lerp(transform.position, backgroundTargetPosX, smoothingX * Time.deltaTime);
		

			float parallaxY = (previousCamPos.y - cam.position.y) * transform.position.z;
			Vector3 backgroundTargetPosY = new Vector3(transform.position.x, 
			                                           transform.position.y + parallaxY, 
			                                           transform.position.z);
			
			transform.position = Vector3.Lerp(transform.position, backgroundTargetPosY, smoothingY * Time.deltaTime);
		

		previousCamPos = cam.position;	
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