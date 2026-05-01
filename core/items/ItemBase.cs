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
	[Export] public ItemOutpostCompatibilityData OutpostCompatibility { get; set; }
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

	public bool CanUse(ItemUseContext context) {
		if (context == null || IsEmpty || IsBroken || !HasEnoughUses(context.UseCountCost))
			return false;

		return Dependencies == null || Dependencies.CanExecute(context);
	}

	public bool TryUse(ItemUseContext context) {
		if (!CanUse(context))
			return false;

		if (Dependencies != null && !Dependencies.ApplyCosts(context))
			return false;

		ConsumeUse(context.UseCountCost);
		ApplyConditionDamage(context.ConditionDamage);
		return true;
	}

	public void ResetRuntimeValues() {
		UseCountCurrent = UseCountDefault;
		ConditionCurrent = ConditionDefault;
	}

	internal bool ConsumeUse(int amount = 1) {
		if (!IsConsumable)
			return true;

		amount = Mathf.Max(0, amount);
		if (amount == 0)
			return true;

		if (!HasEnoughUses(amount))
			return false;

		UseCountCurrent -= amount;
		return true;
	}

	private bool HasEnoughUses(int amount) {
		return !IsConsumable || UseCountCurrent >= Mathf.Max(0, amount);
	}

	private void ApplyConditionDamage(int amount) {
		if (!HasCondition || amount <= 0)
			return;

		ConditionCurrent = Mathf.Max(0, ConditionCurrent - amount);
	}

	public override string ToString() {
		return $"{GetType().Name}(Name={ItemName}, Cost={Cost}, MaxStack={MaxStackSize}, Uses={UseCountCurrent}/{UseCountMax}, Condition={ConditionCurrent}/{ConditionMax})";
	}

	public bool HasCompatibility(OutpostBuildingCompatibility compatibility) {
		return OutpostCompatibility != null && OutpostCompatibility.HasCompatibility(compatibility);
	}

	public bool IsCompatibleWith(OutpostBuildingCompatibility compatibility) {
		return OutpostCompatibility == null || OutpostCompatibility.IsCompatibleWith(compatibility);
	}
}
