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

/**
 * @class strange.extensions.sequencer.impl.SequencerException
 * 
 * @deprecated
 */

using System;
using strange.extensions.sequencer.api;

namespace strange.extensions.sequencer.impl
{
	public class SequencerException : Exception
	{
		public SequencerExceptionType type{ get; set;}

		public SequencerException () : base()
		{
		}

		/// Constructs a SequencerException with a message and SequencerExceptionType
		public SequencerException(string message, SequencerExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

