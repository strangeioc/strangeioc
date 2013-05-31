using System;
using UnityEngine;
using strange.extensions.context.api;

namespace strange.extensions.context.impl
{
	public class ContextView : MonoBehaviour, IContextView
	{
		public IContext context{get;set;}
		
		public ContextView ()
		{
		}
	}
}

