using UnityEngine;

/// <summary>
/// Attaching this script to an object will make that object face the specified target.
/// The most ideal use for this script is to attach it to the camera and make the camera look at its target.
/// </summary>

[AddComponentMenu("NGUI/Examples/Look At Target")]
public class LookAtTarget : MonoBehaviour
{
	public int level = 0;
	public Transform target;
	public float speed = 8f;
	
	public bool FixZ = false;
	public bool FixPos = false;

	Vector3 mFixPos;
	Transform mTrans;

	void Start ()
	{
		mTrans = transform;
		mFixPos = mTrans.position;
	}

	void LateUpdate ()
	{
		if (target != null)
		{
			Vector3 dir = target.position - mTrans.position;
			float mag = dir.magnitude;

			if (mag > 0.001f)
			{
				Quaternion lookRot = Quaternion.LookRotation(dir);
				if(FixZ)
					lookRot = Quaternion.Euler(0f, lookRot.eulerAngles.y, 0f);
				mTrans.rotation = Quaternion.Slerp(mTrans.rotation, lookRot, Mathf.Clamp01(speed * Time.deltaTime));
			}
		}

		if (FixPos)
			mTrans.position = mFixPos;
	}
}