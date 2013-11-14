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
 * @enum strange.extensions.pool.api.PoolOverflowBehavior
 *
 * Behavior when a fixed-size pool overflows its limit.
 * By default, a fixed-size pool will throw an Exception if it is requested to
 * deliver more instances than allowed. This can be set to throw a warning or
 * to fail silently.
 * Infinitely-sized pools automatically expand in size on overflow, based on their
 * PoolInflationType.
 */

using System;

namespace strange.extensions.pool.api
{
	public enum PoolOverflowBehavior
	{
		/// Requesting more than the fixed size will throw an exception.
		EXCEPTION,
		
		/// Requesting more than the fixed size will throw a warning.
		WARNING,

		/// Requesting more than the fixed size will return null and not throw an error.
		IGNORE
	}
}

