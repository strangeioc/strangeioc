/*
* Copyright 2015 StrangeIoC
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
 * @class strange.extensions.context.impl.ContextException
 * 
 * An exception raised by the Context system.
 * 
 * @see strange.extensions.editor.api.EditorContextExceptionType
 */

using System;
using strange.extensions.context.api;

namespace strange.extensions.editor.impl
{
	public class EditorContextException : Exception
	{
		public EditorContextExceptionType type{ get; set;}

		public EditorContextException () : base()
		{
		}

		/// Constructs a ContextException with a message and ContextExceptionType
		public EditorContextException(string message, EditorContextExceptionType exceptionType) : base(message)
		{
			type = exceptionType;
		}
	}
}

