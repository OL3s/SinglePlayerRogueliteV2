#nullable enable

using Godot;
using MyTypes;
using System;

[GlobalClass]
public partial class ItemBase : Resource {
	[Export] public string ItemName { get; set; } = "NONAME";
	[Export] public ItemDependency Dependencies { get; set; } = new();
	[Export] public Texture2D Icon { get; set; } = new PlaceholderTexture2D();
	[Export] public int MaxStackSize { get; set; } = 1;
	[Export] public int Cost { get; set; } = 0;
	[Export] public string ItemID { get; set; }
	[Export] public bool IsConsumable { get; set; } = false;
	[Export] public int UseCountMax { get; set; } = 0;
	[Export] public int UseCountDefault { get; set; } = 0;
	[Export] public int UseCountCurrent { get; set; } = 0;
	[Export] public bool HasCondition { get; set; } = false;
	[Export] public int ConditionMax { get; set; } = 0;
	[Export] public int ConditionDefault { get; set; } = 0;
	[Export] public int ConditionCurrent { get; set; } = 0;
	[Export] public AmmoType AmmoType { get; set; } = AmmoType.None;
	public bool IsStackable => MaxStackSize > 1;
	public bool IsAmmo => AmmoType != AmmoType.None;
	public bool IsEmpty => IsConsumable && UseCountCurrent <= 0;
	public bool IsBroken => HasCondition && ConditionCurrent <= 0;

	public ItemBase() {
		ItemID = Guid.NewGuid().ToString();
	}

	public ItemBase(string itemName, ItemDependency? dependencies, Texture2D icon, int maxStackSize, int cost) {
		ItemName = itemName;
		Dependencies = dependencies ?? new ItemDependency();
		Icon = icon;
		MaxStackSize = maxStackSize;
		Cost = cost;
		ItemID = Guid.NewGuid().ToString();
	}

	public bool CanUse(PlayerSkillData? playerSkills = null, AmmoType? ammoType = null, int? mana = null) {
		if (IsEmpty || IsBroken)
			return false;

		return Dependencies == null || Dependencies.CanExecute(playerSkills, ammoType, mana);
	}

	public void ResetRuntimeValues() {
		UseCountCurrent = UseCountDefault;
		ConditionCurrent = ConditionDefault;
	}

	public bool TryConsumeUse(int amount = 1) {
		if (!IsConsumable)
			return true;

		if (amount <= 0)
			return true;

		if (UseCountCurrent < amount)
			return false;

		UseCountCurrent -= amount;
		return true;
	}

	public void ApplyConditionDamage(int amount) {
		if (!HasCondition || amount <= 0)
			return;

		ConditionCurrent = Mathf.Max(0, ConditionCurrent - amount);
	}

	public override string ToString() {
		return $"{GetType().Name}(Name={ItemName}, Cost={Cost}, MaxStack={MaxStackSize}, Uses={UseCountCurrent}/{UseCountMax}, Condition={ConditionCurrent}/{ConditionMax})";
	}
}
