using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettings 
{
	/// <summary>
	/// This setting determines the game mode for the match.
	/// </summary>
	public static MatchType type
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines if the turn timer is active during the match.
	/// </summary>
	public static bool turnTimer
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines how much time should be allotted per turn if the turn timer is active.
	/// </summary>
	public static float timerSetting
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines how many special abilities each team starts with.
	/// </summary>
	public static int teamSize
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines if special ability stacking is allowed.
	/// </summary>
	public static bool stacking
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines the starting information for each player in the match.
	/// </summary>
	private static List<PlayerSettings> actualPlayerSettings = new List<PlayerSettings> ( );
	public static ReadOnlyCollection<PlayerSettings> playerSettings
	{
		get
		{
			return actualPlayerSettings.AsReadOnly ( );
		}
	}

	/// <summary>
	/// This setting determines the settings for each of the individual heroes.
	/// </summary>
	private static List<HeroSettings> actualHeroSettings = new List<HeroSettings> ( );
	public static ReadOnlyCollection<HeroSettings> heroSettings
	{
		get
		{
			return actualHeroSettings.AsReadOnly ( );
		}
	}
	private static Dictionary<int, HeroSettings> dic = new Dictionary<int, HeroSettings> ( );
	private static ReadOnlyDictionary<int, HeroSettings> readOnlyDic = new ReadOnlyDictionary<int, HeroSettings> ( dic );

	/// <summary>
	/// Sets the match settings.
	/// </summary>
	public static void SetMatchSettings ( MatchType _type, bool _turnTimer = true, float _timer = 90f, int _teamSize = 3, bool _stacking = false, List<HeroSettings> _heroes = null )
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

		// Set hero settings
		if ( _heroes != null )
		{
			// Set custom special settings
			actualHeroSettings.Clear ( );
			actualHeroSettings = _heroes;
			dic.Clear ( );
			foreach ( HeroSettings h in heroSettings )
				dic.Add ( h.id, h );
		}
		else
		{
			// Set default hero settings
			SetDefaultHeroSettings ( );
		}

		// Clear previous player settings
		actualPlayerSettings.Clear ( );

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
			actualPlayerSettings.Add ( cp1 );
			actualPlayerSettings.Add ( cp2 );

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
			actualPlayerSettings.Add ( rp1 );
			actualPlayerSettings.Add ( rp2 );
			actualPlayerSettings.Add ( rp3 );
			actualPlayerSettings.Add ( rp4 );
			actualPlayerSettings.Add ( rp5 );
			actualPlayerSettings.Add ( rp6 );

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
			foreach ( Hero h in HeroInfo.list )
				if ( dic [ h.id ].selection )
					ids.Add ( h.id );

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

	private static void SetDefaultHeroSettings ( )
	{
		actualHeroSettings.Clear ( );
		dic.Clear ( );

		for ( int i = 0; i < HeroInfo.list.Count; i++ )
		{
			HeroSettings h = new HeroSettings ( HeroInfo.list [ i ].id, true, true, (Ability.AbilityType)HeroInfo.list [ i ].ability1.type, HeroInfo.list [ i ].ability1.duration, HeroInfo.list [ i ].ability1.cooldown, true, (Ability.AbilityType)HeroInfo.list [ i ].ability2.type, HeroInfo.list [ i ].ability2.duration, HeroInfo.list [ i ].ability2.cooldown );
			actualHeroSettings.Add ( h );
			dic.Add ( heroSettings [ i ].id, heroSettings [ i ] );
		}
	}

	public static HeroSettings GetHeroSettingsByID ( int id )
	{
		return readOnlyDic [ id ];
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

public class HeroSettings
{
	public int id;
	public bool selection;
	public AbilitySettings ability1;
	public AbilitySettings ability2;

	public HeroSettings ( int _id, bool _selection, bool _enable1, Ability.AbilityType _type1, int _duration1, int _cooldown1, bool _enable2, Ability.AbilityType _type2, int _duration2, int _cooldown2 )
	{
		id = _id;
		selection = _selection;
		ability1 = new AbilitySettings ( _enable1, _type1, _duration1, _cooldown1 );
		ability2 = new AbilitySettings ( _enable2, _type2, _duration2, _cooldown2 );
	}

	public HeroSettings ( int _id, bool _selection, AbilitySettings _ability1, AbilitySettings _ability2 )
	{
		id = _id;
		selection = _selection;
		ability1 = _ability1;
		ability2 = _ability2;
	}
}

public class AbilitySettings
{
	public bool enabled;
	public Ability.AbilityType type;
	public bool active;
	public int duration;
	public int cooldown;

	public AbilitySettings ( bool _enabled, Ability.AbilityType _type, int _duration, int _cooldown )
	{
		enabled = _enabled;
		type = _type;
		active = false;
		duration = _duration;
		cooldown = _cooldown;
	}
}
