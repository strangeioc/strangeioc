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

namespace strange.extensions.reflector.api
{
	public enum ReflectionExceptionType
	{
		/// The reflector requires a constructor, which Interfaces don't provide.
		CANNOT_REFLECT_INTERFACE,

		/// The reflector is not allowed to inject into private/protected setters.
		CANNOT_INJECT_INTO_NONPUBLIC_SETTER,

		/// ListensTo attribute must have a matching Inject tag for the Signal in question
		LISTENS_TO_MUST_HAVE_INJECTION,
	}
}

