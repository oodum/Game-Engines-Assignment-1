using System;
using System.Linq;
using Combat;
using EventBus;
using Extensions;
using Input;
using MusicEngine;
using Scoring;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IncantationSystem.Castables {
	[RequireComponent(typeof(IncantationCaster))]
	public class PlayerCaster : MonoBehaviour {
		IncantationCaster caster;
		IncantationQuiver quiver;
		[SerializeField, Required] PlayerInputProcessor inputProcessor;

		MusicManager musicManager;
		CombatManager combatManager;
		EventBinding<BeatEvent> beatEvent;

		[SerializeField] bool showDebugInfo;

		void Awake() {
			caster = gameObject.GetOrAdd<IncantationCaster>();
			quiver = gameObject.GetOrAdd<IncantationQuiver>();
		}

		void OnEnable() {
			inputProcessor.OnCastPressed += Cast;
			EventBus<BeatEvent>.Register(beatEvent = new(OnBeat));
		}

		void OnDisable() {
			inputProcessor.OnCastPressed -= Cast;
			EventBus<BeatEvent>.Deregister(beatEvent);
		}

		void Start() {
			inputProcessor.SetFight(); // TODO: Refactor
			musicManager = MusicManager.Instance;
			ServiceLocator.ServiceLocator.For(this).Get(out combatManager);
		}

		void Cast() {
			float timing = (float)musicManager.RelativeProgress;
			if (combatManager.CurrentCombatState != CombatManager.CombatState.Good
			    && !IsBuffered(ref timing)) return;
			var castCommand = caster.Cast((float)musicManager.RelativeProgress);
			casts += $"{musicManager.RelativeProgress:F2} ";
			castCommand.Raise();
		}

		bool IsBuffered(ref float timing) {
			if (quiver.Incantations.Any(i => i.IsComplete)) return false;
			float inverseTiming = Mathf.Abs(musicManager.BarLength - timing);
			if (Mathf.Approximately(inverseTiming, 4)) return true;
			if (musicManager.ConvertProgressToTime(inverseTiming) <= TimingComputer.MISS_THRESHOLD) {
				timing = -inverseTiming;
				return true;
			}
			return false;
		}

		void OnBeat(BeatEvent @event) {
			if (@event.IsDownBeat) casts = String.Empty;
		}

		string casts = String.Empty;

		void OnGUI() {
			if (!showDebugInfo) return;
			GUI.Label(new(new(200, 130), new(400, 1000)), casts);
		}
	}
}