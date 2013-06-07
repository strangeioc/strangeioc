/// The Context is where all the magic really happens.
/// ===========
/// Other than copying the constructors, all you really need to do when you create
/// your context is override Context or one of its subclasses, then set up
/// your list of mappings.
/// 
/// In an MVCSContext, like the one we're using, there are three types of
/// available mappings:
/// 1. Dependency Injection - Bind your dependencies to injectionBinder.
/// 2. View/Mediator Binding - Bind MonoBehaviours on your GameObjects to Mediators that speak to the rest of the app
/// 3. Event Binding - Bind Events to any/all of the following:
/// 		- Event/Method Binding -	Firing the event will trigger the method(s).
/// 		- Event/Command Binding -	Firing the event will instantiate the Command(s) and run its Execute() method.
/// 		- Event/Sequence Binding -	Firing the event will instantiate a Command(s), run its Execute() method, and,
/// 									unless the sequence is interrupted, fire each subsequent Command until the
/// 									sequence is complete.

using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;

namespace strange.examples.myfirstproject
{
	public class MyFirstContext : MVCSContext
	{
		
		
		public MyFirstContext () : base()
		{
		}
		
		public MyFirstContext (MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
		{
		}
		
		protected override void mapBindings()
		{
			//Injection binding.
			//Map a mock model and a mock service, both as Singletons
			injectionBinder.Bind<IExampleModel>().To<ExampleModel>().ToSingleton();
			injectionBinder.Bind<IExampleService>().To<ExampleService>().ToSingleton();

			//View/Mediator binding
			//This Binding instantiates a new ExampleMediator whenever as ExampleView
			//Fires its Awake method. The Mediator communicates to/from the View
			//and to/from the App. This keeps dependencies between the view and the app
			//separated.
			mediationBinder.Bind<ExampleView>().To<ExampleMediator>();
			
			//Event/Command binding
			commandBinder.Bind(ExampleEvent.REQUEST_WEB_SERVICE).To<CallWebServiceCommand>();
			//The START event is fired as soon as mappings are complete.
			//Note how we've bound it "Once". This means that the mapping goes away as soon as the command fires.
			commandBinder.Bind(ContextEvent.START).To<StartCommand>().Once ();

		}
	}
}

