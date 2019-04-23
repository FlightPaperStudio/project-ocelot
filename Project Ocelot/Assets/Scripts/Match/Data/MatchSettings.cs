using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MatchSettings 
{
	#region Settings Properties

	/// <summary>
	/// This setting determines the game mode for the match.
	/// </summary>
	public static MatchType Type
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines if the turn timer is active during the match.
	/// </summary>
	public static bool TurnTimer
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines how much time should be allotted per turn if the turn timer is active.
	/// </summary>
	public static float TimePerTurn
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines the number of heroes each team starts with.
	/// </summary>
	public static int HeroesPerTeam
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting determines if dublicate heroes are allowed on the same team.
	/// </summary>
	public static bool HeroLimit
	{
		get;
		private set;
	}

	/// <summary>
	/// This setting is the current debate of the match.
	/// </summary>
	public static Debate MatchDebate
	{
		get;
		private set;
	}

	#endregion // Settings Properties

	#region Settings Data

	public static List<PlayerSettings> Players = new List<PlayerSettings> ( );

	private static UnitSettingData [ ] unitSettings;
	private static Dictionary<int, UnitSettingData> unitSettingsDictionary = new Dictionary<int, UnitSettingData> ( );

	private const int PAWN_UNIT_ID = 1;

	#endregion // Settings Data

	#region Public Static Functions

	/// <summary>
	/// Sets the match settings.
	/// </summary>
	/// <param name="type"> The match type for the match. </param>
	/// <param name="turnTimer"> Whether or not the turn timer is enabled for the match. </param>
	/// <param name="timePerTurn"> The amount of time allotted per turn for the turn timer for the match. </param>
	/// <param name="heroesPerTeam"> The amount of heroes allowed per team for the match. </param>
	/// <param name="heroLimit"> Whether or not duplicate heroes are allowed per team for the match. </param>
	/// <param name="heroes"> The custom hero settings for the match. </param>
	public static void SetMatchSettings ( MatchType type, bool turnTimer = true, float timePerTurn = 90f, int heroesPerTeam = 3, bool heroLimit = true, List<UnitSettingData> heroes = null )
	{
		// Set the match type
		Type = type;

		// Set the turn timer
		TurnTimer = turnTimer;
		TimePerTurn = timePerTurn;

		// Set the heroes per team
		HeroesPerTeam = heroesPerTeam;

		// Set the hero limit
		HeroLimit = heroLimit;

		// Set the match debate
		MatchDebate = DebateGenerator.GetRandomDebate ( Type );

		// Set the units
		SetUnitSettings ( heroes );

		// Set the players
		SetPlayerSettings ( );

		//// Set hero settings
		//if ( _heroes != null )
		//{
		//	// Set custom special settings
		//	actualHeroSettings.Clear ( );
		//	actualHeroSettings = _heroes;
		//	dic.Clear ( );
		//	foreach ( HeroSettings h in heroSettings )
		//		dic.Add ( h.id, h );
		//}
		//else
		//{
		//	// Set default hero settings
		//	SetDefaultHeroSettings ( );
		//}

		//// Clear previous player settings
		//Players.Clear ( );

		//// Check match type for adding players
		//switch ( Type )
		//{
		//// Two player game modes
		//case MatchType.Classic:
		//case MatchType.Mirror:
		//case MatchType.CustomClassic:
		//case MatchType.CustomMirror:

		//	// Initialize two players
		//	PlayerSettings cp1 = new PlayerSettings ( Player.TeamColor.BLUE, Player.Direction.LEFT_TO_RIGHT, Player.PlayerControl.LOCAL_PLAYER );
		//	PlayerSettings cp2 = new PlayerSettings ( Player.TeamColor.ORANGE, Player.Direction.RIGHT_TO_LEFT, Player.PlayerControl.LOCAL_PLAYER );

		//	// Set player names
		//	cp1.PlayerName = "Blue Team";
		//	cp2.PlayerName = "Orange Team";

		//	// Add players
		//	Players.Add ( cp1 );
		//	Players.Add ( cp2 );

		//	break;

		//// Six player game modes
		//case MatchType.Rumble:
		//case MatchType.CustomRumble:

		//	// Initialize six players
		//	PlayerSettings rp1 = new PlayerSettings ( Player.TeamColor.BLUE, Player.Direction.LEFT_TO_RIGHT, Player.PlayerControl.LOCAL_PLAYER );
		//	PlayerSettings rp2 = new PlayerSettings ( Player.TeamColor.GREEN, Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT, Player.PlayerControl.LOCAL_PLAYER );
		//	PlayerSettings rp3 = new PlayerSettings ( Player.TeamColor.YELLOW, Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT, Player.PlayerControl.LOCAL_PLAYER );
		//	PlayerSettings rp4 = new PlayerSettings ( Player.TeamColor.ORANGE, Player.Direction.RIGHT_TO_LEFT, Player.PlayerControl.LOCAL_PLAYER );
		//	PlayerSettings rp5 = new PlayerSettings ( Player.TeamColor.PINK, Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT, Player.PlayerControl.LOCAL_PLAYER );
		//	PlayerSettings rp6 = new PlayerSettings ( Player.TeamColor.PURPLE, Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT, Player.PlayerControl.LOCAL_PLAYER );

		//	// Set player names
		//	rp1.PlayerName = "Blue Team";
		//	rp2.PlayerName = "Green Team";
		//	rp3.PlayerName = "Yellow Team";
		//	rp4.PlayerName = "Orange Team";
		//	rp5.PlayerName = "Pink Team";
		//	rp6.PlayerName = "Purple Team";

		//	// Add players
		//	Players.Add ( rp1 );
		//	Players.Add ( rp2 );
		//	Players.Add ( rp3 );
		//	Players.Add ( rp4 );
		//	Players.Add ( rp5 );
		//	Players.Add ( rp6 );

		//	break;
		//}

		//// Set teams and formations to empty
		//foreach ( PlayerSettings p in playerSettings )
		//{
		//	// Clear any previous heroes
		//	p.heroIDs.Clear ( );

		//	// Set formation to default
		//	p.Formation = new int [ TEAM_SIZE ] { LEADER_UNIT, NO_UNIT, NO_UNIT, NO_UNIT, NO_UNIT, NO_UNIT };
		//}

		//// Check for match types with randomly assigned teams and formations
		//if ( Type == MatchType.Mirror || Type == MatchType.CustomMirror )
		//{
		//	// Store the information for randomly assigning specials
		//	List<int> ids = new List<int> ( );
		//	foreach ( Hero h in HeroInfo.list )
		//		if ( dic [ h.ID ].selection )
		//			ids.Add ( h.ID );

		//	// Store the information for randomaly assigning starting positions
		//	List<int> pos = new List<int>
		//	{
		//		1,
		//		2,
		//		3,
		//		4,
		//		5
		//	};

		//	// Randomly assign the same team selection and team formation to each team
		//	int slots = 1;
		//	for ( int i = 0; i < HeroesPerTeam; i++ )
		//	{
		//		// Grab a random hero
		//		int h = ids [ Random.Range ( 0, ids.Count ) ];

		//		// Occupy the hero's slots
		//		slots += HeroInfo.GetHeroByID ( h ).Slots;

		//		// Grab a random position
		//		int f = pos [ Random.Range ( 0, pos.Count ) ];

		//		// Assign the special and position to each team
		//		foreach ( PlayerSettings p in playerSettings )
		//		{
		//			// Assign the special
		//			p.heroIDs.Add ( h );

		//			// Assign the postion
		//			p.Formation [ f ] = h;
		//		}

		//		// Remove the hero from the list
		//		if ( !HeroLimit )
		//			ids.Remove ( h );

		//		// Remove any heroes that occupy more slots than the remaining number
		//		List<int> overSlotLimit = new List<int> ( );
		//		for ( int j = 0; j < ids.Count; j++ )
		//			if ( slots + HeroInfo.GetHeroByID ( ids [ j ] ).Slots > TEAM_SIZE )
		//				overSlotLimit.Add ( ids [ j ] );
		//		for ( int j = 0; j < overSlotLimit.Count; j++ )
		//			ids.Remove ( overSlotLimit [ j ] );

		//		// Remove the postion from the list
		//		pos.Remove ( f );

		//		// Check for remaining slots
		//		if ( slots == TEAM_SIZE )
		//			break;
		//	}

		//	// Randomly assign pawns for the remaining slots
		//	for ( int i = 0; i < TEAM_SIZE - slots; i++ )
		//	{
		//		// Grab a random position
		//		int f = pos [ Random.Range ( 0, pos.Count ) ];

		//		// Assign the special and position to each team
		//		foreach ( PlayerSettings p in playerSettings )
		//		{
		//			// Assign the postion
		//			p.Formation [ f ] = PAWN_UNIT;
		//		}

		//		// Remove the postion from the list
		//		pos.Remove ( f );
		//	}
		//}
	}

	/// <summary>
	/// Gets the read only data for a unit.
	/// </summary>
	/// <param name="id"> The ID of the unit. </param>
	/// <returns> The read only unit data. </returns>
	public static IReadOnlyUnitData GetUnitData ( int id )
	{
		// Return the read only data
		return unitSettingsDictionary [ id ];
	}

	/// <summary>
	/// Gets the read only data for an ability.
	/// </summary>
	/// <param name="unitID"> The ID of the unit. </param>
	/// <param name="abilityPosition"> The order position of the ability. </param>
	/// <returns> The read only ability data. </returns>
	public static IReadOnlyAbilityData GetAbilityData ( int unitID, int abilityPosition )
	{
		// Check the ability position
		switch ( abilityPosition )
		{
		default:
		case 1:
			// Return the read only data for ability 1
			return unitSettingsDictionary [ unitID ].Ability1;
		case 2:
			// Return the read only data for ability 2
			return unitSettingsDictionary [ unitID ].Ability2;
		case 3:
			// Return the read only data for ability 3
			return unitSettingsDictionary [ unitID ].Ability3;
		}
	}

	/// <summary>
	/// Gets a new instance of leader data based on the unit settings for the match.
	/// </summary>
	/// <param name="team"> The team of the new leader. </param>
	/// <returns> A new instance of leader data. </returns>
	public static UnitSettingData GetLeader ( Player.TeamColor team )
	{
		// Create a new instance of a leader
		UnitSettingData leader = new UnitSettingData
		{
			ID = unitSettingsDictionary [ (int)team + 2 ].ID,
			UnitName = unitSettingsDictionary [ (int)team + 2 ].UnitName,
			UnitNickname = unitSettingsDictionary [ (int)team + 2 ].UnitNickname,
			UnitBio = unitSettingsDictionary [ (int)team + 2 ].UnitBio,
			FinishingMove = unitSettingsDictionary [ (int)team + 2 ].FinishingMove,
			Portrait = unitSettingsDictionary [ (int)team + 2 ].Portrait,
			Role = unitSettingsDictionary [ (int)team + 2 ].Role,
			Slots = unitSettingsDictionary [ (int)team + 2 ].Slots,
			IsEnabled = unitSettingsDictionary [ (int)team + 2 ].IsEnabled
		};

		// Get leader ability data
		leader.InitializeAbilities ( new AbilityData [ ] { unitSettingsDictionary [ (int)team + 2 ].Ability1, unitSettingsDictionary [ (int)team + 2 ].Ability2, unitSettingsDictionary [ (int)team + 2 ].Ability3 } );

		// Return team leader
		return leader;
	}

	/// <summary>
	/// Gets a new instance of hero data based on the unit settings for the match.
	/// </summary>
	/// <param name="id"> The ID of the hero. </param>
	/// <returns> A new instance of hero data. </returns>
	public static UnitSettingData GetHero ( int id )
	{
		// Create a new instance of a leader
		UnitSettingData hero = new UnitSettingData
		{
			ID = unitSettingsDictionary [ id ].ID,
			UnitName = unitSettingsDictionary [ id ].UnitName,
			UnitNickname = unitSettingsDictionary [ id ].UnitNickname,
			UnitBio = unitSettingsDictionary [ id ].UnitBio,
			FinishingMove = unitSettingsDictionary [ id ].FinishingMove,
			Portrait = unitSettingsDictionary [ id ].Portrait,
			Role = unitSettingsDictionary [ id ].Role,
			Slots = unitSettingsDictionary [ id ].Slots,
			IsEnabled = unitSettingsDictionary [ id ].IsEnabled
		};

		// Get leader ability data
		hero.InitializeAbilities ( new AbilityData [ ] { unitSettingsDictionary [ id ].Ability1, unitSettingsDictionary [ id ].Ability2, unitSettingsDictionary [ id ].Ability3 } );

		// Return team leader
		return hero;
	}

	/// <summary>
	/// Gets a new instance of pawn data.
	/// </summary>
	/// <returns> A new instance of pawn data. </returns>
	public static UnitSettingData GetPawn ( )
	{
		// Generate random character.
		CNG.Character character = CNG.NameGenerator.GetCharacter ( );

		// Create a new instance of a leader
		UnitSettingData pawn = new UnitSettingData
		{
			ID = unitSettingsDictionary [ PAWN_UNIT_ID ].ID,
			UnitName = character.WesternNameOrder,//NameGenerator.CreateName ( ),
			UnitNickname = character.QuotedNickname,//NameGenerator.CreateNickname ( ),
			UnitBio = unitSettingsDictionary [ PAWN_UNIT_ID ].UnitBio,
			FinishingMove = unitSettingsDictionary [ PAWN_UNIT_ID ].FinishingMove,
			Portrait = unitSettingsDictionary [ PAWN_UNIT_ID ].Portrait,
			Role = unitSettingsDictionary [ PAWN_UNIT_ID ].Role,
			Slots = unitSettingsDictionary [ PAWN_UNIT_ID ].Slots,
			IsEnabled = unitSettingsDictionary [ PAWN_UNIT_ID ].IsEnabled
		};

		// Return team leader
		return pawn;
	}

	/// <summary>
	/// Assigns each player the exact same units and formation for a Mirror Match.
	/// </summary>
	public static void SetMirrorUnits ( )
	{
		// Get a list of available heroes and formations
		List<UnitSettingData> availableHeroes = unitSettings.Where ( x => x.IsEnabled && ( x.Role == UnitData.UnitRole.OFFENSE || x.Role == UnitData.UnitRole.DEFENSE || x.Role == UnitData.UnitRole.SUPPORT ) ).ToList ( );
		List<int> availableFormations = new List<int> { 1, 2, 3, 4, 5 };

		// Store the units and formations
		List<int> unitIDs = new List<int> ( );
		List<int> formation = new List<int> ( );

		// Set the number of slots
		int slotCounter = 5;

		// Get the number of heroes per team
		for ( int i = 0; i < HeroesPerTeam; i++ )
		{
			// Get the index of a random hero
			int heroIndex = Random.Range ( 0, availableHeroes.Count );

			// Get the index of a random formation
			int formationIndex = Random.Range ( 0, availableFormations.Count );
			Debug.Log ( "Hero: " + availableHeroes [ heroIndex ].UnitName + " Formation: " + availableFormations [ formationIndex ] );
			// Add unit and formation
			unitIDs.Add ( availableHeroes [ heroIndex ].ID );
			formation.Add ( availableFormations [ formationIndex ] );

			// Decrement slots
			slotCounter -= availableHeroes [ heroIndex ].Slots;

			// Remove unit and formation from availability
			if ( HeroLimit )
				availableHeroes.RemoveAt ( heroIndex );
			availableFormations.RemoveAt ( formationIndex );

			// Remove any heroes who are over the slot limit
			availableHeroes = availableHeroes.Where ( x => x.Slots <= slotCounter ).ToList ( );
		}

		// Get formations for the remaining pawns
		for ( int i = 0; i < slotCounter; i++ )
		{
			// Get the index of a random formation
			int formationIndex = Random.Range ( 0, availableFormations.Count );
			Debug.Log ( "Pawn Formation: " + availableFormations [ formationIndex ] );
			// Add formation
			formation.Add ( availableFormations [ formationIndex ] );

			// Remove formation from availability
			availableFormations.RemoveAt ( formationIndex );
		}

		// Set units to teams
		for ( int i = 0; i < Players.Count; i++ )
		{
			// Add units
			for ( int j = 0; j < formation.Count; j++ )
			{
				UnitSettingData unit = j < unitIDs.Count ? GetHero ( unitIDs [ j ] ) : GetPawn ( );
				Players [ i ].Units.Add ( unit );
				Players [ i ].UnitFormation.Add ( unit, formation [ j ] );
				Debug.Log ( "Adding " + unit.UnitName + " at postion " + formation [ j ] );
			}
		}
	}

	#endregion // Public Static Functions

	#region Private Static Functions

	/// <summary>
	/// Sets the unit settings for the match.
	/// </summary>
	/// <param name="heroes"> A list of any custom hero settings. </param>
	private static void SetUnitSettings ( List<UnitSettingData> heroes )
	{
		// Get all default unit data
		unitSettings = UnitDatabase.GetUnits ( );
		//unitSettings = UnitDataStorage.GetUnits ( );
		unitSettingsDictionary.Clear ( );

		// Set the unit setting for each unit
		for ( int unitIndex = 0, heroIndex = 0; unitIndex < unitSettings.Length; unitIndex++ )
		{
			// Check for custom hero settings
			if ( heroes != null && ( unitSettings [ unitIndex ].Role == UnitData.UnitRole.OFFENSE || unitSettings [ unitIndex ].Role == UnitData.UnitRole.DEFENSE || unitSettings [ unitIndex ].Role == UnitData.UnitRole.SUPPORT ) )
			{
				// Update the unit setting with the custom match setting
				unitSettings [ unitIndex ].IsEnabled = heroes [ heroIndex ].IsEnabled;

				// Update the ability 1 setting with the custom match setting
				if ( unitSettings [ unitIndex ].Ability1 != null )
				{
					unitSettings [ unitIndex ].Ability1.IsEnabled = heroes [ heroIndex ].Ability1.IsEnabled;
					unitSettings [ unitIndex ].Ability1.Duration = heroes [ heroIndex ].Ability1.Duration;
					unitSettings [ unitIndex ].Ability1.Cooldown = heroes [ heroIndex ].Ability1.Cooldown;
				}

				// Update the ability 2 setting with the custom match setting
				if ( unitSettings [ unitIndex ].Ability2 != null )
				{
					unitSettings [ unitIndex ].Ability2.IsEnabled = heroes [ heroIndex ].Ability2.IsEnabled;
					unitSettings [ unitIndex ].Ability2.Duration = heroes [ heroIndex ].Ability2.Duration;
					unitSettings [ unitIndex ].Ability2.Cooldown = heroes [ heroIndex ].Ability2.Cooldown;
				}

				// Update the ability 3 setting with the custom match setting
				if ( unitSettings [ unitIndex ].Ability3 != null )
				{
					unitSettings [ unitIndex ].Ability3.IsEnabled = heroes [ heroIndex ].Ability3.IsEnabled;
					unitSettings [ unitIndex ].Ability3.Duration = heroes [ heroIndex ].Ability3.Duration;
					unitSettings [ unitIndex ].Ability3.Cooldown = heroes [ heroIndex ].Ability3.Cooldown;
				}

				// Increment the hero index
				heroIndex++;
			}
			// Check for partial heroes to apply the custom hero settings to 
			else if ( heroes != null && unitSettings [ unitIndex ].Role == UnitData.UnitRole.PARTIAL )
			{
				// Update the unit setting with the custom match setting
				unitSettings [ unitIndex ].IsEnabled = heroes [ heroIndex - 1 ].IsEnabled;

				// Update the ability 1 setting with the custom match setting
				if ( unitSettings [ unitIndex ].Ability1 != null )
				{
					unitSettings [ unitIndex ].Ability1.IsEnabled = heroes [ heroIndex - 1 ].Ability1.IsEnabled;
					unitSettings [ unitIndex ].Ability1.Duration = heroes [ heroIndex - 1 ].Ability1.Duration;
					unitSettings [ unitIndex ].Ability1.Cooldown = heroes [ heroIndex - 1 ].Ability1.Cooldown;
				}

				// Update the ability 2 setting with the custom match setting
				if ( unitSettings [ unitIndex ].Ability2 != null )
				{
					unitSettings [ unitIndex ].Ability2.IsEnabled = heroes [ heroIndex - 1 ].Ability2.IsEnabled;
					unitSettings [ unitIndex ].Ability2.Duration = heroes [ heroIndex - 1 ].Ability2.Duration;
					unitSettings [ unitIndex ].Ability2.Cooldown = heroes [ heroIndex - 1].Ability2.Cooldown;
				}

				// Update the ability 3 setting with the custom match setting
				if ( unitSettings [ unitIndex ].Ability3 != null )
				{
					unitSettings [ unitIndex ].Ability3.IsEnabled = heroes [ heroIndex - 1 ].Ability3.IsEnabled;
					unitSettings [ unitIndex ].Ability3.Duration = heroes [ heroIndex - 1 ].Ability3.Duration;
					unitSettings [ unitIndex ].Ability3.Cooldown = heroes [ heroIndex - 1 ].Ability3.Cooldown;
				}
			}

			// Add unit to the dictionary
			unitSettingsDictionary.Add ( unitSettings [ unitIndex ].ID, unitSettings [ unitIndex ] );
		}
	}

	/// <summary>
	/// Sets the players for the match.
	/// </summary>
	private static void SetPlayerSettings ( )
	{
		// Remove any previous players
		Players.Clear ( );

		// Determine the number of players
		int playerTotal = 0;
		switch ( Type )
		{
		case MatchType.Classic:
		case MatchType.CustomClassic:
		case MatchType.Mirror:
		case MatchType.CustomMirror:
			playerTotal = 2;
			break;
		case MatchType.Ladder:
		case MatchType.CustomLadder:
			playerTotal = 3;
			break;
		case MatchType.Rumble:
		case MatchType.CustomRumble:
			playerTotal = 6;
			break;
		}

		// Add player settings
		for ( int i = 0; i < playerTotal; i++ )
		{
			Players.Add ( new PlayerSettings
			{
				PlayerName = "Player " + ( i + 1 ).ToString ( ),
				TurnOrder = i + 1,
				Team = Player.TeamColor.NO_TEAM,
				TeamDirection = GetPlayerDirection ( i + 1 ),
				Control = Player.PlayerControl.LOCAL_PLAYER
			} );
		}
	}

	/// <summary>
	/// Gets the direction the player will be going in the match.
	/// </summary>
	/// <param name="turnOrder"> The player's turn order. </param>
	/// <returns> The direction of the player. </returns>
	private static Player.Direction GetPlayerDirection ( int turnOrder )
	{
		// Check match type
		if ( Type == MatchType.Classic || Type == MatchType.CustomClassic || Type == MatchType.Mirror || Type == MatchType.CustomMirror )
		{
			if ( turnOrder == 1 )
				return Player.Direction.LEFT_TO_RIGHT;
			else if ( turnOrder == 2 )
				return Player.Direction.RIGHT_TO_LEFT;
		}
		else if ( Type == MatchType.Ladder || Type == MatchType.CustomLadder )
		{
			if ( turnOrder == 1 )
				return Player.Direction.LEFT_TO_RIGHT;
			else if ( turnOrder == 2 )
				return Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT;
			else if ( turnOrder == 3 )
				return Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT;
		}
		else if ( Type == MatchType.Rumble || Type == MatchType.CustomRumble )
		{
			if ( turnOrder == 1 )
				return Player.Direction.LEFT_TO_RIGHT;
			else if ( turnOrder == 2 )
				return Player.Direction.TOP_LEFT_TO_BOTTOM_RIGHT;
			else if ( turnOrder == 3 )
				return Player.Direction.TOP_RIGHT_TO_BOTTOM_LEFT;
			else if ( turnOrder == 4 )
				return Player.Direction.RIGHT_TO_LEFT;
			else if ( turnOrder == 5 )
				return Player.Direction.BOTTOM_RIGHT_TO_TOP_LEFT;
			else if ( turnOrder == 6 )
				return Player.Direction.BOTTOM_LEFT_TO_TOP_RIGHT;
		}

		// Return player 1 by default
		return Player.Direction.LEFT_TO_RIGHT;
	}

	#endregion // Private Static Functions

	
	/// <summary>
	/// This setting determines the starting information for each player in the match.
	/// </summary>
	
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
