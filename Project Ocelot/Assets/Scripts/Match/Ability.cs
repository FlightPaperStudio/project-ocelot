using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Ability 
{
//	//Ability information
//	public int ID;
//	public string Name;
//	public string Desc;
//	public bool IsActive = true;
//	public bool AttachesToPiece;
//	public PieceColor AttachedPiece = PieceColor.None;
//
//	//Ability list informaton
//	public static Abilities AbilityList = new Abilities ( );
//
//	public Ability ( )
//	{
//
//	}
//
//	public Ability ( Ability a )
//	{
//		ID = a.ID;
//		Name = a.Name;
//		Desc = a.Desc;
//		AttachesToPiece = a.AttachesToPiece;
//	}
//
//	public Ability ( int _ID, string _Name, string _Desc, bool _AttachesToPiece )
//	{
//		ID = _ID;
//		Name = _Name;
//		Desc = _Desc;
//		AttachesToPiece = _AttachesToPiece;
//	}
}

public class Abilities
{
//	//Animation information
//	private const float ANIMATE_TIME = 0.5f;
//
//	/// <summary>
//	/// Gets an ability by it's ID.
//	/// </summary>
//	public Ability GetAbilityByID ( int id )
//	{
//		//Check id and return the corrisponding ability
//		if ( id == Armor.ID )
//			return new Ability ( Armor );
//		if ( id == Caboose.ID )
//			return new Ability ( Caboose );
//		if ( id == Conversion.ID )
//			return new Ability ( Conversion );
//		if ( id == GrimReaper.ID )
//			return new Ability ( GrimReaper );
//		if ( id == Jolt.ID )
//			return new Ability ( Jolt );
//		if ( id == MadHatter.ID )
//			return new Ability ( MadHatter );
//		if ( id == NonagressionPact.ID )
//			return new Ability ( NonagressionPact );
//		if ( id == Teleport.ID )
//			return new Ability ( Teleport );
//		if ( id == Torus.ID )
//			return new Ability ( Torus );
//		if ( id == Catapult.ID )
//			return new Ability ( Catapult );
//		if ( id == Pacifist.ID )
//			return new Ability ( Pacifist );
//		if ( id == Sacrifice.ID )
//			return new Ability ( Sacrifice );
//
//		//Return null if nothing is found
//		return null;
//	}
//
//	private Ability armor = new Ability
//		(
//			1,
//			"Armor",
//			"Armor requires a piece to be jumped an additional time to be captured.",
//			true 
//		);
//
//	public Ability Armor
//	{
//		get { return armor; }
//	}
//
//	/// <summary>
//	/// Uses the armor ability to avoid a capture.
//	/// </summary>
//	public void UseArmor ( Piece p, Info info )
//	{
//		//Deactivate ability
//		p.ability.IsActive = false;
//		info.DisableAbility ( Armor.ID, p.owner );
//
//		//Check if jumped by a piece with sacrifice
//		if ( HasSacrifice ( info.selectedPiece ) )
//		{
//			//Allow the piece to double jump armor
//			info.sacrificeArmorTile = info.selectedPiece.currentTile;
//		}
//
//		//Animate armor
//		p.sprite.DOFade ( 0, ANIMATE_TIME ).SetLoops ( 2, LoopType.Yoyo );
//	}
//
//	private Ability caboose = new Ability
//		(
//			2,
//			"Caboose",
//			"Caboose allows three or more nonwhite pieces in a line to all move forward at once in the direction of the line.",
//			false
//		);
//
//	public Ability Caboose
//	{
//		get { return caboose; }
//	}
//
//	/// <summary>
//	/// Returns the number of friendly pieces are in a line.
//	/// </summary>
//	private int CabooseNeighborChecker ( Tile t, int direction, Player p, int count )
//	{
//		//Check for friendly piece
//		if ( t.neighbors [ direction ] != null && t.neighbors [ direction ].currentPiece != null && t.neighbors [ direction ].currentPiece.owner == p && t.neighbors [ direction ].currentPiece.color != PieceColor.White )
//			return CabooseNeighborChecker ( t.neighbors [ direction ], direction, p, count + 1 );
//		else
//			return count;
//	}
//
//	/// <summary>
//	/// Check if there is a viable setup for using the Caboose ability.
//	/// </summary>
//	public bool HasCaboose ( Info info )
//	{
//		//Find the current player's pieces
//		foreach ( Tile t in info.board )
//		{
//			//Check for the player's pieces
//			if ( t.currentPiece == null || t.currentPiece.owner != info.currentPlayer || t.currentPiece.color == PieceColor.White )
//				continue;
//
//			//Store direction
//			int start, end;
//
//			//Check player
//			if ( t.currentPiece.owner == info.player1 ) 
//			{
//				//Section off tiles to move right
//				start = 2;
//				end = 6;
//			} 
//			else 
//			{
//				//Section off tiles to move left
//				start = 0;
//				end = 4;
//			}
//
//			//Check each direction
//			for ( int i = start; i < end; i++ )
//			{
//				//Check for tile
//				if ( t.neighbors [ i ] == null || t.neighbors [ i ].currentPiece == null || t.neighbors [ i ].currentPiece.owner == info.opponent || t.neighbors [ i ].currentPiece.color == PieceColor.White )
//					continue;
//
//				//Store the number of friendly pieces in a line
//				int count = CabooseNeighborChecker ( t.neighbors [ i ], i , info.currentPlayer , 1 );
//
//				//Check count
//				if ( count > 1 )
//				{
//					//Check if the front of the line has a open space for movement
//					switch ( count )
//					{
//						case 2:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Return that there is at least one Caboose setup
//								return true;
//							}
//							break;
//						case 3:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Return that there is at least one Caboose setup
//								return true;
//							}
//							break;
//						case 4:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Return that there is at least one Caboose setup
//								return true;
//							}
//							break;
//						case 5:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Return that there is at least one Caboose setup
//								return true;
//							}
//							break;
//					}
//				}
//			}
//		}
//
//		//Return that there are no Caboose setups
//		return false;
//	}
//
//	/// <summary>
//	/// Check if there is a viable setup for using the Caboose ability.
//	/// </summary>
//	public void GetCaboose ( Info info )
//	{
//		//Clear caboose list
//		info.cabooseList.Clear ( );
//
//		//Find the current player's pieces
//		foreach ( Tile t in info.board )
//		{
//			//Check for the player's pieces
//			if ( t.currentPiece == null || t.currentPiece.owner != info.currentPlayer || t.currentPiece.color == PieceColor.White )
//				continue;
//			
//			//Store direction
//			int start, end;
//			
//			//Check player
//			if ( t.currentPiece.owner == info.player1 ) 
//			{
//				//Section off tiles to move right
//				start = 2;
//				end = 6;
//			} 
//			else 
//			{
//				//Section off tiles to move left
//				start = 0;
//				end = 4;
//			}
//			
//			//Check each direction
//			for ( int i = start; i < end; i++ )
//			{
//				//Check for tile
//				if ( t.neighbors [ i ] == null || t.neighbors [ i ].currentPiece == null || t.neighbors [ i ].currentPiece.owner == info.opponent || t.neighbors [ i ].currentPiece.color == PieceColor.White )
//					continue;
//
//				//Store the number of friendly pieces in a line
//				int count = CabooseNeighborChecker ( t.neighbors [ i ], i , info.currentPlayer, 1 );
//				
//				//Check count
//				if ( count > 1 )
//				{
//					//Check if the front of the line has a open space for movement
//					switch ( count )
//					{
//						case 2:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Store caboose
//								CabooseList l = new CabooseList ( );
//								l.list.Add ( t );
//								l.list.Add ( t.neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ] );
//
//								//Store direction
//								l.direction = i;
//
//								//Store destination
//								l.destination = t.neighbors [ i ].neighbors [ i ].neighbors [ i ];
//
//								//Add caboose to list
//								info.cabooseList.Add ( l );
//
//								//Highlight tile
//								t.HighlightAbilityPieceSelection ( );
//							}
//							break;
//						case 3:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Store caboose
//								CabooseList l = new CabooseList ( );
//								l.list.Add ( t );
//								l.list.Add ( t.neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] );
//
//								//Store direction
//								l.direction = i;
//
//								//Store destination
//								l.destination = t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ];
//								
//								//Add caboose to list
//								info.cabooseList.Add ( l );
//
//								//Highlight tile
//								t.HighlightAbilityPieceSelection ( );
//							}
//							break;
//						case 4:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Store caboose
//								CabooseList l = new CabooseList ( );
//								l.list.Add ( t );
//								l.list.Add ( t.neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] );
//								
//								//Store direction
//								l.direction = i;
//								
//								//Store destination
//								l.destination = t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ];
//								
//								//Add caboose to list
//								info.cabooseList.Add ( l );
//
//								//Highlight tile
//								t.HighlightAbilityPieceSelection ( );
//							}
//							break;
//						case 5:
//							//Check tile
//							if ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//							{
//								//Store caboose
//								CabooseList l = new CabooseList ( );
//								l.list.Add ( t );
//								l.list.Add ( t.neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] );
//								l.list.Add ( t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ] );
//								
//								//Store direction
//								l.direction = i;
//								
//								//Store destination
//								l.destination = t.neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ].neighbors [ i ];
//								
//								//Add caboose to list
//								info.cabooseList.Add ( l );
//
//								//Highlight tile
//								t.HighlightAbilityPieceSelection ( );
//							}
//							break;
//					}
//				}
//			}
//		}
//	}
//
//	/// <summary>
//	/// Highlights the list of tiles in a caboose.
//	/// </summary>
//	public void HighlightCaboose ( CabooseList l )
//	{
//		//Highlight caboose
//		foreach ( Tile t in l.list )
//			t.HighlightSelectedAbilityPiece ( );
//		
//		//Highlight destination
//		l.destination.HighlightSelectedMove ( );
//	}
//
//	/// <summary>
//	/// Uses the Caboose ability by moving each piece in the caboose forward.
//	/// </summary>
//	public void UseCaboose ( CabooseList l, Info info )
//	{
//		//Store animation
//		Sequence s = DOTween.Sequence ( );
//
//		//Move each piece in the caboose
//		for ( int i = l.list.Count - 1; i > -1; i-- )
//		{
//			//Store destination
//			Tile t = l.list [ i ];
//			Tile d = l.list [ i ].neighbors [ l.direction ];
//
//			//Update tile and piece references
//			t.currentPiece.currentTile = d;
//			d.currentPiece = t.currentPiece;
//			t.currentPiece = null;
//
//			//Move the piece
//			s.Insert ( 0, d.currentPiece.transform.DOMove ( d.transform.position, ANIMATE_TIME ) );
//		}
//
//		//End turn
//		s.OnComplete ( () =>
//		    {
//				//Clear caboose info
//				info.cabooseList.Clear ( );
//				info.selectedCaboose = null;
//				info.multiCabooseTile = false;
//			
//				//End turn
//				info.EndTurn ( );
//			} );
//	}
//
//	private Ability conversion = new Ability
//		(
//			3,
//			"Conversion",
//			"Conversion allows a piece to convert the first enemy piece it jumps into an ally. Conversion can be used once per game.",
//			true
//		);
//
//	public Ability Conversion
//	{
//		get { return conversion; }
//	}
//
//	/// <summary>
//	/// Uses the Conversion ability by stealing the opponent's piece for the player's own use.
//	/// </summary>
//	public void UseConversion ( Piece convertedPiece, Info info )
//	{
//		//Deactivate ability
//		info.selectedPiece.ability.IsActive = false;
//		info.DisableAbility ( Conversion.ID, info.currentPlayer );
//
//		//Check for any abilities
//		if ( convertedPiece.ability != null )
//		{
//			//Remove the ability
//			info.DisableAbility ( convertedPiece.ability.ID, convertedPiece.owner );
//			convertedPiece.ability = null;
//		}
//		
//		//Remove any nonagression partners
//		convertedPiece.nonagressionPartners.Clear ( );
//		
//		//Convert piece
//		convertedPiece.owner = info.currentPlayer;
//		info.currentPlayer.pieces.Add ( convertedPiece );
//		info.opponent.pieces.Remove ( convertedPiece );
//
//		//Change piece's color
//		convertedPiece.color = PieceColor.Grey;
//
//		//Animate conversion
//		Vector3 rot;
//		if ( convertedPiece.owner == info.player1 )
//			rot = new Vector3 ( 0, 0, 45 );
//		else
//			rot = new Vector3 ( 0, 0, 225 );
//		Sequence s = DOTween.Sequence ( )
//			.AppendInterval ( ANIMATE_TIME )
//			.Append ( convertedPiece.sprite.DOColor ( Piece.Display.Grey, ANIMATE_TIME ) )
//			.Insert ( ANIMATE_TIME, convertedPiece.transform.DORotate ( rot, ANIMATE_TIME ) );
//	}
//
//	private Ability grimReaper = new Ability
//		(
//			4,
//			"Grim Reaper",
//			"Grim Reaper allows a piece to automatically move to the last position of a friendly piece that is captured.",
//			true
//		);
//
//	public Ability GrimReaper
//	{
//		get { return grimReaper; }
//	}
//
//	/// <summary>
//	/// Determines whether the player has the Grim Reaper ability.
//	/// </summary>
//	public bool HasGrimReaper ( Player p )
//	{
//		//Search for a piece with the Grim Reaper ability
//		for ( int i = 0; i < p.pieces.Count; i++ )
//		{
//			//Check ability
//			if ( p.pieces [ i ].ability == null || !p.pieces [ i ].ability.IsActive || p.pieces [ i ].ability.ID != GrimReaper.ID ) 
//				continue;
//
//			//Return that the player does have a Grim Reaper piece
//			return true;
//		}
//
//		//Return that the player does not have a Grim Reaper piece
//		return false;
//	}
//
//	/// <summary>
//	/// Returns the player's Grim Reaper piece.
//	/// </summary>
//	public Piece GetGrimReaper ( Player p )
//	{
//		//Search for a piece with the Grim Reaper ability
//		for ( int i = 0; i < p.pieces.Count; i++ )
//		{
//			//Check ability
//			if ( p.pieces [ i ].ability == null || p.pieces [ i ].ability.ID != GrimReaper.ID )
//				continue;
//			
//			//Return that the player does have a Grim Reaper piece
//			return p.pieces [ i ];
//		}
//		
//		//Return that the player does not have a Grim Reaper piece
//		return null;
//	}
//
//	/// <summary>
//	/// Uses the Grim Reaper ability by moving the player's Grim Reaper piece to its new tile.
//	/// </summary> 
//	public void UseGrimReaper ( Piece p, Tile t, Info info )
//	{
//		//Store piece color
//		Color c = p.sprite.color;
//
//		//Animate grim reaper
//		Sequence s = DOTween.Sequence ( )
//			.Append ( p.currentTile.sprite.DOColor ( Color.black, ANIMATE_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
//			.Insert ( 0, p.sprite.DOColor ( Color.black, ANIMATE_TIME ) )
//			.Insert ( ANIMATE_TIME, p.sprite.DOFade ( 0, 0 ) )
//			.AppendCallback ( () => 
//			{
//				//Move the player's Grim Reaper piece
//				p.currentTile.currentPiece = null;
//				p.currentTile = t;
//				t.currentPiece = p;
//				p.Move ( t.transform.position );
//			} )
//			.Append ( t.sprite.DOColor ( Color.black, ANIMATE_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
//			.Insert ( ANIMATE_TIME * 3, p.sprite.DOFade ( 1, 0 ) )
//			.Insert ( ANIMATE_TIME * 3, p.sprite.DOColor ( c, ANIMATE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//Reset grim reaper tile
//				info.grimReaperTile = null;
//				
//				//Hide prompt
//				info.currentPlayer.prompt.gameObject.SetActive ( false );
//
//				//Store temporary save data
//				info.TempSave ( );
//				
//				//Reset board
//				info.ResetBoardColor ( );
//				
//				//Highlight pieces
//				info.HighlightCurrentPlayerPieces ( );
//			} );
//	}
//
//	private Ability jolt = new Ability
//		(
//			5,
//			"Jolt",
//			"Jolt allows a piece to enable any friendly pieces that it jumps to make an additional move that turn.",
//			true
//		);
//
//	public Ability Jolt
//	{
//		get { return jolt; }
//	}
//
//	/// <summary>
//	/// Uses the Jolt ability by granting a piece an addition move that turn.
//	/// </summary>
//	public void UseJolt ( Tile t, Info info )
//	{
//		//Add piece to the jolt list
//		info.joltList.Add ( t );
//		t.currentPiece.isJolted = true;
//
//		//Animate the jolted piece
//		t.currentPiece.transform.DOShakePosition ( ANIMATE_TIME, 0.5f, 20 ).SetLoops ( -1 );
//	}
//
//	private Ability madHatter = new Ability
//		(
//			6,
//			"Mad Hatter",
//			"Mad Hatter allows any two of the player's nonwhite pieces to swap places at the beginning of a turn. Mad Hatter can be used once per game.",
//			false
//		);
//
//	public Ability MadHatter
//	{
//		get { return madHatter; }
//	}
//
//	/// <summary>
//	/// Uses the Mad Hatter ability by swapping two pieces.
//	/// </summary>
//	public void UseMadHatter ( Tile t1, Tile t2, Info info )
//	{
//		//Store temporary value
//		Piece temp = t1.currentPiece;
//
//		//Swap pieces
//		t1.currentPiece.currentTile = t2;
//		t2.currentPiece.currentTile = t1;
//		t1.currentPiece = t2.currentPiece;
//		t2.currentPiece = temp;
//
//		//Bring pieces to front
//		t1.currentPiece.sprite.sortingOrder = 2;
//		t2.currentPiece.sprite.sortingOrder = 2;
//
//		//Animate mad hatter
//		Sequence s = DOTween.Sequence ( )
//			.Append ( t1.currentPiece.transform.DOMove ( t1.transform.position, ANIMATE_TIME ) )
//			.Insert ( 0, t2.currentPiece.transform.DOMove ( t2.transform.position, ANIMATE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//Deactivate ability
//				info.abilityInUse.IsActive = false;
//				info.DisableAbility ( info.abilityInUse.ID, info.currentPlayer );
//
//				//Enable abilities use buttons
//				info.EnableAbilityButtons ( );
//				
//				//Reset ability selection list
//				info.abilityTileSelection.Clear ( );
//				
//				//Reset board
//				info.ResetBoardColor ( );
//				
//				//Highlight pieces
//				info.HighlightCurrentPlayerPieces ( );
//			} );
//	}
//
//	private Ability nonagressionPact = new Ability
//		(
//			7,
//			"Nonaggression Pact",
//			"Nonaggression Pact prevents one of each players' nonwhite pieces from interacting with one another. The two selected pieces cannot jump or use abilities against each other. Nonaggression Pact can be used once per game.",
//			false
//		);
//
//	public Ability NonagressionPact
//	{
//		get { return nonagressionPact; }
//	}
//
//	/// <summary>
//	/// Uses the Nonagression Pact by marking two pieces that cannot interact with one another.
//	/// </summary>
//	public void UseNonagressionPact ( Tile t1, Tile t2, Info info )
//	{
//		//Add each piece as each other's nonagression partner
//		t1.currentPiece.nonagressionPartners.Add ( t2.currentPiece.color );
//		t2.currentPiece.nonagressionPartners.Add ( t1.currentPiece.color );
//
//		//Animate nonaggression pact
//		Sequence s = DOTween.Sequence ( )
//			.Append ( t1.currentPiece.sprite.DOColor ( new Color32 ( 150, 50, 255, 255 ), ANIMATE_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
//			.Insert ( 0, t2.currentPiece.sprite.DOColor ( new Color32 ( 150, 50, 255, 255 ), ANIMATE_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
//			.OnComplete ( () =>
//			{
//				//Deactivate ability
//				info.abilityInUse.IsActive = false;
//				info.DisableAbility ( info.abilityInUse.ID, info.currentPlayer );
//				
//				//Enable abilities use buttons
//				info.EnableAbilityButtons ( );
//				
//				//Reset ability selection list
//				info.abilityTileSelection.Clear ( );
//				
//				//Reset board
//				info.ResetBoardColor ( );
//				
//				//Highlight pieces
//				info.HighlightCurrentPlayerPieces ( );
//			} );
//	}
//
//	private Ability teleport = new Ability
//		(
//			8,
//			"Teleport",
//			"Teleport allows a piece to instantly teleport forward within three spaces of its origin.",
//			true
//		);
//
//	public Ability Teleport
//	{
//		get { return teleport; }
//	}
//
//	/// <summary>
//	/// Uses the Teleport ability to instantly move a piece a few spaces away.
//	/// </summary>
//	public void UseTeleport ( Piece p, Tile t, Info info )
//	{
//		//Change the tile references
//		p.currentTile.currentPiece = null;
//		p.currentTile = t;
//		t.currentPiece = p;
//
//		//Clear board
//		info.ResetBoardColor ( );
//		
//		//Mark that it is no longer the beginning of a turn
//		info.beginningOfTurn = false;
//		info.DisableAbilityButtons ( );
//
//		//Animate teleport
//		Sequence s = DOTween.Sequence ( )
//			.Append ( p.sprite.DOFade ( 0, ANIMATE_TIME ) )
//			.AppendCallback ( () =>
//			{
//				//Move piece
//				p.Move ( t.transform.position );
//			} )
//			.Append ( p.sprite.DOFade ( 1, ANIMATE_TIME ) )
//			.OnComplete ( () =>
//			{
//				//End turn
//				info.EndTurn ( );
//			} );
//	}
//
//	/// <summary>
//	/// Marks every tile a piece could teleport to with the Teleport ability.
//	/// </summary>
//	public void GetTeleport ( Tile t, int start, int end, int count )
//	{
//		//Check each neighbor
//		for ( int i = start; i < end; i++ )
//		{
//			//Check for neighbor
//			if ( t.neighbors [ i ] != null )
//			{
//				//Check if the tile is available
//				if ( t.neighbors [ i ].currentPiece == null && t.neighbors [ i ].state == TileState.Normal )
//				{
//					//Highlight tile
//					t.neighbors [ i ].HighlightPotentialAbility ( );
//				}
//			
//				//Check the reach of the teleport
//				if ( count > 0 )
//				{
//					//Continue search
//					GetTeleport ( t.neighbors [ i ], start, end, count - 1 );
//				}
//			}
//		}
//	}
//
//	private Ability torus = new Ability
//		(
//			9,
//			"Torus",
//			"Torus allows a piece's movement to wrap around the board.",
//			true
//		);
//
//	public Ability Torus
//	{
//		get { return torus; }
//	}
//
//	/// <summary>
//	/// Uses the Torus ability to allow the piece's movement to wrap around the board.
//	/// </summary>
//	public void UseTorus ( Piece p, Tile t, Info info )
//	{
//		//Set previous tile
//		p.prevTile = p.currentTile;
//
//		//Movement information
//		float xMove;
//		float yMove;
//		float reentry = 1.5f;
//		float delay = 0.1f;
//		bool isTorusJump = t.isTorusJump;
//		Sequence s = DOTween.Sequence ( );
//
//		//Mark that it is no longer the beginning of a turn
//		info.beginningOfTurn = false;
//		info.DisableAbilityButtons ( );
//
//		//Clear board
//		info.ResetBoardColor ( );
//		info.BringPieceToFront ( p );
//
//		//Check tile position
//		if ( p.currentTile.transform.position.x != t.transform.position.x )
//		{
//			//Mark that the movement is forward
//			xMove = 1.3f;
//			yMove = 0.75f;
//		}
//		else
//		{
//			//Mark that the movement is strictly side to side
//			xMove = 0;
//			yMove = 1.5f;
//		}
//
//		//Check if the move is a jump
//		if ( isTorusJump )
//		{
//			//Find middle tile
//			Tile mid = null;
//			bool isTorusFirst = false;
//			for ( int i = 0; i < t.neighbors.Length; i++ )
//			{
//				//Check for selected tile
//				if ( p.currentTile.neighbors [ i ] != null && p.currentTile.neighbors [ i ].torusNeighbors [ i ] != null && p.currentTile.neighbors [ i ].torusNeighbors [ i ] == t )
//				{
//					//Store middle tile
//					mid = p.currentTile.neighbors [ i ];
//					isTorusFirst = false;
//					break;
//				}
//				else if ( p.currentTile.torusNeighbors [ i ] != null && p.currentTile.torusNeighbors [ i ].neighbors [ i ] != null && p.currentTile.torusNeighbors [ i ].neighbors [ i ] == t )
//				{
//					//Store middle tile
//					mid = p.currentTile.torusNeighbors [ i ];
//					isTorusFirst = true;
//					break;
//				}
//			}
//
//			//Check if torus is first
//			if ( isTorusFirst )
//			{
//				//Determine starting tile position
//				if ( p.currentTile.transform.position.y < 0 )
//					yMove *= -1;
//				else
//					reentry *= -1;
//				if ( p.currentTile.transform.position.x > t.transform.position.x )
//					xMove *= -1;
//
//				//Animate torus
//				s.Append ( p.transform.DOMove ( new Vector3 ( p.transform.position.x + xMove, p.transform.position.y + yMove, p.transform.position.y ), ANIMATE_TIME ) )
//					.Insert ( 0, p.sprite.DOFade ( 0, ANIMATE_TIME ) )
//					.Append ( p.transform.DOMove ( new Vector3 ( mid.transform.position.x, mid.transform.position.y + reentry, mid.transform.position.z ), 0 ) )
//					.AppendCallback ( () =>
//					{
//						//Check for capture
//						if ( mid.currentPiece.owner == info.opponent )
//							mid.CapturePiece ( mid );
//					} )
//					.AppendInterval ( delay );
//
//				//Check middle tile position
//				if ( mid.transform.position.x != t.transform.position.x )
//					s.Append ( p.transform.DOMove ( mid.transform.position, ANIMATE_TIME ) );
//				else
//					s.Append ( p.transform.DOMove ( t.transform.position, ANIMATE_TIME * 2 ) );
//
//				//Finish torus animation
//				s.Insert ( ANIMATE_TIME + delay, p.sprite.DOFade ( 1, ANIMATE_TIME ) )
//					.Append ( p.transform.DOMove ( t.transform.position, ANIMATE_TIME ) );
//			}
//			else
//			{
//				//Determine middle tile position
//				if ( mid.transform.position.y < 0 )
//					yMove *= -1;
//				else
//					reentry *= -1;
//				if ( mid.transform.position.x > t.transform.position.x )
//					xMove *= -1;
//
//				//Check for capture
//				if ( mid.currentPiece.owner == info.opponent )
//					mid.CapturePiece ( mid );
//
//				//Animate torus
//				s.Append ( p.transform.DOMove ( new Vector3 ( mid.transform.position.x + xMove, mid.transform.position.y + yMove, mid.transform.position.z ), ANIMATE_TIME * 2 ) )
//					.Insert ( ANIMATE_TIME, p.sprite.DOFade ( 0, ANIMATE_TIME ) )
//					.Append ( p.transform.DOMove ( new Vector3 ( t.transform.position.x, t.transform.position.y + reentry, t.transform.position.z ), 0 ) )
//					.AppendInterval ( delay )
//					.Append ( p.transform.DOMove ( t.transform.position, ANIMATE_TIME ) )
//					.Insert ( ( ANIMATE_TIME * 2 ) + delay, p.sprite.DOFade ( 1, ANIMATE_TIME ) );
//			}
//		}
//		else
//		{
//			//Determine starting tile position
//			if ( p.currentTile.transform.position.y < 0 )
//				yMove *= -1;
//			else
//				reentry *= -1;
//			if ( p.currentTile.transform.position.x > t.transform.position.x )
//				xMove *= -1;
//
//			//Animate torus
//			s.Append ( p.transform.DOMove ( new Vector3 ( p.transform.position.x + xMove, p.transform.position.y + yMove, p.transform.position.z ), ANIMATE_TIME ) )
//				.Insert ( 0, p.sprite.DOFade ( 0, ANIMATE_TIME ) )
//				.Append ( p.transform.DOMove ( new Vector3 ( t.transform.position.x, t.transform.position.y + reentry, t.transform.position.z ), 0 ) )
//				.AppendInterval ( delay )
//				.Append ( p.transform.DOMove ( t.transform.position, ANIMATE_TIME ) )
//				.Insert ( ANIMATE_TIME + delay, p.sprite.DOFade ( 1, ANIMATE_TIME ) );
//		}
//
//		//Update tile references and check for additional movement at the end of the animation
//		s.OnComplete ( () =>
//			{
//				//Change the tile references
//				p.currentTile.currentPiece = null;
//				p.currentTile = t;
//				t.currentPiece = p;
//
//				//Check follow up
//				if ( isTorusJump )
//					t.SelectPiece ( );
//				else
//					info.EndTurn ( );
//			} );
//	}
//
//	/// <summary>
//	/// Marks every tile a piece with the Torus ability could move to.
//	/// </summary>
//	public bool GetTorus ( Tile t, int start, int end, Info info )
//	{
//		//Track turn
//		bool additionalMove = false;
//
//		//Check neighboring tiles
//		for ( int i = start; i < end; i++ ) 
//		{
//			//Check for tile
//			if ( t.neighbors [ i ] != null )
//			{
//				//Check for piece for turn to continue and previous tile
//				if ( t.neighbors [ i ].currentPiece != null )
//				{
//					//Check for torus ability and torus jump space
//					if ( t.neighbors [ i ].torusNeighbors [ i ] != null && t.neighbors [ i ].torusNeighbors [ i ].currentPiece == null && !info.selectedPiece.IsPrevTile ( t.neighbors [ i ].torusNeighbors [ i ] ) )
//					{
//						//Check for nonagression pacts
//						if ( t.neighbors [ i ].currentPiece.owner != info.currentPlayer && t.neighbors [ i ].currentPiece.nonagressionPartners.Contains ( t.currentPiece.color ) )
//							continue;
//						
//						//Check if piece is friendly 
//						if ( t.neighbors [ i ].currentPiece.owner != info.currentPlayer && !HasPacifist ( t.neighbors [ i ].currentPiece, info ) )
//						{
//							//Highlight potential capture
//							t.neighbors [ i ].HighlightPotentialCapture ( );
//						}
//
//						//Check if it is the beginning of the turn
//						if ( !info.beginningOfTurn )
//							additionalMove = true;
//						
//						//Mark as a torus jump 
//						t.neighbors [ i ].torusNeighbors [ i ].isTorusJump = true;
//						
//						//Highlight potential jump
//						t.neighbors [ i ].torusNeighbors [ i ].HighlightPotentialAbility ( );
//					}
//				}
//			}
//			else
//			{
//				//Check for torus tile
//				if ( t.torusNeighbors [ i ] != null )
//				{
//					//Check for a piece
//					if ( t.torusNeighbors [ i ].currentPiece != null )
//					{
//						//Check for jump space
//						if ( t.torusNeighbors [ i ].neighbors [ i ] != null && t.torusNeighbors [ i ].neighbors [ i ].currentPiece == null && !info.selectedPiece.IsPrevTile ( t.torusNeighbors [ i ].neighbors [ i ] ) )
//						{
//							//Check for nonagression pacts
//							if ( t.torusNeighbors [ i ].currentPiece.owner != info.currentPlayer && t.torusNeighbors [ i ].currentPiece.nonagressionPartners.Contains ( t.currentPiece.color ) )
//								continue;
//							
//							//Check if piece is friendly 
//							if ( t.torusNeighbors [ i ].currentPiece.owner != info.currentPlayer && !HasPacifist ( t.torusNeighbors [ i ].currentPiece, info ) )
//							{
//								//Highlight potential capture
//								t.torusNeighbors [ i ].HighlightPotentialCapture ( );
//							}
//
//							//Allow turn to continue
//							if ( !info.beginningOfTurn )
//								additionalMove = true;
//
//							//Mark as a torus jump
//							t.torusNeighbors [ i ].neighbors [ i ].isTorusJump = true;
//							
//							//Highlight potential jump
//							t.torusNeighbors [ i ].neighbors [ i ].HighlightPotentialAbility ( );
//						}
//					}
//					else
//					{
//						//Check previous tile
//						if ( info.beginningOfTurn && !info.selectedPiece.IsPrevTile ( t.torusNeighbors [ i ] ) )
//							t.torusNeighbors [ i ].HighlightPotentialAbility ( );
//					}
//				}
//			}
//		}
//
//		//Return if there is an additional move
//		return additionalMove;
//	}
//
//	private Ability catapult = new Ability
//		(
//			10,
//			"Catapult",
//			"Catapult allows a piece to land one tile further away when jumping a piece. Any enemy pieces jumped over while using Catapult will be captured.",
//			true
//		);
//
//	public Ability Catapult
//	{
//		get { return catapult; }
//	}
//
//	/// <summary>
//	/// Uses the Catapult ability.
//	/// </summary>
//	public void UseCatapult ( Piece p, Tile t, Info info )
//	{
//		//Store the two tiles being jumped
//		Tile mid1 = null;
//		Tile mid2 = null;
//		for ( int i = 0; i < t.neighbors.Length; i++ )
//		{
//			//Find middle tiles
//			if ( p.currentTile.neighbors [ i ] != null && p.currentTile.neighbors [ i ].neighbors [ i ] != null && p.currentTile.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && p.currentTile.neighbors [ i ].neighbors [ i ].neighbors [ i ] == t )
//			{
//				mid1 = p.currentTile.neighbors [ i ];
//				mid2 = p.currentTile.neighbors [ i ].neighbors [ i ];
//				break;
//			}
//		}
//
//		//Mark that it is no longer the beginning of a turn
//		info.beginningOfTurn = false;
//		info.DisableAbilityButtons ( );
//
//		//Check if piece is friendly on the first tile
//		if ( mid1.state == TileState.PotentialCapture )
//			mid1.CapturePiece ( mid1 );
//		
//		//Check if piece is friendly on the second tile
//		if ( mid2.state == TileState.PotentialCapture )
//			mid2.CapturePiece ( mid2 );
//
//		//Clear board
//		info.ResetBoardColor ( );
//		info.BringPieceToFront ( p );
//
//		//Animate catapult
//		Sequence s = DOTween.Sequence ( )
//			.Append ( p.transform.DOMove ( t.transform.position, ANIMATE_TIME * 2 ) )
//			.Insert ( 0, p.transform.DOScale ( 4, ANIMATE_TIME ).SetLoops ( 2, LoopType.Yoyo ) )
//			.OnComplete ( () =>
//			{
//				//Update tile references
//				p.currentTile.currentPiece = null;
//				p.currentTile = t;
//				t.currentPiece = p;
//
//				//End turn
//				info.EndTurn ( );
//			} );
//	}
//
//	/// <summary>
//	/// Marks the tile a piece with Catapult could move to as well as any potential captures.
//	/// </summary>
//	public void GetCatapult ( Tile t, int start, int end, Info info )
//	{
//		//Check neighboring tiles
//		for ( int i = start; i < end; i++ )
//		{
//			//Check for catapult landing
//			if ( t.neighbors [ i ] != null && t.neighbors [ i ].currentPiece != null && t.neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ] != null && t.neighbors [ i ].neighbors [ i ].neighbors [ i ].currentPiece == null )
//			{
//				//Check for nonaggression pacts
//				if ( ( t.neighbors [ i ].currentPiece.owner == info.opponent && t.neighbors [ i ].currentPiece.nonagressionPartners.Contains ( t.currentPiece.color ) ) || ( t.neighbors [ i ].neighbors [ i ].currentPiece != null && t.neighbors [ i ].neighbors [ i ].currentPiece.owner == info.opponent && t.neighbors [ i ].neighbors [ i ].currentPiece.nonagressionPartners.Contains ( t.currentPiece.color ) ) )
//					continue;
//
//				//Highlight catapult landing
//				t.neighbors [ i ].neighbors [ i ].neighbors [ i ].HighlightPotentialAbility ( );
//
//				//Check for capture on the first tile
//				if ( t.neighbors [ i ].currentPiece.owner == info.opponent && !HasPacifist ( t.neighbors [ i ].currentPiece, info ) )
//					t.neighbors [ i ].HighlightPotentialCapture ( );
//
//				//Check for capture on the second tile
//				if ( t.neighbors [ i ].neighbors [ i ].currentPiece != null && t.neighbors [ i ].neighbors [ i ].currentPiece.owner == info.opponent && !HasPacifist ( t.neighbors [ i ].neighbors [ i ].currentPiece, info ) )
//					t.neighbors [ i ].neighbors [ i ].HighlightPotentialCapture ( );
//			}
//		}
//	}
//
//	private Ability pacifist = new Ability
//		(
//			11,
//			"Pacifist",
//			"Pacifist makes a piece immune to being captured, however, the piece is also unable to capture any enemy pieces.",
//			true
//		);
//
//	public Ability Pacifist
//	{
//		get { return pacifist; }
//	}
//
//	/// <summary>
//	/// Determines whether a piece can ignore a capture with the Pacifist ability.
//	/// </summary>
//	public bool HasPacifist ( Piece p, Info info )
//	{
//		//Check if the current piece has conversion
//		if ( info.selectedPiece.ability != null && info.selectedPiece.ability.ID == Conversion.ID && info.selectedPiece.ability.IsActive )
//			return false;
//
//		//Check if the piece has pacifist
//		if ( p.ability != null && p.ability.ID == Pacifist.ID )
//			return true;
//
//		//Return that the piece doesn't have the pacifist ability
//		return false;
//	}
//
//	private Ability sacrifice = new Ability
//		(
//			12,
//			"Sacrifice",
//			"Sacrifice doubles the movement of a piece in exchange for the player starting the game with one less piece.",
//			true
//		);
//
//	public Ability Sacrifice
//	{
//		get { return sacrifice; }
//	}
//
//	/// <summary>
//	/// Uses the Sacrifice ability to move twice in one turn.
//	/// </summary>
//	public void UseSacrifice ( Piece p )
//	{
//		//Increment move counter
//		p.sacrificeMoveRemaining = false;
//	}
//
//	/// <summary>
//	/// Determines whether a piece has the Sacrifice ability and has additional moves left.
//	/// </summary>
//	public bool HasSacrifice ( Piece p )
//	{
//		//Check if the piece has sacrifice and an additional turn remaining
//		if ( p != null && p.ability != null && p.ability.ID == Sacrifice.ID && p.sacrificeMoveRemaining && !p.isJolted )
//			return true;
//
//		//Return that the piece doesn't have additional moves
//		return false;
//	}
//
//	/// <summary>
//	/// Marks the tiles that a piece with the Sacrifice ability should be able to move to that SelectPiece ( ) won't normal detect.
//	/// </summary>
//	public void GetSacrifice ( Tile t, int start, int end, Info info )
//	{
//		//Check each neighbor
//		for ( int i = start; i < end; i++ )
//		{
//			//Check for available tiles
//			if ( t.neighbors [ i ] != null )
//			{
//				//Check if tile is unoccupied
//				if ( t.neighbors [ i ].currentPiece != null )
//				{
//					//Check if tile is the sacrifice armor tile
//					if ( t.neighbors [ i ].neighbors [ i ] != null && info.sacrificeArmorTile != null && t.neighbors [ i ].neighbors [ i ] == info.sacrificeArmorTile )
//					{
//						//Highlight potential capture
//						t.neighbors [ i ].HighlightPotentialCapture ( );
//						t.neighbors [ i ].neighbors [ i ].HighlightPotentialAbility ( );
//					}
//				}
//				else
//				{
//					//Check if tile is unmarked
//					if ( t.currentPiece.sacrificeMoveRemaining && t.neighbors [ i ].state == TileState.Normal )
//						t.neighbors [ i ].HighlightPotentialAbility ( );
//				}
//			}
//		}
//	}
}

public class CabooseList
{
//	public List < Tile > list = new List < Tile > ( );
//	public Tile destination;
//	public int direction;
}