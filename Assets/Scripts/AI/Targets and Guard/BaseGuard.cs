using UnityEngine;
using System.Collections;

public class BaseGuard : MonoBehaviour
{
	enum mAbility
	{
		None,
		GoldLost,
		InfamyLost,
		StatLost,
		EquipLost
	};
	public int mAttack;

	public int mMovement;
	public int mRunMovement;

	public int mGold;

	public int mDetectionRun;
	public int mDetectionWalk;
	public int mDetectionSee;

	public int mRange;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void Attack()
	{
		//attack something
	}
}
