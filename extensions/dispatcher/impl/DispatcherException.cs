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
 * @class strange.extensions.dispatcher.impl.DispatcherException
 * 
 * An exception thrown by the Dispatcher system.
 */

using System;
using strange.extensions.dispatcher.api;

namespace strange.extensions.dispatcher.impl
{
	public class DispatcherException : Exception
	{
		public DispatcherExceptionType type{ get; set;}

		public DispatcherException() : base()
		{
		}

		/// Constructs a DispatcherException with a message and DispatcherExceptionType
		public DispatcherException(string message, DispatcherExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

