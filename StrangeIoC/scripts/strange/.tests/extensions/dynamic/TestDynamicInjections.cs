
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
			String shipId = Guid.NewGuid().ToString();

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
			Assert.Fail("NYI");
		}


		class DynamicallyInjected : IDynamicallyInjected
		{

			[DynamicInject] 
			public IShip ship { get; set; }

			[DynamicInject] 
			public IWeapon weapon { get; set; }

			public string Id { get; set; }
			public object getDynamicInjectId()
			{
				return Id;
			}
		}

		interface IShip
		{
			string ShipId { get; set; }
		}

		interface IWeapon
		{
			string ShipId { get; set; }
		}

		class Ship : IShip
		{
			public string ShipId { get; set; }
		}

		class Weapon : IWeapon
		{
			public string ShipId { get; set; }
		}


	}
}
