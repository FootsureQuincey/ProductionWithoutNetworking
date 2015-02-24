using UnityEngine;
using System.Collections;

public class WeldingtonAutomaton
{
	public string mCharacterName = "Weldington";
	public int mInputAttack = 4;
	public int mInputDefence = 4;
	public int mInputMovement = 4;
	public int mInputRange = 1;
	public Faction mFaction = Faction.Clockwork;
	
	public enum Faction
	{
		SkyPirates,
		Clockwork,
		Tesla,
		Automaton
	};
}
