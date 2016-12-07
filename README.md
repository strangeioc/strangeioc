## Strange: the IoC framework for `Unity3D` and `C#`

Current version: v1.0.0

Website: http://strangeioc.github.io/strangeioc/


> Strange attractors create predictable patterns, often in chaotic systems.

Strange is a super-lightweight and highly extensible Inversion-of-Control (IoC) framework, written specifically for C# and Unity. We've validated Strange on web, standalone, and iOS, and Android.

* [Overview](http://strangeioc.github.com/strangeioc/exec.html)
* [StrangeIoC documentation](http://strangeioc.github.com/strangeioc/docs/html/index.html)
* [The Big, Strange How-To](http://strangeioc.github.com/strangeioc/TheBigStrangeHowTo.html)
* [Do you use RobotLegs? You're in luck! This Strange page is for you!](http://strangeioc.github.com/strangeioc/rl.html)
* [FAQ](http://strangeioc.github.com/strangeioc/faq.html)
* [Feature requests/Bug reports](https://github.com/strangeioc/strangeioc/issues)

It contains the following features, most of which are optional:

* A core binding framework that pretty much lets you bind one or more of anything to one or more of anything else.
* Dependency Injection
  * Map as singleton, value or factory (get a new instance each time you need one)
  * Name injections and/or supply specific implementations to specific consumer classes
  * Perform constructor or setter injection
  * Tag your preferred constructor
  * Tag a method to fire after construction
  * Inject into MonoBehaviours
  * Bind polymorphically (bind any or all of your interfaces to a single concrete class)
* Reflection binding dramatically reduces overhead of employing reflectivity
* Two shared event bus systems, EventDispatcher and Signals. Both allow you to:
  * Dispatch events to any point in your application
  * Map local events for local communication
  * Map events to Commands classes to separate business logic
  * EventDispatcher transmits data payloads as primitives or ValueObjects
  * Signals transmits data in bindable, type-safe parameters
* MonoBehaviour mediation
  * Facilitate separation of a view from the application using it
  * Keep Unity-specific code isolated from the rest of the app
* Optional MVCS (Model/View/Controller/Service) structure
* Multiple contexts
  * Allow subcomponents (separate Scenes) to function on their own, or in the context of larger apps.
  * Allow communication between contexts.
* Promises
  * Similar to Javascript Q-Promises, these help control flow and error handling 
  * Promises also fit some common signal use cases much more cleanly!
* Annotated 'Implicit' Bindings
 * Reduce boiler plate code written in your Context and Mediators!
* JSON-driven bindings
 * Dynamically load your bindings at runtime!
* Don't see what you need? The core binding framework is simple to extend. Build new Binders like:
  * A different type of dispatcher, like AS3-Signals <- WAIT A MOMENT! WE DID EXACTLY THAT!!!
  * An entity framework
  * A multi-loader

In addition to organizing your project into a sensible structure, Strange offers the following benefits:

* Designed to play well with Unity3D. Also designed to play well without it.
* Separate UnityEngine code from the rest of your app.
  * Improves portability
  * Improves unit testability
* A common event bus makes information flow easy and highly decoupled. (Note: Unity's SendMessage method does this, of course, but it's dangerous as all get-out. I may write a whole article on just this topic at some point.)
* The extensible binder really is amazing (a friend used to tell me "it's good to like your own cookin'"). The number of things you can accomplish with the tiny core framework would justify Strange all on its own.
* Multiple contexts allow you to "bootstrap" subcomponents so they operate fine either on their own or as an integrated part. This can hugely speed up your development process and allow developers to work in isolation, then integrate in later stages of development.
* Get rid of platform-specific #IF...#ELSE constructs in your code. IoC allows you to write whole concrete implementations correctly, then bind the ones you want to use at compile time or at run time. (As with other forms of binding, #IF...#ELSE clauses can be isolated into a single class and away from the rest of your code.)

### Supported Versions/Platforms
Strange works with Unity 3.5+. [Click here](http://strangeioc.github.io/strangeioc/faq.html#supported-platforms) to see the full list of supported runtime platforms.

# Acknowledgements
It is hard to adequately credit the creators of the open source Actionscript framework RobotLegs for their influence on the creation of StrangeIoC. While Strange is not a port of RobotLegs, the ensigns of that library are copiously reflected throughout this one. For their great service to my professional development, I offer that team my sincerest thanks. And a donut. Seriously, if you're ever in town, let me buy you a donut.

Kudos to Will Corwin for picking up a thrown-down gauntlet and writing the Signals and Implicit Bindings implementations.

I also need to thank and congratulate the folks at [ThirdMotion](http://www.thirdmotion.com) who inexplicably gave me time to build Strange and license to open source it.

/**********************************************/
  Copyright 2013 ThirdMotion, Inc.
 
 	Licensed under the Apache License, Version 2.0 (the "License");
 	you may not use this file except in compliance with the License.
 	You may obtain a copy of the License at
 
 		http://www.apache.org/licenses/LICENSE-2.0
 
 		Unless required by applicable law or agreed to in writing, software
 		distributed under the License is distributed on an "AS IS" BASIS,
 		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 		See the License for the specific language governing permissions and
 		limitations under the License.
 
