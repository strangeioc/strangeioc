using System;

namespace strange.examples.multiplecontexts.game
{
	public interface IScore
	{
		int score {get;}
		int lives {get;}
		
		void Reset();
		int AddToScore(int value);
		int LoseLife();
	}
}

