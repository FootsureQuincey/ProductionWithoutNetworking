using UnityEngine;
using System.Collections;

public class ThordrannDwarf
{

	public string mCharacterName = "Thordrann";
	public int mInputAttack = 4;
	public int mInputDefence = 6;
	public int mInputMovement = 5;
	public int mInputRange = 1;
	public Faction mFaction = Faction.Tesla;
	
	public enum Faction
	{
		SkyPirates,
		Clockwork,
		Tesla,
		Automaton
	};
}
