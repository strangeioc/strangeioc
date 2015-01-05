
using System;
using NUnit.Core;
using NUnit.Framework;
using strange.extensions.injector.api;
using strange.extensions.injector.impl;
using strange.extensions.localInject;

namespace strange.tests.extensions.dynamic
{
	[TestFixture]
	public class TestDynamicInjections
	{
		private IInjectionBinder injectionBinder;

		[SetUp]
		public void SetUp()
		{
			injectionBinder = new InjectionBinder();
		}

		[TearDown]
		public void TearDown()
		{
			
		}

		[Test]
		public void TestDynamic()
		{
			Guid shipId = Guid.NewGuid();

			IShip ship = new Ship();
			ship.ShipId = shipId;

			IWeapon weapon = new Weapon();
			weapon.ShipId = shipId;

			injectionBinder.Bind<IShip>().To(ship).ToName(shipId);
			injectionBinder.Bind<IWeapon>().To(weapon).ToName(shipId);

			DynamicallyInjected instance = new DynamicallyInjected();
			instance.Id = shipId;
			injectionBinder.injector.Inject(instance);

			Assert.AreEqual(instance.ship.ShipId, shipId);
			Assert.AreEqual(instance.weapon.ShipId, shipId);
		}

		[Test]
		public void TestNormalInjectionCheckedForDynamicId()
		{
			Guid shipId = Guid.NewGuid();

			IShip ship = new Ship();
			ship.ShipId = shipId;

			IWeapon weapon = new Weapon();
			weapon.ShipId = shipId;

			MyIdModel idModel = new MyIdModel();
			idModel.Id = shipId;

			injectionBinder.Bind<MyIdModel>().To(idModel);
			injectionBinder.Bind<IShip>().To(ship).ToName(shipId);
			injectionBinder.Bind<IWeapon>().To(weapon).ToName(shipId);

			DynamicallyInjectedNeedsInjectionForId instance = new DynamicallyInjectedNeedsInjectionForId();
			injectionBinder.injector.Inject(instance);

			Assert.AreEqual(instance.ship.ShipId, shipId);
			Assert.AreEqual(instance.weapon.ShipId, shipId);
		}


		class DynamicallyInjected : IDynamicallyInjected
		{

			[DynamicInject] 
			public IShip ship { get; set; }

			[DynamicInject] 
			public IWeapon weapon { get; set; }

			public Guid Id { get; set; }
			public object getDynamicInjectId()
			{
				return Id;
			}
		}

		class DynamicallyInjectedNeedsInjectionForId : IDynamicallyInjected
		{
			[Inject]
			public MyIdModel MyIdModel { get; set; }

			[DynamicInject] 
			public IShip ship { get; set; }

			[DynamicInject] 
			public IWeapon weapon { get; set; }

			public object getDynamicInjectId()
			{
				return MyIdModel.Id;
			}
		}

		class MyIdModel
		{
			public Guid Id;
		}

		interface IShip
		{
			Guid ShipId { get; set; }
		}

		interface IWeapon
		{
			Guid ShipId { get; set; }
		}

		class Ship : IShip
		{
			public Guid ShipId { get; set; }
		}

		class Weapon : IWeapon
		{
			public Guid ShipId { get; set; }
		}


	}
}
