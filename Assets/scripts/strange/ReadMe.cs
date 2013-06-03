/** \mainpage StrangeIoC
 *
 * \section intro_sec Introduction
 *
 * Strange is a super-lightweight and highly extensible Inversion-of-Control (IoC) framework, written specifically for C# and Unity. We’ve validated Strange on web and standalone, iOS and Android. It contains the following features, most of which are optional:
 * <ul>
 *  <li>A core binding framework that pretty much lets you bind one or more of anything to one or more of anything else.</li>
 *  <li>Dependency Injection</li>
 *  <li>A shared event bus</li>
 *  <li>MonoBehaviour mediation</li>
 *  <li>Optional MVCS (Model/View/Controller/Service) structure</li>
 *  <li>Multiple contexts</li>
 *  <li>Don’t see what you need? The core binding framework is simple to extend. Build new Binders like:</li>
 * </ul>
 * In addition to organizing your project into a sensible structure, Strange offers the following benefits:
 * <ul>
 *  <li>Designed to play well with Unity3D. Also designed to play well without it.</li>
 *  <li>Separate UnityEngine code from the rest of your app, improving portability and unit testability.</li>
 *  <li>A common event bus makes information flow easy and highly decoupled.</li>
 *  <li>The extensible binder really is amazing (a friend used to tell me “it’s good to like your own cookin’!”). The number of things you can accomplish with the tiny core framework would justify Strange all on its own.</li>
 *  <li>Multiple contexts allow you to “bootstrap” subcomponents so they operate fine either on their own or in-app. This can hugely speed up your development process and allow developers to work in isolation, then integrate in later stages of development.</li>
 * </ul>
 * An executive overview of the benefits of IoC in general and StrangeIoC in particular is available here: <a href="">TODO</a>
 * <br />
 * Some fun documentation and tutorials are available here: <a href="">TODO</a>.
 */