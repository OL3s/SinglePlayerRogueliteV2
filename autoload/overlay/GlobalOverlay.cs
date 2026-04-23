using Godot;

public partial class GlobalOverlay : CanvasLayer
{
	public int OverlayCount => GetChildCount();

	public static GlobalOverlay Get()
	{
		var sceneTree = Engine.GetMainLoop() as SceneTree;
		return sceneTree?.Root?.GetNodeOrNull<GlobalOverlay>("/root/GlobalOverlay");
	}

	public void AddOverlay(PackedScene overlayScene)
	{
		if (overlayScene == null)
		{
			GD.PushError("Overlay scene is null, cannot add.");
			return;
		}

		AddChild(overlayScene.Instantiate());
	}

	public void CloseTopOverlay()
	{
		var childCount = GetChildCount();
		if (childCount == 0)
		{
			return;
		}

		GetChild(childCount - 1).QueueFree();
	}

	public void CloseAllOverlays()
	{
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}
	}

	public Error ChangeRootScene(PackedScene scene)
	{
		if (scene == null)
		{
			GD.PushError("Scene is null, cannot change root scene.");
			return Error.InvalidParameter;
		}

		CloseAllOverlays();

		var sceneTree = Engine.GetMainLoop() as SceneTree;
		if (sceneTree == null)
		{
			GD.PushError("SceneTree is unavailable, cannot change root scene.");
			return Error.Unavailable;
		}

		return sceneTree.ChangeSceneToPacked(scene);
	}
}
