using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
namespace GameEntity {
	[RequireComponent(typeof(HealthComponent))]
	public abstract class Entity : SerializedMonoBehaviour {
		// Lazyload the HealthComponent to avoid using awake (overriding in subclasses isn't clear)
		protected HealthComponent HealthComponent => healthComponent ?? gameObject.GetOrAdd<HealthComponent>();
		HealthComponent healthComponent;

		public int Health => HealthComponent.Health;
		public Vector2 Position { get; protected set; }
		public void Heal(int amount) => HealthComponent.Heal(amount);
		public void Damage(int amount) => HealthComponent.Damage(amount);
		protected abstract void Register();
	}
}
