#nullable enable

using Godot;
using System;

[GlobalClass]
public partial class ItemBase : Resource
{
	[Export] public string ItemName { get; set; } = "NONAME";
	[Export] public DependencyLevel? LevelDependency { get; set; }
	[Export] public Texture2D Icon { get; set; } = new PlaceholderTexture2D();
	[Export] public int MaxStackSize { get; set; } = 1;
	[Export] public int Cost { get; set; } = 0;
	[Export] public string ItemID { get; set; }
	public bool IsStackable => MaxStackSize > 1;

	public ItemBase()
	{
		ItemID = Guid.NewGuid().ToString();
	}

	public ItemBase(string itemName, DependencyLevel? levelDependency, Texture2D icon, int maxStackSize, int cost)
	{
		ItemName = itemName;
		LevelDependency = levelDependency;
		Icon = icon;
		MaxStackSize = maxStackSize;
		Cost = cost;
		ItemID = Guid.NewGuid().ToString();
	}
}
