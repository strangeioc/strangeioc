/**
 * Interface for a SemiBinding.
 * 
 * A SemiBinding is the smallest atomic part of the Babel framework. It represents
 * either the Key or the Value or the Name arm of the binding.
 * 
 * The SemiBinding stores some value...a system Type, a list, a concrete value.
 * 
 * It also has a constraint defined by the constant ONE or MANY.
 * A constraint of ONE makes the SemiBinding maintain a singular value, rather than a list.
 * 
 * The default constraints are:
 * Key 		- 	ONE
 * Value	-	MANY
 * Name		-	ONE
 */

using System;

namespace babel.framework.api
{
	public interface ISemiBinding
	{
		Enum constraint{ get; set;}
		ISemiBinding Add(object o);
		ISemiBinding Remove(object o);
		object value{ get; }
		bool uniqueValues{get;set;}
	}
}

