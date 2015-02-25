using NUnit.Framework;
using strange.extensions.context.impl;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.listBind.api;
using strange.extensions.listBind.impl;
using System.Collections.Generic;

namespace strange.unittests
{
    [TestFixture]
    public class TestListBinder
    {
        private MockContext context;
        private IListBinder binder;
        private IInjectionBinder injectionBinder;
        private object contextView;

        [SetUp]
        public void SetUp()
        {
            Context.firstContext = null;
            contextView = new object();
            context = new MockContext(contextView, true);
            context.Start();
            binder = context.listBinder;
            injectionBinder = context.injectionBinder;

            injectionBinder.Bind<ListItemDependency>().To<ListItemDependency>();
        }

        [Test]
        public void TestBindToListButNotToInjectionBinder()
        {
            binder.Bind<IListItem>().To<ListItemA>();
            var list = context.injectionBinder.GetInstance<IList<IListItem>>();
            var listInstance = list[0];
            IListItem instance = null;
            bool exception = false;
            try
            {
                instance = context.injectionBinder.GetInstance<IListItem>();
            } catch(InjectionException e)
            {
                exception = true;
            }
            Assert.True(exception);
            Assert.NotNull(listInstance);
            Assert.Null(instance);
        }

        [Test]
        public void TestBindDuplicateType()
        {
            binder.Bind<IListItem>().To<ListItemA>();
            binder.Bind<IListItem>().To<ListItemA>();
            var list = context.injectionBinder.GetInstance<IList<IListItem>>();
            Assert.AreEqual(2, list.Count);
            Assert.IsAssignableFrom(typeof(ListItemA), list[0]);
            Assert.IsAssignableFrom(typeof(ListItemA), list[1]);
        }



        [Test]
        public void TestInjectListWithValues()
        {
            binder.Bind<string>().ToValue("a");
            binder.Bind<string>().ToValue("b");
            binder.Bind<string>().ToValue("c");

            var strings = context.injectionBinder.GetInstance<IList<string>>();

            Assert.AreEqual(3, strings.Count);
            Assert.AreEqual("a", strings[0]);
            Assert.AreEqual("b", strings[1]);
            Assert.AreEqual("c", strings[2]);
        }

        [Test]
        public void TestInjectListWithDuplicateValues()
        {
            binder.Bind<string>().ToValue("a");
            binder.Bind<string>().ToValue("b");
            binder.Bind<string>().ToValue("a");

            var strings = context.injectionBinder.GetInstance<IList<string>>();

            Assert.AreEqual(3, strings.Count);
            Assert.AreEqual(strings[0], strings[2]);
        }


        [Test]
        public void TestInjectTypesWithDependenciesToList()
        {
            binder.Bind<IListItem>().To<ListItemA>();
            binder.Bind<IListItem>().To<ListItemB>();

            var list = context.injectionBinder.GetInstance<IList<IListItem>>();

            Assert.AreEqual(2, list.Count);
            Assert.IsAssignableFrom(typeof(ListItemA), list[0]);
            Assert.NotNull(list[0].Dep);
            Assert.IsAssignableFrom(typeof(ListItemB), list[1]);
            Assert.NotNull(list[1].Dep);
        }


        [Test]
        public void TestInjectListWithSingletonItems()
        {
            
            binder.Bind<IListItem>().To<ListItemA>().ToSingleton();
            binder.Bind<IListItem>().To<ListItemB>();

            var list1 = context.injectionBinder.GetInstance<IList<IListItem>>();
            var list2 = context.injectionBinder.GetInstance<IList<IListItem>>();

            Assert.AreNotEqual(list1, list2);

            // Singleton
            Assert.AreEqual(list1[0], list2[0]);

            // New instance
            Assert.AreNotEqual(list1[1], list2[1]);
            
        }

        [Test]
        public void TestInjectListWithMixedValuesAndTypes()
        {
            binder.Bind<IListItem>().ToValue(new ListItemA(new ListItemDependency()));
            binder.Bind<IListItem>().To<ListItemB>();

            var list1 = context.injectionBinder.GetInstance<IList<IListItem>>();
            var list2 = context.injectionBinder.GetInstance<IList<IListItem>>();

            Assert.AreNotEqual(list1, list2);

            // Bound by value
            Assert.AreEqual(list1[0], list2[0]);

            // New instance
            Assert.AreNotEqual(list1[1], list2[1]);

        }


        [TestCase]
        public void TestInjectListAsSingleton()
        {
            
            binder.Bind<IListItem>().ToSingleton();
            binder.Bind<IListItem>().ToValue(new ListItemA(new ListItemDependency()));
            binder.Bind<IListItem>().To<ListItemB>();

            var list1 = context.injectionBinder.GetInstance<IList<IListItem>>();
            var list2 = context.injectionBinder.GetInstance<IList<IListItem>>();

            // No matter how list items were bound they are all equal between 
            // list1 and list2 because the list itself is bound as a singleton
            Assert.AreEqual(list1, list2);
            Assert.AreEqual(list1[0], list2[0]);
            Assert.AreEqual(list1[1], list2[1]);
            
        }

        [Test]
        public void TestInjectSingletonToListAndStandalone()
        {
            binder.Bind<IListItem>().To<ListItemA>();
            binder.Bind<IListItem>().To<ListItemB>();
            binder.Bind<IListItem>().To<ListItemC>();

            injectionBinder.Bind<ListItemB>().To<ListItemB>().ToSingleton();
            injectionBinder.Bind<IListItem>().To<ListItemA>().ToSingleton();
   
            var list = context.injectionBinder.GetInstance<IList<IListItem>>();
            var instanceA = context.injectionBinder.GetInstance<IListItem>();
            var instanceB = context.injectionBinder.GetInstance<ListItemB>();

            // The instace was bound as a singleton so list will contain that singleton 
            Assert.AreEqual(list[0], instanceA);
            Assert.AreEqual(list[1], instanceB);
            Assert.IsAssignableFrom(typeof(ListItemC), list[2]);
            

        }
    }



    public interface IListItem
    {
        ListItemDependency Dep { get; }
    }


    public class ListItemA : IListItem
    {
        private readonly ListItemDependency dep;
        public ListItemDependency Dep { get { return this.dep; } }
        [Construct]
        public ListItemA(ListItemDependency dep)
        {
            this.dep = dep;
        }

    }
    public class ListItemB: IListItem
    {
        [Inject]
        public ListItemDependency Dep { get; set; }

    }

    public class ListItemC : IListItem
    {
        [Inject]
        public ListItemDependency Dep { get; set; }
    }

    public class ListItemWithInvalidConstructor : IListItem
    {
        private readonly ListItemDependency dep;
        public ListItemDependency Dep { get { return this.dep; } }
        public ListItemWithInvalidConstructor(ListItemDependency dep)
        {
            this.dep = dep;
        }
        public ListItemWithInvalidConstructor(ListItemDependency dep, string hops)
        {
            this.dep = dep;
        }
    }


    public class ListItemDependency
    {

    }
}
