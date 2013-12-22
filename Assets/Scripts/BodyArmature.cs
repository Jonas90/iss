using UnityEngine;

public class BodyArmature
{
	private GameObject spine;
	//	private GameObject neck;
	//	private GameObject head;
	//	
	//	private GameObject shoulderLeft;
	private GameObject armUpperLeft;
	//	private GameObject armLowerLeft;
	//	private GameObject handLeft;
	//	
	//	private GameObject shoulderRight;
	//	private GameObject armUpperRight;
	//	private GameObject armLowerRight;
	//	private GameObject handRight;
	//	
	//	private GameObject hipLeft;
	//	private GameObject legUpperLeft;
	//	private GameObject legLowerLeft;
	//	private GameObject footLeft;
	//	
	//	private GameObject hipRight;
	//	private GameObject legUpperRight;
	//	private GameObject legLowerRight;
	//	private GameObject footRight;

	public BodyArmature(string name)
	{
		string start = name + "/Armature/";

		spine = GameObject.Find(start + "Root/Spine");
		//neck  = GameObject.Find(start + "Root/Spine/Neck");
		//head  = GameObject.Find(start + "Root/Spine/Neck/Head");
		//
		//shoulderLeft = GameObject.Find(start + "Root/Spine/Shoulder_L");
		armUpperLeft = GameObject.Find(start + "Root/Spine/Shoulder_L/ArmUpper_L");
		//armLowerLeft = GameObject.Find(start + "Root/Spine/Shoulder_L/ArmUpper_L/ArmLower_L");
		//handLeft     = GameObject.Find(start + "Root/Spine/Shoulder_L/ArmUpper_L/ArmLower_L/Hand_L");
		//
		//shoulderRight = GameObject.Find(start + "Root/Spine/Shoulder_R");
		//armUpperRight = GameObject.Find(start + "Root/Spine/Shoulder_R/ArmUpper_R");
		//armLowerRight = GameObject.Find(start + "Root/Spine/Shoulder_R/ArmUpper_R/ArmLower_R");
		//handRight     = GameObject.Find(start + "Root/Spine/Shoulder_R/ArmUpper_R/ArmLower_R/Hand_R");
		//
		//hipLeft      = GameObject.Find(start + "Hip_R");
		//legUpperLeft = GameObject.Find(start + "Hip_R/LegUpper_R");
		//legLowerLeft = GameObject.Find(start + "Hip_R/LegUpper_R/LegLower_R");
		//footLeft     = GameObject.Find(start + "Hip_R/LegUpper_R/LegLower_R/Foot_R");
		//
		//hipRight      = GameObject.Find(start + "Hip_R");
		//legUpperRight = GameObject.Find(start + "Hip_R/LegUpper_R");
		//legLowerRight = GameObject.Find(start + "Hip_R/LegUpper_R/LegLower_R");
		//footRight     = GameObject.Find(start + "Hip_R/LegUpper_R/LegLower_R/Foot_R");
	}

	public void setArmLeftDirection(Vector3 direction)
	{
		setDirection(armUpperLeft, direction);

		/*
		Vector3 eulr = neck.transform.eulerAngles;
		neck.transform.rotation = Quaternion.Euler(eulr.x, eulr.y + 1, eulr.z);
		
		eulr = armUpperRight.transform.eulerAngles;
		//armUpperRight.transform.rotation = Quaternion.Euler(eulr.x, eulr.y + 5, eulr.z);
		
		armUpperRight.transform.rotation = Quaternion.Euler(eulr.x + 2, eulr.y, eulr.z);
		
		legLowerRight.transform.rotation = Quaternion.Euler(eulr.x + 2, eulr.y, eulr.z);
		
		//head.transform.LookAt(Vector3.zero);
		
		Debug.Log(head.transform.position);
		
		//head.transform.Translate(Vector3.up * Time.deltaTime * 0.1f, Space.World);
		*/
	}

	public void setArmLowerLeftDirection(Vector3 direction)
	{
		//Debug.DrawRay(armLowerLeft.transform.position, direction, Color.red);
		//setDirection(armLowerLeft, direction);
	}

	public void setSpineDirection(Vector3 direction)
	{
		setDirection(spine, direction);
	}

	void setDirection(GameObject obj, Vector3 direction)
	{
		obj.transform.LookAt(obj.transform.position + direction);
	}
}
