using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace ProjectOcelot.Match.Setup
{
	public struct Debate
	{
		public int ID;
		public string EventName;
		public string DebateTopic;
		public DebateResponse [ ] Responses;

		/// <summary>
		/// The number of debate participants
		/// </summary>
		public int DebateStances
		{
			get
			{
				// Track the number of debate stances
				int count = 0;

				// Count each stance
				for ( int i = 0; i < Responses.Length; i++ )
					if ( Responses [ i ].HasStance )
						count++;

				// Return the number of debate participants
				return count;
			}
		}

		/// <summary>
		/// Get the response of a particular leader by its team color.
		/// </summary>
		/// <param name="team"> The team color of the leader. </param>
		/// <returns> The leader's response </returns>
		public DebateResponse GetLeaderResponse ( Player.TeamColor team )
		{
			// Return the leader response
			return Responses [ (int)team ];
		}
	}

	[System.Serializable]
	public struct DebateResponse
	{
		public string Answer;
		public string Victory;

		/// <summary>
		/// Whether or not there is an answer for this debate response.
		/// </summary>
		public bool HasStance
		{
			get
			{
				// Check for answer
				return !string.IsNullOrEmpty ( Answer );
			}
		}
	}

	public static class DebateGenerator
	{
		#region Private Classes

		[System.Serializable]
		private class DebateJSONData
		{
			public int ID;
			public string EventName;
			public string DebateTopic;

			public DebateResponse Blue;
			public DebateResponse Green;
			public DebateResponse Yellow;
			public DebateResponse Orange;
			public DebateResponse Pink;
			public DebateResponse Purple;
		}

		private class DebateJSONHolder
		{
			public DebateJSONData [ ] Debates;
		}

		#endregion // Private Classes

		#region Debate Data

		private static ReadOnlyCollection<Debate> debateData;
		private static ReadOnlyDictionary<int, Debate> debateDictionary;

		#endregion // Debate Data

		#region Public Functions

		/// <summary>
		/// Initializes the debate list data from a JSON file.
		/// </summary>
		/// <param name="json"> The text from a JSON file. </param>
		public static void InitializeJSONData ( string json )
		{
			// Store data from JSON file
			DebateJSONHolder holder = JsonUtility.FromJson<DebateJSONHolder> ( json );

			// Store each debate
			Debate [ ] tempDebateData = new Debate [ holder.Debates.Length ];
			Dictionary<int, Debate> tempDebateDictionary = new Dictionary<int, Debate> ( );
			for ( int i = 0; i < holder.Debates.Length; i++ )
			{
				// Store debate data
				tempDebateData [ i ] = new Debate
				{
					ID = holder.Debates [ i ].ID,
					EventName = holder.Debates [ i ].EventName,
					DebateTopic = holder.Debates [ i ].DebateTopic,
					Responses = new DebateResponse [ ]
					{
					holder.Debates [ i ].Blue,
					holder.Debates [ i ].Green,
					holder.Debates [ i ].Yellow,
					holder.Debates [ i ].Orange,
					holder.Debates [ i ].Pink,
					holder.Debates [ i ].Purple
					}
				};

				// Add debate to dictionary
				tempDebateDictionary.Add ( tempDebateData [ i ].ID, tempDebateData [ i ] );
			}

			// Store data as immutable
			debateData = new ReadOnlyCollection<Debate> ( tempDebateData );
			debateDictionary = new ReadOnlyDictionary<int, Debate> ( tempDebateDictionary );
		}

		/// <summary>
		/// Get a randomly selected debate with enough participants for a given match type.
		/// </summary>
		/// <param name="match"> The type of match the debate is going to be used for. </param>
		/// <returns> The randomly selected debate. </returns>
		public static Debate GetRandomDebate ( MatchType match )
		{
			// Store a list of filtered debates
			Debate [ ] filteredList;

			// Filter the debates by the match type
			if ( match == MatchType.Rumble || match == MatchType.CustomRumble )
			{
				// Filter for debates with at least 6 participants
				filteredList = debateData.Where ( x => x.DebateStances >= 6 ).ToArray ( );
			}
			else if ( match == MatchType.Control || match == MatchType.CustomControl )
			{
				// Filter for debates with at least 3 participants
				filteredList = debateData.Where ( x => x.DebateStances >= 3 ).ToArray ( );
			}
			else
			{
				// Return all debates
				filteredList = debateData.ToArray ( );
			}

			// Return a randomly selected debate
			return filteredList [ Random.Range ( 0, filteredList.Length ) ];
		}

		/// <summary>
		/// Get a debate by its ID.
		/// </summary>
		/// <param name="id"> The ID of the debate to be retrieved. </param>
		/// <returns> The debate of the corresponding ID. </returns>
		public static Debate GetDebate ( int id )
		{
			// Return the debate by its id
			return debateDictionary [ id ];
		}

		#endregion // Public Functions
	}
}