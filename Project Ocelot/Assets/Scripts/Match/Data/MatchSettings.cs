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

	public static Debate MatchDebate
	{
		get;
		private set;
	}

	private static UnitDefaultData [ ] unitSettings;
	private static Dictionary<int, UnitDefaultData> unitSettingsDictionary = new Dictionary<int, UnitDefaultData> ( );

	public static UnitDefaultData GetUnitSetting ( int id )
	{
		return unitSettingsDictionary [ id ];
	}

	/// <summary>
	/// This setting determines the starting information for each player in the match.
	/// </summary>
	public static List<PlayerSettings> Players = new List<PlayerSettings> ( );
	public static ReadOnlyCollection<PlayerSettings> playerSettings
	{
		get
		{
			return Players.AsReadOnly ( );
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

	public const int TEAM_SIZE = 6;
	public const int NO_UNIT = -2;
	public const int LEADER_UNIT = -1;
	public const int PAWN_UNIT = 0;

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

		MatchDebate = DebateGenerator.GetRandomDebate ( type );

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
		Players.Clear ( );

		// Check match type for adding players
		switch ( type )
		{
		// Two player game modes
		case MatchType.Classic:
		case MatchType.Mirror:
		case MatchType.CustomClassic:
		case MatchType.CustomMirror:

			// Initialize two players
			PlayerSettings cp1 = new PlayerSettings ( Player.TeamColor.BLUE,   Player.Direction.LEFT_TO_RIGHT, Player.PlayerControl.LOCAL_PLAYER );
			PlayerSettings cp2 = new PlayerSettings ( Player.TeamColor.ORANGE, Player.Direction.RIGHT_TO_LEFT, Player.PlayerControl.LOCAL_PLAYER );

			// Set player names
			cp1.PlayerName = "Blue Team";
			cp2.PlayerName = "Orange Team";

			// Add players
			Players.Add ( cp1 );
			Players.Add ( cp2 );

			break;

		// Six player game modes
		case MatchType.Rumble:
		case MatchType.CustomRumble:

			// Initialize six players
			PlayerSettings rp1 = new PlayerSettings ( Player.TeamColor.BLUE,   Player.Direction.LEFT_TO_RIGHT,          Player.PlayerControl.LOCAL_PLAYER );
			PlayerSettings rp2 = new PlayerSettings ( Player.TeamColor.GREEN,  Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT, Player.PlayerControl.LOCAL_PLAYER );
			PlayerSettings rp3 = new PlayerSettings ( Player.TeamColor.YELLOW, Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT, Player.PlayerControl.LOCAL_PLAYER );
			PlayerSettings rp4 = new PlayerSettings ( Player.TeamColor.ORANGE, Player.Direction.RIGHT_TO_LEFT,          Player.PlayerControl.LOCAL_PLAYER );
			PlayerSettings rp5 = new PlayerSettings ( Player.TeamColor.PINK,   Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT, Player.PlayerControl.LOCAL_PLAYER );
			PlayerSettings rp6 = new PlayerSettings ( Player.TeamColor.PURPLE, Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT, Player.PlayerControl.LOCAL_PLAYER );

			// Set player names
			rp1.PlayerName = "Blue Team";
			rp2.PlayerName = "Green Team";
			rp3.PlayerName = "Yellow Team";
			rp4.PlayerName = "Orange Team";
			rp5.PlayerName = "Pink Team";
			rp6.PlayerName = "Purple Team";

			// Add players
			Players.Add ( rp1 );
			Players.Add ( rp2 );
			Players.Add ( rp3 );
			Players.Add ( rp4 );
			Players.Add ( rp5 );
			Players.Add ( rp6 );

			break;
		}

		// Set teams and formations to empty
		foreach ( PlayerSettings p in playerSettings )
		{
			// Clear any previous heroes
			p.heroIDs.Clear ( );

			// Set formation to default
			p.Formation = new int [ TEAM_SIZE ] { LEADER_UNIT, NO_UNIT, NO_UNIT, NO_UNIT, NO_UNIT, NO_UNIT };
		}

		// Check for match types with randomly assigned teams and formations
		if ( type == MatchType.Mirror || type == MatchType.CustomMirror )
		{
			// Store the information for randomly assigning specials
			List<int> ids = new List<int> ( );
			foreach ( Hero h in HeroInfo.list )
				if ( dic [ h.ID ].selection )
					ids.Add ( h.ID );

			// Store the information for randomaly assigning starting positions
			List<int> pos = new List<int>
			{
				1,
				2,
				3,
				4,
				5
			};

			// Randomly assign the same team selection and team formation to each team
			int slots = 1;
			for ( int i = 0; i < teamSize; i++ )
			{
				// Grab a random hero
				int h = ids [ Random.Range ( 0, ids.Count ) ];

				// Occupy the hero's slots
				slots += HeroInfo.GetHeroByID ( h ).Slots;

				// Grab a random position
				int f = pos [ Random.Range ( 0, pos.Count ) ];

				// Assign the special and position to each team
				foreach ( PlayerSettings p in playerSettings )
				{
					// Assign the special
					p.heroIDs.Add ( h );

					// Assign the postion
					p.Formation [ f ] = h;
				}

				// Remove the hero from the list
				if ( !stacking )
					ids.Remove ( h );

				// Remove any heroes that occupy more slots than the remaining number
				List<int> overSlotLimit = new List<int> ( );
				for ( int j = 0; j < ids.Count; j++ )
					if ( slots + HeroInfo.GetHeroByID ( ids [ j ] ).Slots > TEAM_SIZE )
						overSlotLimit.Add ( ids [ j ] );
				for ( int j = 0; j < overSlotLimit.Count; j++ )
					ids.Remove ( overSlotLimit [ j ] );

				// Remove the postion from the list
				pos.Remove ( f );

				// Check for remaining slots
				if ( slots == TEAM_SIZE )
					break;
			}

			// Randomly assign pawns for the remaining slots
			for ( int i = 0; i < TEAM_SIZE - slots; i++ )
			{
				// Grab a random position
				int f = pos [ Random.Range ( 0, pos.Count ) ];

				// Assign the special and position to each team
				foreach ( PlayerSettings p in playerSettings )
				{
					// Assign the postion
					p.Formation [ f ] = PAWN_UNIT;
				}

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
			HeroSettings h = new HeroSettings ( HeroInfo.list [ i ].ID, true, true, (Ability.AbilityType)HeroInfo.list [ i ].Ability1.Type, HeroInfo.list [ i ].Ability1.Duration, HeroInfo.list [ i ].Ability1.Cooldown, true, (Ability.AbilityType)HeroInfo.list [ i ].Ability2.Type, HeroInfo.list [ i ].Ability2.Duration, HeroInfo.list [ i ].Ability2.Cooldown );
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
	Ladder,
	Rumble,
	CustomClassic,
	CustomMirror,
	CustomLadder,
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
