using UnityEngine;
using System.Collections;

public class AnchorbeardPirate
{

	public string mCharacterName = "Anchorbeard";
	public int mInputAttack = 6;
	public int mInputDefence = 4;
	public int mInputMovement = 4;
	public int mInputRange = 1;
	public Faction mFaction = Faction.SkyPirates;
	
	public enum Faction
	{
		SkyPirates,
		Clockwork,
		Tesla,
		Automaton
	};
}
