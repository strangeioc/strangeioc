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
 * @interface strange.framework.api.ISemiBinding
 * 
 * A managed list of values.
 * 
 * A SemiBinding is the smallest atomic part of the strange framework. It represents
 * either the Key or the Value or the Name arm of the binding.
 * <br />
 * The SemiBinding stores some value...a system Type, a list, a concrete value.
 * <br />
 * It also has a constraint defined by the constant ONE or MANY.
 * A constraint of ONE makes the SemiBinding maintain a singular value, rather than a list.
 * <br />
 * The default constraints are:
 * <ul>
 *  <li>Key - ONE</li>
 *  <li>Value - MANY</li>
 *  <li>Name - ONE</li>
 * </ul>
 * 
 * @see strange.framework.api.BindingConstraintType
 */

using System;

namespace strange.framework.api
{
	public interface ISemiBinding : IManagedList
	{
		/// Set or get the constraint. 
		BindingConstraintType constraint{ get; set;}

		/// A secondary constraint that ensures that this SemiBinding will never contain multiple values equivalent to each other. 
		bool uniqueValues{get;set;}
	}
}

