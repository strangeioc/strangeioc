/// The Class is for development and testing.
/// ===============
/// If you're looking for stuff to get you started, try MyFirstProjectRoot

using System;
using UnityEngine;
using strange.extensions.context.impl;

namespace strange.examples.justtests
{
	public class TesterContextView : ContextView
	{

		void Awake()
		{
			context = new TesterContext(this);
		}
	}
}

