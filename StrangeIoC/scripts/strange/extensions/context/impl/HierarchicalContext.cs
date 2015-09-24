using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.injector.impl;

namespace strange.extensions.context.impl
{
	public class HierarchicalContext : Context
	{

		public HierarchicalContext() : base()
		{}

		public HierarchicalContext(object view) : base(view)
		{
		}

		public HierarchicalContext(object view, ContextStartupFlags flags) : base(view, flags)
		{
		}

		public HierarchicalContext(object view, bool autoMapping)
			: base(view, autoMapping)
		{
		}

		private HierarchicalInjectionBinder _injectionBinder = new HierarchicalInjectionBinder();

		/// A Binder that handles dependency injection binding and instantiation
		public HierarchicalInjectionBinder injectionBinder 
		{
			get { return _injectionBinder;}
			set { _injectionBinder = value; }
		}

		protected readonly HashSet<IContext> ChildContexts = new HashSet<IContext>();

		public override IContext AddContext(IContext context)
		{
			ChildContexts.Add(context);
			HierarchicalContext hierarchicalContext = context as HierarchicalContext;
			if (hierarchicalContext != null)
			{
				//First, supply any existing inherited bindings to the child context
				hierarchicalContext.injectionBinder.SupplyInheritedBindings(injectionBinder.InheritedBindings);
				
				//Next, assign delegates to resolve and unbind inherited bindings properly
				injectionBinder.NewInheritedBinding += hierarchicalContext.injectionBinder.ResolveBinding;
				injectionBinder.RemovedInheritedBinding += hierarchicalContext.injectionBinder.Unbind;
			}
			return base.AddContext(context);
		}

		public override IContext RemoveContext(IContext context)
		{
			ChildContexts.Remove(context);

			//Cleanup
			HierarchicalContext hierarchicalContext = context as HierarchicalContext;
			if (hierarchicalContext != null)
			{
				hierarchicalContext.injectionBinder.RemoveInheritedBindings(injectionBinder.InheritedBindings);
				injectionBinder.NewInheritedBinding -= hierarchicalContext.injectionBinder.ResolveBinding;
				injectionBinder.RemovedInheritedBinding -= hierarchicalContext.injectionBinder.Unbind;
			}
			return base.RemoveContext(context);
		}
	}
}
