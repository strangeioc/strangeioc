/// Score Model
/// ======================
/// Track score and lives.

using System;

namespace strange.examples.multiplecontexts.game
{
	public class ScoreModel : IScore
	{
		private int initLives = 3;
		private int _score;
		private int _lives;
		
		public ScoreModel()
		{
			Reset ();
		}
		
		public int AddToScore(int value)
		{
			_score += value;
			return score;
		}
		
		public int LoseLife()
		{
			_lives = Math.Max(0, _lives - 1);
			return lives;
		}
		
		public void Reset()
		{
			_score = 0;
			_lives = initLives;
		}
		
		public int score
		{
			get
			{
				return _score;
			}
		}
		
		public int lives
		{
			get
			{
				return _lives;
			}
		}
	}
}

