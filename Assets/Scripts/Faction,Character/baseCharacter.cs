//Created by Jack ng
//Nov 1st 2014 
//Updated by Jack Ng
//Jan 8th, 2015


public class baseCharacter
{
	public string mCharacterName;
	public int mInputAttack;
	public int mInputDefence;
	public int mInputMovement;
	public int mInputRange;
	public Faction mFaction;

	public enum Faction
	{
		SkyPirates,
		Clockwork,
		Tesla,
		Automaton
	};
}
