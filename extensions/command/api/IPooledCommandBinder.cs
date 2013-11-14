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
 *	@interface strange.extensions.command.api.IPooledCommandBinder
 *
 *	Interface for a CommandBinder that allows pooling. Pooling allows Commands to
 *	be recycled, which can be more efficient.
 */

using System;
using System.Collections.Generic;
using strange.extensions.pool.impl;
using strange.extensions.command.impl;

namespace strange.extensions.command.api
{
	public interface IPooledCommandBinder
	{
		/// Retrieve the Pool of the specified type
		Pool<T> GetPool<T>();

		/// Switch to disable pooling for those that don't want to use it.
		bool usePooling{ get; set; }
	}
}

