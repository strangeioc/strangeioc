/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

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

