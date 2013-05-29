/// MainContext maps the Context for the top-level component.
/// ===========
/// I'm assuming here that you've already gone through myfirstproject, or that
/// you're experienced with Babel.

using System;
using UnityEngine;
using babel.extensions.context.api;
using babel.extensions.context.impl;
using babel.extensions.dispatcher.eventdispatcher.api;
using babel.extensions.dispatcher.eventdispatcher.impl;

using babel.examples.multiplecontexts.main.view;
using babel.examples.multiplecontexts.main.controller;

namespace babel.examples.multiplecontexts.main
{
	public class MainContext : MVCSContext
	{
		
		
		public MainContext () : base()
		{
		}
		
		public MainContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			
			
			
			
			
			
			mediationBinder.Bind<ExampleView>().To<ExampleMediator>();
			
			commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once();
			commandBinder.Bind(ExampleEvent.LOAD_SCENE).To<LoadSceneCommand>();
		}
	}
}

