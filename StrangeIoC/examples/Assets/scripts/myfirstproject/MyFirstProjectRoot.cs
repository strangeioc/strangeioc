/// The Root is the entry point to a strange-enabled Unity3D app.
/// ===============
/// 
/// Attach this MonoBehaviour to a GameObject at the top of your Main scene.
/// 
/// It is considered a best practice to create ONE GameObject at the top of your 
/// app and attach everything to it. (Note that you can create multiple Roots which 
/// will result in multiple Contexts.  This can be desirable, but it's an advanced use case.
/// Recommend you stick to a single Context until you're confident you know what you're doing.
/// 
/// The GameObject to which this MonoBehaviour is attached to called the 'ContextView'
/// and is injectable anywhere in the application. This is especially
/// useful in commands, where you can access the ContextView to attach further GameObjects 
/// or MonoBehaviours.

using System;
using UnityEngine;
using strange.extensions.context.impl;

namespace strange.examples.myfirstproject
{
	public class MyFirstProjectRoot : ContextView
	{
	
		void Awake()
		{
			//Instantiate the context, passing it this instance.
			context = new MyFirstContext(this);

			//This is the most basic of startup choices, and probably the most common.
			//You can also opt to pass in ContextStartFlag options, such as:
			//
			//context = new MyFirstContext(this, ContextStartupFlags.MANUAL_MAPPING);
			//context = new MyFirstContext(this, ContextStartupFlags.MANUAL_MAPPING | ContextStartupFlags.MANUAL_LAUNCH);
			//
			//These flags allow you, when necessary, to interrupt the startup sequence.
		}
	}
}

