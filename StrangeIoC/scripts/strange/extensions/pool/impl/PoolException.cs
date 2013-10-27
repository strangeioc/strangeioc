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
using strange.extensions.pool.api;

namespace strange.extensions.pool.impl
{
	public class PoolException : Exception
	{
		public PoolExceptionType type{ get; set;}

		public PoolException() : base()
		{
		}

		/// Constructs a PoolException with a message and PoolExceptionType
		public PoolException(string message, PoolExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

