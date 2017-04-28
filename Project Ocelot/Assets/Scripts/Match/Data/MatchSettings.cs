using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings 
{
	/// <summary>
	/// This setting determines the game mode for the match.
	/// </summary>
	public static MatchType type;

	/// <summary>
	/// This setting determines if the turn timer is active during the match.
	/// </summary>
	public static bool turnTimer;

	/// <summary>
	/// This setting determines how much time should be allotted per turn if the turn timer is active.
	/// </summary>
	public static float timerSetting;

	/// <summary>
	/// This setting determines how many special abilities each team starts with.
	/// </summary>
	public static int teamSize;

	/// <summary>
	/// This setting determines if special ability stacking is allowed.
	/// </summary>
	public static bool stacking;

	/// <summary>
	/// This setting determines the starting information for each player in the match.
	/// </summary>
	public static List<PlayerSettings> playerSettings = new List<PlayerSettings> ( );

	/// <summary>
	/// This setting determines the settings for each of the individual special abilities.
	/// </summary>
	public static List<SpecialSettings> specialSettings = new List<SpecialSettings> ( );
	private static Dictionary<int, SpecialSettings> dic = new Dictionary<int, SpecialSettings> ( );

	/// <summary>
	/// Sets the match settings.
	/// </summary>
	public static void SetMatchSettings ( MatchType _type, bool _turnTimer = true, float _timer = 90f, int _teamSize = 3, bool _stacking = false, List<SpecialSettings> _special = null )
	{
		// Set the match type
		type = _type;

		// Set timer
		turnTimer = _turnTimer;
		timerSetting = _timer;

		// Set the team size
		teamSize = _teamSize;

		// Set stacking
		stacking = _stacking;

		// Set special abilities
		if ( _special != null )
		{
			// Set custom special settings
			specialSettings = _special;
			dic.Clear ( );
			foreach ( SpecialSettings s in specialSettings )
				dic.Add ( s.id, s );
		}
		else
		{
			// Set default special settings
			SetDefaultSpecialSettings ( );
		}

		// Clear previous player settings
		playerSettings.Clear ( );

		// Check match type for adding players
		switch ( type )
		{
		// Two player game modes
		case MatchType.Classic:
		case MatchType.Mirror:
		case MatchType.CustomClassic:
		case MatchType.CustomMirror:

			// Initialize two players
			PlayerSettings cp1 = new PlayerSettings ( Player.TeamColor.Blue,   Player.Direction.LeftToRight, Player.PlayerControl.LocalHuman );
			PlayerSettings cp2 = new PlayerSettings ( Player.TeamColor.Orange, Player.Direction.RightToLeft, Player.PlayerControl.LocalHuman );

			// Set player names
			cp1.name = "Blue Team";
			cp2.name = "Orange Team";

			// Add players
			playerSettings.Add ( cp1 );
			playerSettings.Add ( cp2 );

			break;

		// Six player game modes
		case MatchType.Rumble:
		case MatchType.CustomRumble:

			// Initialize six players
			PlayerSettings rp1 = new PlayerSettings ( Player.TeamColor.Blue,   Player.Direction.LeftToRight,          Player.PlayerControl.LocalHuman );
			PlayerSettings rp2 = new PlayerSettings ( Player.TeamColor.Green,  Player.Direction.TopLeftToBottomRight, Player.PlayerControl.LocalHuman );
			PlayerSettings rp3 = new PlayerSettings ( Player.TeamColor.Yellow, Player.Direction.TopRightToBottomLeft, Player.PlayerControl.LocalHuman );
			PlayerSettings rp4 = new PlayerSettings ( Player.TeamColor.Orange, Player.Direction.RightToLeft,          Player.PlayerControl.LocalHuman );
			PlayerSettings rp5 = new PlayerSettings ( Player.TeamColor.Pink,   Player.Direction.BottomRightToTopLeft, Player.PlayerControl.LocalHuman );
			PlayerSettings rp6 = new PlayerSettings ( Player.TeamColor.Purple, Player.Direction.BottomLeftToTopRight, Player.PlayerControl.LocalHuman );

			// Set player names
			rp1.name = "Blue Team";
			rp2.name = "Green Team";
			rp3.name = "Yellow Team";
			rp4.name = "Orange Team";
			rp5.name = "Pink Team";
			rp6.name = "Purple Team";

			// Add players
			playerSettings.Add ( rp1 );
			playerSettings.Add ( rp2 );
			playerSettings.Add ( rp3 );
			playerSettings.Add ( rp4 );
			playerSettings.Add ( rp5 );
			playerSettings.Add ( rp6 );

			break;
		}

		// Set teams and formations to empty
		foreach ( PlayerSettings p in playerSettings )
		{
			// Set specials to blank
			p.specialIDs.Clear ( );
			for ( int i = 0; i < teamSize; i++ )
				p.specialIDs.Add ( 0 );

			// Set formation to default
			p.formation = new int [ 6 ] { -1, 0, 0, 0, 0, 0 };
		}

		// Check for match types with randomly assigned teams and formations
		if ( type == MatchType.Mirror || type == MatchType.CustomMirror )
		{
			// Store the information for randomly assigning specials
			List<int> ids = new List<int> ( );
			foreach ( Special s in SpecialInfo.list )
				if ( dic [ s.id ].selection )
					ids.Add ( s.id );

			// Store the information for randomaly assigning starting positions
			List<int> pos = new List<int> ( );
			pos.Add ( 1 ); pos.Add ( 2 ); pos.Add ( 3 ); pos.Add ( 4 ); pos.Add ( 5 );

			// Randomly assign the same team selection and team formation to each team
			for ( int i = 0; i < teamSize; i++ )
			{
				// Grab a random special
				int s = ids [ Random.Range ( 0, ids.Count ) ];

				// Grab a random position
				int f = pos [ Random.Range ( 0, pos.Count ) ];

				// Assign the special and position to each team
				foreach ( PlayerSettings p in playerSettings )
				{
					// Assign the special
					p.specialIDs [ i ] = s;

					// Assign the postion
					p.formation [ f ] = s;
				}

				// Remove the special from the list
				if ( !stacking )
					ids.Remove ( s );

				// Remove the postion from the list
				pos.Remove ( f );
			}
		}
	}

	private static void SetDefaultSpecialSettings ( )
	{
		specialSettings.Clear ( );
		dic.Clear ( );

		for ( int i = 0; i < SpecialInfo.list.Length; i++ )
		{
			SpecialSettings s = new SpecialSettings ( SpecialInfo.list [ i ].id, true, SpecialInfo.list [ i ].cooldown );
			specialSettings.Add ( s );
			dic.Add ( specialSettings [ i ].id, specialSettings [ i ] );
		}
	}

	public static SpecialSettings GetSpecialSettingsByID ( int id )
	{
		return dic [ id ];
	}
}

public enum MatchType
{
	Classic,
	Mirror,
	Rumble,
	CustomClassic,
	CustomMirror,
	CustomRumble
}

public class SpecialSettings
{
	public int id;
	public bool selection;
	public int cooldown;

	public SpecialSettings ( int _id = 0, bool _selection = true, int _cooldown = 0 )
	{
		id = _id;
		selection = _selection;
		cooldown = _cooldown;
	}
}
