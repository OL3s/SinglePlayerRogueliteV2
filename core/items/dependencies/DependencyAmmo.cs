using Godot;
using MyTypes;

[GlobalClass]
public partial class DependencyAmmo : Dependency
{
	[Export] public AmmoType AmmoType { get; set; }
}
