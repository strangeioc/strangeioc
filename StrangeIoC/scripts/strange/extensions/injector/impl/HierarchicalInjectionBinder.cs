using System;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.framework.api;

namespace strange.extensions.injector.impl
{
	public class HierarchicalInjectionBinder : InjectionBinder, IInjectionBinder
	{
		private readonly List<IInjectionBinding> _inheritedBindings = new List<IInjectionBinding>();
		
		public IEnumerable<IInjectionBinding> InheritedBindings
		{
			get { return _inheritedBindings; }
		}

		public Action<IInjectionBinding, object> NewInheritedBinding = delegate { };
		public Action<IInjectionBinding> RemovedInheritedBinding = delegate { };

		public override void ResolveBinding(IBinding binding, object key)
		{
			//Remove existing inherited binding, unbind children
			var existingInheritedBinding = GetBinding(key);
			if (existingInheritedBinding != null && existingInheritedBinding.isInherited)
			{
				//Unbind any existing inherited binding for this and all child contexts
				Unbind(existingInheritedBinding);
			}

			base.ResolveBinding(binding, key);
			var injectionBinding = (IInjectionBinding) binding;
			
			if (injectionBinding.isInherited)
			{
				
				NewInheritedBinding(injectionBinding, key);
				_inheritedBindings.Add(injectionBinding);
			}
		}

		public void SupplyInheritedBindings(IEnumerable<IInjectionBinding> bindings)
		{
			foreach (var binding in bindings)
			{
				object[] objects = binding.key as object[];
				object key = objects != null ? objects[0] : binding.key;
				//The key should be an object[] at this point, but just in case we support a binding with a single key here
				ResolveBinding(binding, key);
			}
		}

		public override void Unbind(IBinding binding)
		{
			var injectionBinding = (IInjectionBinding) binding;

			if (injectionBinding.isInherited)
			{
				//Remove *only* an inherited binding. Verify bindings are equal
				//If we have overridden locally, we should not remove
				object[] objects = binding.key as object[];
				object key = objects != null ? objects[0] : binding.key;
				IInjectionBinding existingBinding = GetBinding(key);
				if (existingBinding == injectionBinding)
				{
					base.Unbind(key, existingBinding.name);
					RemovedInheritedBinding(existingBinding);
				}

				//Always remove the inherited binding here.
				_inheritedBindings.Remove(injectionBinding);

			}
		}


		public void RemoveInheritedBindings(IEnumerable<IInjectionBinding> bindings)
		{
			foreach (var binding in bindings)
			{
				Unbind(binding);
			}
		}
	}
}
