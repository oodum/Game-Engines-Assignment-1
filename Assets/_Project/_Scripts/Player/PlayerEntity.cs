using System;
using Combat;
using IncantationSystem.Castables;
using UnityEngine;
namespace GameEntity {
	[RequireComponent(typeof(PlayerCaster), typeof(PlayerMovement))]
	public class PlayerEntity : Entity {
		CombatManager combatManager;
		void Start() {
			ServiceLocator.ServiceLocator.For(this).Get(out combatManager);
			Register();
		}
		protected override void Register() {
			combatManager.RegisterAsGoodEntity(this);
		}
	}
}
