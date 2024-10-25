﻿namespace EventBus {
	public interface IEvent { }

	public struct TestEvent : IEvent { }

	public struct PlayerEvent : IEvent {
		public int Health;
	}
}
