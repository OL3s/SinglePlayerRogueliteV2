using Godot;
using Godot.Collections;
using MyTypes;
using SaveData;

[GlobalClass]
public partial class ItemDependency : Resource {
	[Export] public Array<Dependency> Dependencies { get; set; } = new();

	public void AddDependency(Dependency dependency) {
		if (dependency == null)
			return;

		Dependencies.Add(dependency);
	}

	internal bool CanExecute(ItemUseContext context = null, PlayerRuntimeStats playerStats = null) {
		context = PrepareContext(context, playerStats);

		if (Dependencies == null) {
			GD.PushError("ItemDependency: Dependencies array is missing.");
			return false;
		}

		foreach (var dependency in Dependencies) {
			if (dependency != null && !dependency.IsMet(context))
				return false;
		}

		return true;
	}

	internal bool ApplyCosts(ActionContext context) {
		context = PrepareContext(context as ItemUseContext);

		foreach (var dependency in Dependencies) {
			if (dependency != null && !dependency.ApplyCost(context))
				return false;
		}

		return true;
	}

	private ItemUseContext PrepareContext(ItemUseContext context = null, PlayerRuntimeStats playerStats = null) {
		context ??= new ItemUseContext();

		RunData runData = null;
		PlayerData playerData = null;

		try {
			var saveNode = SaveNode.Get();
			runData = saveNode.RunData;
			playerData = saveNode.PlayerData;
		}
		catch (System.Exception exception) {
			GD.PushError($"ItemDependency: SaveNode.Get() failed while preparing dependency context: {exception.Message}");
		}

		context.RunData ??= runData;
		context.PlayerData ??= playerData;
		context.PlayerSkills ??= context.PlayerData?.Skills;
		context.PlayerStats ??= playerStats ?? PlayerRuntimeStats.CreateWithDefaults(context.PlayerData);

		if (context.RunData == null)
			GD.PushError("ItemDependency: RunData was not found while preparing dependency context.");

		if (context.PlayerData == null)
			GD.PushError("ItemDependency: PlayerData was not found while preparing dependency context.");

		if (context.PlayerSkills == null)
			GD.PushError("ItemDependency: PlayerSkillData was not found while preparing dependency context.");

		context.Mana ??= context.PlayerStats.CurrentMana;
		context.Stamina ??= context.PlayerStats.CurrentStamina;

		var requiredAmmoType = GetRequiredAmmoType();
		if (context.AmmoType == null && requiredAmmoType != null)
			context.AmmoType = requiredAmmoType;

		if (context.AmmoType != null) {
			context.AmmoItem ??= context.PlayerStats.GetAmmoItem(context.AmmoType.Value);

			if (context.AmmoItem == null)
				GD.PushError($"ItemDependency: Required ammo item of type {context.AmmoType.Value} was not found.");
		}

		return context;
	}

	public AmmoType? GetRequiredAmmoType() {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyAmmo ammoDependency) return ammoDependency.AmmoType;
		}

		return null;
	}

	public int? GetManaCost() {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyMana manaDependency) return manaDependency.Cost;
		}

		return null;
	}

	public int? GetStaminaCost() {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyStamina staminaDependency) return staminaDependency.Cost;
		}

		return null;
	}

	public int? GetRequiredSkillLevel(PlayerSkillData.PlayerSkillType skillType) {
		foreach (var dependency in Dependencies) {
			if (dependency is DependencyLevel levelDependency && levelDependency.SkillType == skillType)
				return levelDependency.RequiredLevel;
		}

		return null;
	}
}
