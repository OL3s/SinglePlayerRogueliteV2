using Godot;
using System;

public partial class Gem : Control
{
	public enum GemType
	{
		Blue,
		Red,
		Green,
	}

	private static readonly Texture2D BlueBackgroundTexture = GD.Load<Texture2D>("res://assets/gems/spr-gemstone-empty1.png");
	private static readonly Texture2D BlueGemTexture = GD.Load<Texture2D>("res://assets/gems/spr-gemstones-full1.png");
	private static readonly Texture2D RedBackgroundTexture = GD.Load<Texture2D>("res://assets/gems/spr-gemstone-empty2.png");
	private static readonly Texture2D RedGemTexture = GD.Load<Texture2D>("res://assets/gems/spr-gemstones-full2.png");
	private static readonly Texture2D GreenBackgroundTexture = GD.Load<Texture2D>("res://assets/gems/spr-gemstone-empty3.png");
	private static readonly Texture2D GreenGemTexture = GD.Load<Texture2D>("res://assets/gems/spr-gemstones-full3.png");

	[Export] public GemType Type { get; set; } = GemType.Blue;
	private TextureRect _background;
	private TextureRect _gemSprite;

	public override void _Ready()
	{
		_background = GetNodeOrNull<TextureRect>("Background") ?? throw new ArgumentNullException(nameof(_background), "Background node is missing.");
		_gemSprite = GetNodeOrNull<TextureRect>("Gem") ?? throw new ArgumentNullException(nameof(_gemSprite), "Gem node is missing.");

		(Texture2D backgroundTexture, Texture2D gemTexture) = Type switch
		{
			GemType.Blue => (BlueBackgroundTexture, BlueGemTexture),
			GemType.Red => (RedBackgroundTexture, RedGemTexture),
			GemType.Green => (GreenBackgroundTexture, GreenGemTexture),
			_ => throw new ArgumentOutOfRangeException(),
		};

		_background.Texture = backgroundTexture;
		_background.Visible = true;
		_gemSprite.Texture = gemTexture;
		_gemSprite.Visible = IsUnlocked();
	}

	private bool IsUnlocked()
	{
		var metaData = SaveNode.Get().MetaData;

		return Type switch
		{
			GemType.Blue => metaData.GemBlueCollected,
			GemType.Red => metaData.GemRedCollected,
			GemType.Green => metaData.GemGreenCollected,
			_ => false,
		};
	}
}
