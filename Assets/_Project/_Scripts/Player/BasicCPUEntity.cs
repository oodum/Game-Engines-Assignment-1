using Combat;
using Sirenix.OdinInspector;

namespace GameEntity {
	public class BasicCPUEntity : Entity {
		CombatManager combatManager;
		void Start() {
			ServiceLocator.ServiceLocator.For(this).Get(out combatManager);
			Register();
		}

		protected override void Register() {
			combatManager.RegisterAsBadEntity(this);
		}
	}
}