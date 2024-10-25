using System;
using System.Collections.Generic;
using System.Linq;
using EventBus;
using GameEntity;
using IncantationSystem.Castables;
using MusicEngine;
using Scoring;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Combat {
	public class CombatManager : SerializedMonoBehaviour {
		[HorizontalGroup("Entities")] [VerticalGroup("Entities/Left")]
		public List<Entity> GoodEntities;

		[VerticalGroup("Entities/Right")] public List<Entity> BadEntities;

		EventBinding<AttackCommand> attackCommand;

		MusicManager musicManager;

		[OdinSerialize] Queue<CombatState> combatStates;
		public CombatState CurrentCombatState => combatStates.Peek();
		public bool InCombat;
		PlayerDashHandler playerDashHandler;

		EventBinding<BeatEvent> beatEvent;

		void Awake() { ServiceLocator.ServiceLocator.Global.Register(this); }

		void Start() {
			musicManager = MusicManager.Instance;
			StartCombat();
		}

		void OnEnable() {
			attackCommand = new(OnAttack);
			beatEvent = new(OnBeat);
			EventBus<AttackCommand>.Register(attackCommand);
			EventBus<BeatEvent>.Register(beatEvent);
		}

		void OnDisable() {
			EventBus<AttackCommand>.Deregister(attackCommand);
			EventBus<BeatEvent>.Deregister(beatEvent);
		}

		public void StartCombat() {
			InitializeCombatStates();
			InCombat = true;
		}

		void OnAttack(AttackCommand @event) { Attack(@event); }

		void OnBeat(BeatEvent @event) {
			if (@event.IsDownBeat && InCombat) {
				combatStates.Dequeue();
				AddDefaultCombatState();
				playerDashHandler.Reset();
			}
		}

		void InitializeCombatStates() {
			combatStates = new();
			AddDefaultCombatState();
			AddDefaultCombatState();
			AddDefaultCombatState();
		}

		void AddDefaultCombatState() {
			if (combatStates.Count == 0) {
				combatStates.Enqueue(CombatState.Good);
				return;
			}

			combatStates.Enqueue(combatStates.Last() == CombatState.Good ? CombatState.Bad : CombatState.Good);
		}

		public void RegisterAsGoodEntity(Entity entity) {
			if (GoodEntities.Contains(entity) || BadEntities.Contains(entity)) return;
			print($"Registering <b>{entity}</b> as good entity");
			playerDashHandler = new(entity);
			GoodEntities.Add(entity);
		}

		public void RegisterAsBadEntity(Entity entity) {
			if (BadEntities.Contains(entity) || BadEntities.Contains(entity)) return;
			print($"Registering <b>{entity}</b> as bad entity");
			BadEntities.Add(entity);
		}

		public void Attack(AttackCommand command) {
			Entity entity = command.Entity;
			int damage = command.Damage;
			if (!InCombat) return;
			if (GoodEntities.Contains(entity)) {
				AttackBadEntities(damage);
			} else if (BadEntities.Contains(entity)) {
				playerDashHandler.Attack(command);
			} else {
				Debug.LogError($"Entity <b>{entity}</b> is not registered into the combat manager");
			}
		}

		void AttackBadEntities(int damage) {
			foreach (Entity badEntity in BadEntities) {
				badEntity.Damage(damage);
			}
		}

		void AttackGoodEntities(int damage) {
			foreach (Entity goodEntity in GoodEntities) {
				goodEntity.Damage(damage);
			}
		}

		void Update() { playerDashHandler.Tick(); }

		public enum CombatState {
			Good,
			Bad,
			Cadenza,
		}

		class PlayerDashHandler {
			readonly EventBinding<DashEvent> dashEvent;
			readonly Entity player;
			PlayerMovement movement;
			AttackCommand command;
			DodgeComputer dodgeComputer;

			readonly List<float> attackTimes = new();
			readonly List<float> dashTimes = new();

			public PlayerDashHandler(Entity player) {
				dashEvent = new(Dash);
				EventBus<DashEvent>.Register(dashEvent);
				ServiceLocator.ServiceLocator.Global.Get(out dodgeComputer);
				this.player = player;
			}

			~PlayerDashHandler() { EventBus<DashEvent>.Deregister(dashEvent); }

			void Dash(DashEvent @event) {
				if (movement == null) movement = @event.Movement;
				if (GetDifference() < 0) return;
				dashTimes.Add(Time.time);
				if (GetDifference() == 0) {
					ApplyDamage(GetMultiplier());
				}
				Recalculate();
			}

			public void Tick() {
				var currentTime = Time.time;
				if (attackTimes.Count == 0) return;
				if (GetDifference() > 0) {
					if (currentTime - attackTimes.Last() > 1) {
						ApplyDamage(GetMultiplier());
					}
				}
			}

			public void Attack(AttackCommand command) {
				this.command = command;
				if (GetDifference() > 0) {
					dashTimes.Add(float.MinValue);
					ApplyDamage();
				} else if (GetDifference() == 0  && attackTimes.Count > 0 && dashTimes.Count > 0) {
					ApplyDamage(GetMultiplier());
					print(GetMultiplier());
				}
				attackTimes.Add(Time.time);
				Recalculate();
			}

			public void ApplyDamage(float multiplier = 1) {
				player.Damage((int)(command.Damage * multiplier));
			}

			public void Reset() {
				if (GetDifference() > 0) {
					ApplyDamage();
				}
				attackTimes.Clear();
				dashTimes.Clear();
				Recalculate();
			}

			void Recalculate() {
				if (movement != null) {
					movement.CanDash = GetDifference() >= 0;
				}
			}

			// positive if there are more attack times than dash times
			public int GetDifference() {
				return attackTimes.Count - dashTimes.Count;
			}
			
			public float GetMultiplier() {
				if (dashTimes.Count == 0 || attackTimes.Count == 0) return 0;
				return dodgeComputer.CalculateDodge(dashTimes.Last() - attackTimes.Last()).DamageMultiplier;
			}
		}
	}
}