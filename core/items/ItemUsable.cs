#nullable enable

using Godot;

[GlobalClass]
public partial class ItemUsable : ItemBase {
	[Export] public PlayerAction? Action { get; set; }

	public ItemUsable() { }

	public ItemUsable(string itemName, ItemDependency? dependencies, PlayerAction? action, Texture2D icon, int maxStackSize, int cost)
		: base(itemName, dependencies, icon, maxStackSize, cost) {
		Action = action;
	}

	public bool CanUse(ItemUseContext context) {
		if (context == null || IsEmpty || IsBroken || !HasEnoughUses(context.UseCountCost))
			return false;

		if (Dependencies != null && !Dependencies.CanExecute(context))
			return false;

		return Action == null || Action.CanExecute(context);
	}

	public bool TryUse(ItemUseContext context) {
		if (!CanUse(context))
			return false;

		context.Item ??= this;

		if (Dependencies != null && !Dependencies.ApplyCosts(context))
			return false;

		if (Action != null && !Action.Execute(context))
			return false;

		ConsumeUse(context.UseCountCost);
		ApplyConditionDamage(context.ConditionDamage);
		return true;
	}
}
