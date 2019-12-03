using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectOcelot.Match.Arena
{
	public class CommandData
	{
		#region Command Data

		/// <summary>
		/// The unit issuing the command.
		/// </summary>
		public Units.HeroUnit Caster
		{
			get;
			private set;
		}

		/// <summary>
		/// The selected hexes for the command.
		/// </summary>
		public List<Hex> Targets
		{
			get;
			private set;
		}

		/// <summary>
		/// The minimum number of hexes that need to be selected for the command.
		/// </summary>
		public int MinTargets
		{
			get;
			private set;
		}

		/// <summary>
		/// The maximum number of hexes that can be selected for the command.
		/// </summary>
		public int MaxTargets
		{
			get;
			private set;
		}

		/// <summary>
		/// The first selected target for the command.
		/// </summary>
		public Hex PrimaryTarget
		{
			get
			{
				// Return the first target
				return Targets [ 0 ];
			}
		}

		public CommandData ( Units.HeroUnit caster, int targets )
		{
			Caster = caster;
			Targets = new List<Hex> ( );
			MinTargets = targets;
			MaxTargets = targets;
		}

		public CommandData ( Units.HeroUnit caster, int minTargets, int maxTargets )
		{
			Caster = caster;
			Targets = new List<Hex> ( );
			MinTargets = minTargets;
			MaxTargets = maxTargets;
		}

		#endregion // Command Data
	}
}