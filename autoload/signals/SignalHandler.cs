using Godot;
using System.Collections.Generic;

public partial class SignalHandler : Node
{
	[Signal] public delegate void SignalRecievedEventHandler(Signals singlaltype);

	private static readonly Dictionary<(Signals signalType, SignalRecievedEventHandler handler), SignalRecievedEventHandler> SubscriptionWrappers = new();

	public static void Subscribe(Signals signalType, SignalRecievedEventHandler handler)
	{
		var signalHandler = Get();
		if (signalHandler == null)
		{
			return;
		}

		var key = (signalType, handler);
		if (SubscriptionWrappers.ContainsKey(key))
		{
			return;
		}

		SignalRecievedEventHandler wrapper = receivedSignalType =>
		{
			if (receivedSignalType == signalType)
			{
				handler(receivedSignalType);
			}
		};

		SubscriptionWrappers[key] = wrapper;
		signalHandler.SignalRecieved += wrapper;
	}

	public static void Unsubscribe(Signals signalType, SignalRecievedEventHandler handler)
	{
		var signalHandler = Get();
		if (signalHandler == null)
		{
			return;
		}

		var key = (signalType, handler);
		if (!SubscriptionWrappers.TryGetValue(key, out var wrapper))
		{
			return;
		}

		signalHandler.SignalRecieved -= wrapper;
		SubscriptionWrappers.Remove(key);
	}

	public static SignalHandler Get()
	{
		var sceneTree = Engine.GetMainLoop() as SceneTree;
		return sceneTree?.Root?.GetNodeOrNull<SignalHandler>("/root/SignalHandler");
	}

	public override void _Ready()
	{
		GD.Print("SignalHandler is ready and can now manage signals.");
	}

	public void EmitSignal(Signals signalType)
	{
		EmitSignal(SignalName.SignalRecieved, Variant.From(signalType));
	}

	public static void EmitSignalStatic(Signals signalType)
	{
		var signalHandler = Get();
		if (signalHandler == null)
			throw new System.Exception("SignalHandler instance not found in the scene tree. Ensure it is added as a child of the root node.");

		signalHandler.EmitSignal(signalType);
	}

	public enum Signals
	{
		ContractSelected,
		PurchaseItem,
		ItemEquipped,
		GoldAmountChanged,
	}
}
