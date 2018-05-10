using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroesSlideMenu : SlideMenu
{
	#region Menu Data

	[SerializeField]
	private SlideMenu hero1Menu;

	#endregion // Menu Data

	#region SlideMenu Override Functions

	protected override void OpenMenu ( bool playAnimation )
	{
		// Open the menu
		base.OpenMenu ( playAnimation );

		// Open the hero 1 settings menu by default
		hero1Menu.OpenMenu ( );
	}

	#endregion // SlideMenuOverride Functions
}
