using Godot;
using System;
using System.Collections.Generic;

public partial class SignalHandler : Node {
	[Signal] public delegate void SignalReceivedEventHandler(SignalType signalType, Variant data);

	private static readonly Dictionary<(SignalType signalType, Delegate handler), SignalReceivedEventHandler> SubscriptionWrappers = new();

	private static void Subscribe(SignalType signalType, Action<SignalType> handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler);
		if (SubscriptionWrappers.ContainsKey(key))
			return;

		SignalReceivedEventHandler wrapper = (receivedSignalType, _) => {
			if (receivedSignalType == signalType)
				handler(receivedSignalType);
		};

		SubscriptionWrappers[key] = wrapper;
		signalHandler.SignalReceived += wrapper;
	}

	private static void Subscribe(SignalType signalType, Action handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler as Delegate);
		if (SubscriptionWrappers.ContainsKey(key))
			return;

		SignalReceivedEventHandler wrapper = (receivedSignalType, _) => {
			if (receivedSignalType == signalType)
				handler();
		};

		SubscriptionWrappers[key] = wrapper;
		signalHandler.SignalReceived += wrapper;
	}

	private static void Subscribe<[MustBeVariant] T>(SignalType signalType, Action<T> handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler as Delegate);
		if (SubscriptionWrappers.ContainsKey(key))
			return;

		SignalReceivedEventHandler wrapper = (receivedSignalType, data) => {
			if (receivedSignalType != signalType)
				return;

			handler(GetPayload<T>(receivedSignalType, data));
		};

		SubscriptionWrappers[key] = wrapper;
		signalHandler.SignalReceived += wrapper;
	}

	private static void Subscribe<[MustBeVariant] T>(SignalType signalType, Action<SignalType, T> handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler as Delegate);
		if (SubscriptionWrappers.ContainsKey(key))
			return;

		SignalReceivedEventHandler wrapper = (receivedSignalType, data) => {
			if (receivedSignalType != signalType)
				return;

			handler(receivedSignalType, GetPayload<T>(receivedSignalType, data));
		};

		SubscriptionWrappers[key] = wrapper;
		signalHandler.SignalReceived += wrapper;
	}

	private static void Unsubscribe(SignalType signalType, Action<SignalType> handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler);
		if (!SubscriptionWrappers.TryGetValue(key, out var wrapper))
			return;

		signalHandler.SignalReceived -= wrapper;
		SubscriptionWrappers.Remove(key);
	}

	private static void Unsubscribe(SignalType signalType, Action handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler as Delegate);
		if (!SubscriptionWrappers.TryGetValue(key, out var wrapper))
			return;

		signalHandler.SignalReceived -= wrapper;
		SubscriptionWrappers.Remove(key);
	}

	private static void Unsubscribe<[MustBeVariant] T>(SignalType signalType, Action<T> handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler as Delegate);
		if (!SubscriptionWrappers.TryGetValue(key, out var wrapper))
			return;

		signalHandler.SignalReceived -= wrapper;
		SubscriptionWrappers.Remove(key);
	}

	private static void Unsubscribe<[MustBeVariant] T>(SignalType signalType, Action<SignalType, T> handler) {
		var signalHandler = Get();
		if (signalHandler == null)
			return;

		var key = (signalType, handler as Delegate);
		if (!SubscriptionWrappers.TryGetValue(key, out var wrapper))
			return;

		signalHandler.SignalReceived -= wrapper;
		SubscriptionWrappers.Remove(key);
	}

	public static SignalHandler Get() {
		var sceneTree = Engine.GetMainLoop() as SceneTree;
		return sceneTree?.Root?.GetNodeOrNull<SignalHandler>("/root/SignalHandler");
	}

	public override void _Ready() {
		GD.Print("SignalHandler is ready and can now manage signals.");
	}

	private void EmitSignal(SignalType signalType) {
		EmitSignal(SignalName.SignalReceived, Variant.From(signalType), default(Variant));
	}

	private void EmitSignal<[MustBeVariant] T>(SignalType signalType, T data) {
		EmitSignal(SignalName.SignalReceived, Variant.From(signalType), ToVariant(data));
	}

	private static void EmitSignalStatic(SignalType signalType) {
		var signalHandler = Get();
		if (signalHandler == null)
			throw new System.Exception("SignalHandler instance not found in the scene tree. Ensure it is added as a child of the root node.");

		signalHandler.EmitSignal(signalType);
	}

	private static void EmitSignalStatic<[MustBeVariant] T>(SignalType signalType, T data) {
		var signalHandler = Get();
		if (signalHandler == null)
			throw new Exception("SignalHandler instance not found in the scene tree. Ensure it is added as a child of the root node.");

		signalHandler.EmitSignal(signalType, data);
	}

	public static void EmitSignalPurchaseItemStatic(ItemBase item) {
		EmitSignalStatic(SignalType.PurchaseItem, item);
	}

	public static void EmitSignalItemEquippedStatic(ItemBase item) {
		EmitSignalStatic(SignalType.ItemEquipped, item);
	}

	public static void EmitSignalGoldAmountChangedStatic(int goldAmount) {
		EmitSignalStatic(SignalType.GoldAmountChanged, goldAmount);
	}

	public static void SubscribePurchaseItem(Action<ItemBase> handler) {
		Subscribe(SignalType.PurchaseItem, handler);
	}

	public static void SubscribeItemEquipped(Action<ItemBase> handler) {
		Subscribe(SignalType.ItemEquipped, handler);
	}

	public static void SubscribeGoldAmountChanged(Action<int> handler) {
		Subscribe(SignalType.GoldAmountChanged, handler);
	}

	public static void UnsubscribePurchaseItem(Action<ItemBase> handler) {
		Unsubscribe(SignalType.PurchaseItem, handler);
	}

	public static void UnsubscribeItemEquipped(Action<ItemBase> handler) {
		Unsubscribe(SignalType.ItemEquipped, handler);
	}

	public static void UnsubscribeGoldAmountChanged(Action<int> handler) {
		Unsubscribe(SignalType.GoldAmountChanged, handler);
	}

	private static T GetPayload<[MustBeVariant] T>(SignalType signalType, Variant data) {
		try {
			return data.As<T>();
		}
		catch (InvalidCastException exception) {
			throw new InvalidCastException($"Signal '{signalType}' payload could not be cast to '{typeof(T).Name}'.", exception);
		}
	}

	private static Variant ToVariant<[MustBeVariant] T>(T data) {
		if (data == null)
			return default;

		return Variant.From(data);
	}

	public enum SignalType {
		PurchaseItem,
		ItemEquipped,
		GoldAmountChanged,
	}
}
