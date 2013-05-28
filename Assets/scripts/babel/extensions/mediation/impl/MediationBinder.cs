using System;
using UnityEngine;
using babel.extensions.injector.api;
using babel.extensions.mediation.api;
using babel.framework.api;
using babel.framework.impl;

namespace babel.extensions.mediation.impl
{
	public class MediationBinder : Binder, IMediationBinder
	{

		[Inject]
		public IInjectionBinder injectionBinder{ get; set;}

		public MediationBinder ()
		{
		}


		public override IBinding GetRawBinding ()
		{
			return new MediationBinding (resolver) as IBinding;
		}

		public void Trigger(MediationEvent evt, MonoBehaviour view)
		{
			injectionBinder.injector.Inject (view);
			
			Type viewType = view.GetType();
			IMediationBinding binding = GetBinding (viewType) as IMediationBinding;
			if (binding != null)
			{

				switch(evt)
				{
					case MediationEvent.AWAKE:
						mapView (view, binding);
						break;
					case MediationEvent.DESTROYED:
						unmapView (view, binding);
						break;
					default:
						break;
				}
			}
		}

		public override IBinding Bind<T> ()
		{
			injectionBinder.Bind<T> ().To<T>();
			return base.Bind<T> ();
		}

		private void mapView(MonoBehaviour view, IMediationBinding binding)
		{
			Type viewType = view.GetType();

			if (bindings.ContainsKey(viewType))
			{
				object[] values = binding.value as object[];
				int aa = values.Length;
				for (int a = 0; a < aa; a++)
				{
					Type mediatorType = values [a] as Type;
					IMediator mediator = view.gameObject.AddComponent(mediatorType) as IMediator;
					mediator.setViewComponent (view);
					mediator.preRegister ();
					injectionBinder.injector.Inject (mediator);
					mediator.onRegister ();
				}
			}
		}

		private void unmapView(MonoBehaviour view, IMediationBinding binding)
		{
			Type viewType = view.GetType();

			if (bindings.ContainsKey(viewType))
			{
				object[] values = binding.value as object[];
				int aa = values.Length;
				for (int a = 0; a < aa; a++)
				{
					Type mediatorType = values[a] as Type;
					IMediator mediator = view.GetComponent(mediatorType) as IMediator;
					if (mediator != null)
					{
						mediator.onRemove();
					}
				}
			}
		}

		private void enableView(MonoBehaviour view)
		{
		}

		private void disableView(MonoBehaviour view)
		{
		}
	}
}

