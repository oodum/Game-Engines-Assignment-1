using System;
using GameEntity;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour {
	Image image;
	[SerializeField] Entity entity;
	[SerializeField] int maxHealth;

	void Awake() {
		image = GetComponent<Image>();
	}


	void Update() {
		image.fillAmount = (float)entity.Health / maxHealth;
	}
}
