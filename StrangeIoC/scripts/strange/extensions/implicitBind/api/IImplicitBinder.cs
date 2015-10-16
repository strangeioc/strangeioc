﻿/*
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

using System.Reflection;
using strange.extensions.mediation.api;

namespace strange.extensions.implicitBind.api
{
	public interface IImplicitBinder
	{
		/// <summary>
		/// Search through indicated namespaces and scan for all annotated classes.
		/// Automatically create bindings.
		/// </summary>
		/// <param name="usingNamespaces">Array of namespaces. Compared using StartsWith. </param>

		void ScanForAnnotatedClasses(params string[] usingNamespaces);
		Assembly Assembly { get; set; }
	}
}
