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

using System;

namespace strange.examples.multiplecontexts.game
{
	public class GameEvent
	{
		public const string ADD_TO_SCORE = "ADD_TO_SCORE";
		public const string GAME_OVER = "GAME_OVER";
		public const string GAME_UPDATE = "GAME_UPDATE";
		public const string LIVES_CHANGE = "LIVES_CHANGE";
		public const string REPLAY = "REPLAY_REQUEST";
		public const string RESTART_GAME = "RESTART_GAME";
		public const string SCORE_CHANGE = "SCORE_CHANGE";
		public const string SHIP_DESTROYED = "SHIP_DESTROYED";
	}
}

