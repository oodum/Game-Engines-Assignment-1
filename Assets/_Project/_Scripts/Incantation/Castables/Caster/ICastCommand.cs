using EventBus;
using GameEntity;
using UnityEngine;
namespace IncantationSystem.Castables {
	public interface ICastCommand : IEvent {
		public int Damage { get; }
		public CastableIncantation Incantation { get; }
		public float Confidence { get; }
		public Entity Entity { get; }
		public void Cast(Entity receiver);
		void Raise();
	}

	public readonly struct CastCommand : ICastCommand {
		public int Damage { get; }
		public CastableIncantation Incantation { get; }
		public float Confidence { get; }
		public Entity Entity { get; }

		public CastCommand(Entity entity, CastableIncantation incantation) {
			Damage = 0;
			Incantation = incantation;
			Confidence = incantation.Confidence;
			Entity = entity;
		}

		public void Cast(Entity receiver) {
			receiver.Damage(Damage);
		}
		public void Raise() {
			EventBus<CastCommand>.Raise(this);
		}
	}

	public readonly struct AttackCommand : ICastCommand {
		public int Damage { get; }
		public CastableIncantation Incantation { get; }
		public float Confidence { get; }
		public Entity Entity { get; }

		public AttackCommand(Entity entity, CastableIncantation incantation, int damage) {
			Damage = damage;
			Incantation = incantation;
			Confidence = incantation.Confidence;
			Entity = entity;
		}

		public void Cast(Entity receiver) {
			receiver.Damage(Damage);
		}

		public void Raise() {
			EventBus<AttackCommand>.Raise(this);
		}
	}

	public struct NullCastCommand : ICastCommand {
		public int Damage => 0;
		public CastableIncantation Incantation => null;
		public float Confidence => 0;
		public Entity Entity => null;

		public void Cast(Entity receiver) {
			// noop
		}
		public void Raise() {
			// noop
		}
	}
}
