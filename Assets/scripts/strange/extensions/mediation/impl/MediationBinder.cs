using System;
using UnityEngine;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.framework.api;
using strange.framework.impl;

namespace strange.extensions.mediation.impl
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
			//All views have potential to be injected, regardless of whether they are mediated
			if (evt == MediationEvent.AWAKE)
			{
				injectionBinder.injector.Inject (view);
			}
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

