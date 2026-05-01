using Godot;
using SaveData;
using MyTypes;
using System;

public partial class SaveNode : Node {
	private const int StartCharacterSkillXpPool = 600;
	private const float StartCharacterSkillSpikeBias = 3.0f;

	[Export] public MetaData DefaultMetaData { get; set; } = new MetaData();
	[Export] public RunData DefaultRunData { get; set; } = new RunData();
	[Export] public SettingsData DefaultSettingsData { get; set; } = new SettingsData();
	[Export] public CharacterNameData CharacterNames { get; set; } = new CharacterNameData();
	[Export] public string SavePath { get; set; } = "user://saves/";
	public MetaData MetaData { get; set; }
	public RunData RunData { get; set; }
	public SettingsData SettingsData { get; set; }
	public PlayerData[] StartCharacters { get; private set; } = Array.Empty<PlayerData>();
	public bool HadPlayerDataOnLoad { get; private set; }
	public PlayerData PlayerData => RunData.PlayerData;
	public InventoryData InventoryData => PlayerData.InventoryData;
	public EquipedItemsData EquipedItemsData => PlayerData.EquipedItems;
	public static SaveNode Get() => Engine.GetMainLoop() is SceneTree tree ? tree.Root.GetNode<SaveNode>("SaveNode") : throw new InvalidOperationException("SaveNode: Unable to find SaveNode in the scene tree. Ensure that SaveNode is added as a child of the root node and is named 'SaveNode'.");
	public override void _Ready() {
		if (DefaultMetaData == null || DefaultRunData == null || DefaultSettingsData == null) throw new InvalidOperationException("DefaultMetaData, DefaultRunData, and DefaultSettingsData must be assigned in the inspector.");

		ExecuteReady();
	}

	public void ExecuteReady() {
		DirAccess.MakeDirRecursiveAbsolute(SavePath);
		LoadAllData();

		if (!FilesExist())
			SaveAllData();

		RunData.PlayerData ??= new PlayerData();
		RunData.PlayerData.InventoryData ??= new InventoryData();
		RefreshStartCharacters();
		GD.Print("SaveNode is ready. MetaData, RunData, and SettingsData have been initialized.");
	}

	public void RefreshStartCharacters() {
		StartCharacters = new[] { CreateStartCharacter(1), CreateStartCharacter(2), CreateStartCharacter(3) };
	}

	private PlayerData CreateStartCharacter(int index) {
		var random = new RandomNumberGenerator();
		random.Randomize();
		var skills = CreateStartCharacterSkills(random);

		return new PlayerData {
			PlayerName = CharacterNames.GetRandomName(random),
			StartingItem = StartingItems.GetRandomItem(random),
			Skills = skills
		};
	}

	private static PlayerSkillData CreateStartCharacterSkills(RandomNumberGenerator random) {
		var xp = new int[4];
		var fractionalRemainders = new float[4];
		var weights = new float[4];
		var totalWeight = 0.0f;

		for (var i = 0; i < weights.Length; i++) {
			weights[i] = Mathf.Pow(random.RandfRange(0.0001f, 1.0f), StartCharacterSkillSpikeBias);
			totalWeight += weights[i];
		}

		var assignedXp = 0;
		for (var i = 0; i < xp.Length; i++) {
			var exactXp = StartCharacterSkillXpPool * (weights[i] / totalWeight);
			xp[i] = Mathf.FloorToInt(exactXp);
			fractionalRemainders[i] = exactXp - xp[i];
			assignedXp += xp[i];
		}

		for (var i = StartCharacterSkillXpPool - assignedXp; i > 0; i--) {
			var bestIndex = 0;
			for (var j = 1; j < fractionalRemainders.Length; j++) {
				if (fractionalRemainders[j] > fractionalRemainders[bestIndex])
					bestIndex = j;
			}

			xp[bestIndex]++;
			fractionalRemainders[bestIndex] = -1.0f;
		}

		return new PlayerSkillData {
			StrengthXp = xp[0],
			AgilityXp = xp[1],
			ArcanaXp = xp[2],
			VitalityXp = xp[3]
		};
	}

	public void SaveData(SaveResource data, FileType type) {
		var filePath = GetSavePath(type);
		var error = ResourceSaver.Save(data, filePath);
		if (error != Error.Ok) {
			GD.PrintErr($"Failed to save data to {filePath}: {error}");
		}
		else {
			GD.Print($"Data successfully saved to {filePath}");
		}
	}

	public Resource LoadData(FileType type) {
		var filePath = GetSavePath(type);

		if (!FileAccess.FileExists(filePath)) {
			GD.Print($"No existing data found at {filePath}. A new instance will be created.");
			return null;
		}

		var resource = ResourceLoader.Load(filePath);
		if (resource == null) {
			GD.PrintErr($"Failed to load data from {filePath}");
			return null;
		}

		GD.Print($"Data successfully loaded from {filePath}");
		return resource;
	}

	public override void _ExitTree() {
		SaveAllData();
		GD.Print("SaveNode is exiting. All data has been saved.");
	}

	public void DeleteData(FileType type) {
		var filePath = GetSavePath(type);
		if (!FileAccess.FileExists(filePath)) {
			GD.Print($"No data found at {filePath} to delete.");
			return;
		}

		var error = DirAccess.RemoveAbsolute(filePath);
		GD.Print(error == Error.Ok
			? $"Data successfully deleted at {filePath}"
			: $"Failed to delete data at {filePath}: {error}");
	}

	public void DeleteMetaData() => DeleteData(FileType.Meta);
	public void DeleteRunData() => DeleteData(FileType.Run);
	public void DeleteSettingsData() => DeleteData(FileType.Settings);
	public bool FileExists(FileType type) => FileAccess.FileExists(GetSavePath(type));
	public bool MetaDataExists() => FileExists(FileType.Meta);
	public bool RunDataExists() => FileExists(FileType.Run);
	public bool SettingsDataExists() => FileExists(FileType.Settings);
	public void SaveMetaData() => SaveData(MetaData, FileType.Meta);
	public void SaveRunData() => SaveData(RunData, FileType.Run);
	public void SaveSettingsData() => SaveData(SettingsData, FileType.Settings);

	public void DeleteAllData() {
		DeleteData(FileType.Meta);
		DeleteData(FileType.Run);
		DeleteData(FileType.Settings);
	}

	public void WipeRun() {
		GD.Print("WipeRun: resetting run data.");
		RunData = new RunData();
		MetaData ??= new MetaData();
		MetaData.RunCount++;
		GD.Print($"WipeRun: run count increased to {MetaData.RunCount}.");
		SaveMetaData();
	}

	public void SaveAllData() {
		SaveMetaData();
		SaveRunData();
		SaveSettingsData();
	}

	public void LoadAllData() {
		var loadedRunData = LoadData(FileType.Run) as RunData;
		HadPlayerDataOnLoad = loadedRunData?.PlayerData != null;

		MetaData = LoadData(FileType.Meta) as MetaData ?? DefaultMetaData;
		RunData = loadedRunData ?? DefaultRunData;
		SettingsData = LoadData(FileType.Settings) as SettingsData ?? DefaultSettingsData;

		if (MetaData.IsFirstTimePlayer) GD.Print("First time player detected. Tutorial gameplay enabled.");

		GD.Print(MetaData.ToString());
		GD.Print(RunData.ToString());
		GD.Print(SettingsData.ToString());
	}

	public static string GetSavePath(FileType type) => Get().SavePath + $"{type.ToString().ToLower()}_data.tres";

	private bool FilesExist() {
		return FileAccess.FileExists(GetSavePath(FileType.Meta)) &&
			   FileAccess.FileExists(GetSavePath(FileType.Run)) &&
			   FileAccess.FileExists(GetSavePath(FileType.Settings));
	}
}
