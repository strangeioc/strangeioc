using System;
using UnityEngine;
using babel.extensions.context.api;

namespace babel.extensions.context.impl
{
	public class ContextView : MonoBehaviour, IContextView
	{
		public IContext context{get;set;}
		
		public ContextView ()
		{
		}
	}
}

