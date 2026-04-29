using Godot;

[GlobalClass]
public partial class ItemInventory : Button {
	public ItemBase Item { get; private set; }
	public bool HasItem => Item != null;

	public override void _Ready() {
		ClipText = true;
		AutowrapMode = TextServer.AutowrapMode.WordSmart;
		AddThemeFontSizeOverride("font_size", 20);
	}

	public void SetItem(ItemBase item, Texture2D fallbackIcon) {
		Item = item;
		Text = item?.ItemName ?? "Empty";
		Icon = GetItemIcon(item, fallbackIcon);
	}

	public void Clear(Texture2D fallbackIcon) {
		SetItem(null, fallbackIcon);
	}

	private static Texture2D GetItemIcon(ItemBase item, Texture2D fallbackIcon) {
		if (item is ItemEquipable equipable && equipable.EquippedTexture != null) {
			return equipable.EquippedTexture;
		}

		return item?.Icon ?? fallbackIcon;
	}
}
