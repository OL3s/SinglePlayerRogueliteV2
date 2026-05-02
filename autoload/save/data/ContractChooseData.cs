using Godot;
using Godot.Collections;
using MyTypes;

namespace SaveData {
	[GlobalClass]
	public partial class ContractChooseData : SaveResource {
		private const int GeneratedContractCount = 3;
		private static readonly Biomes[] GrasslandsChoices = { Biomes.GrasslandsA };
		private static readonly Biomes[] GrasslandsProgressionChoices = { Biomes.GrasslandsA, Biomes.TundraB, Biomes.DesertB };
		private static readonly Biomes[] TundraBranchChoices = { Biomes.TundraB, Biomes.IcyC, Biomes.JungleC };
		private static readonly Biomes[] DesertBranchChoices = { Biomes.DesertB, Biomes.JungleC, Biomes.LavaC };
		private static readonly Biomes[] IcyBossChoice = { Biomes.IcyC, Biomes.IceBossD };
		private static readonly Biomes[] JungleBossChoice = { Biomes.JungleC, Biomes.JungleBossD };
		private static readonly Biomes[] LavaBossChoice = { Biomes.LavaC, Biomes.LavaBossD };
		private static readonly Biomes[] FallbackChoices = { Biomes.GrasslandsA };

		[Export] public Array<Contract> Contracts { get; set; } = new();

		public Contract CreateRandomContract(Biomes currentBiome, int currentBiomeContractsCompleted, RandomNumberGenerator random = null) {
			random ??= new RandomNumberGenerator();
			random.Randomize();

			var availableBiomes = GetAvailableBiomes(currentBiome, currentBiomeContractsCompleted);
			var biome = GetWeightedRandomBiome(currentBiome, currentBiomeContractsCompleted, availableBiomes, random);

			return new Contract {
				Biome = biome
			};
		}

		public void GenerateContracts(Biomes currentBiome, int currentBiomeContractsCompleted) {
			Contracts.Clear();
			var random = new RandomNumberGenerator();
			random.Randomize();
			var allowDuplicates = GetAvailableContractVariantCount(currentBiome, currentBiomeContractsCompleted) < GeneratedContractCount;

			while (Contracts.Count < GeneratedContractCount) {
				var contract = CreateRandomContract(currentBiome, currentBiomeContractsCompleted, random);
				if (!allowDuplicates && ContainsContract(contract))
					continue;

				Contracts.Add(contract);
			}
		}

		private bool ContainsContract(Contract candidate) {
			foreach (var contract in Contracts) {
				if (contract == null)
					continue;

				if (contract.Biome == candidate.Biome)
					return true;
			}

			return false;
		}

		private static Biomes[] GetAvailableBiomes(Biomes currentBiome, int currentBiomeContractsCompleted) {
			if (currentBiome == Biomes.GrasslandsA && currentBiomeContractsCompleted <= 0)
				return GrasslandsChoices;

			return currentBiome switch {
				Biomes.GrasslandsA => GrasslandsProgressionChoices,
				Biomes.TundraB => TundraBranchChoices,
				Biomes.DesertB => DesertBranchChoices,
				Biomes.IcyC => IcyBossChoice,
				Biomes.JungleC => JungleBossChoice,
				Biomes.LavaC => LavaBossChoice,
				_ => FallbackChoices
			};
		}

		private static Biomes GetWeightedRandomBiome(Biomes currentBiome, int currentBiomeContractsCompleted, Biomes[] availableBiomes, RandomNumberGenerator random) {
			if (availableBiomes == null || availableBiomes.Length == 0)
				return Biomes.GrasslandsA;

			if (availableBiomes.Length == 1)
				return availableBiomes[0];

			var nextBiomeChance = GetNextBiomeChance(currentBiome, currentBiomeContractsCompleted);
			if (random.Randf() >= nextBiomeChance)
				return availableBiomes[0];

			if (availableBiomes.Length == 2)
				return availableBiomes[1];

			return random.Randf() < 0.6f ? availableBiomes[1] : availableBiomes[2];
		}

		private static int GetAvailableContractVariantCount(Biomes currentBiome, int currentBiomeContractsCompleted) {
			var biomes = GetAvailableBiomes(currentBiome, currentBiomeContractsCompleted);
			return biomes.Length;
		}

		private static float GetNextBiomeChance(Biomes currentBiome, int currentBiomeContractsCompleted) {
			return currentBiome switch {
				Biomes.GrasslandsA => Mathf.Clamp(0.15f + (currentBiomeContractsCompleted * 0.2f), 0.15f, 0.9f),
				Biomes.TundraB or Biomes.DesertB => Mathf.Clamp(0.2f + (currentBiomeContractsCompleted * 0.18f), 0.2f, 0.9f),
				Biomes.IcyC or Biomes.JungleC or Biomes.LavaC => Mathf.Clamp(0.35f + (currentBiomeContractsCompleted * 0.25f), 0.35f, 1.0f),
				_ => 1.0f
			};
		}

		public override string ToString() {
			if (Contracts == null || Contracts.Count == 0)
				return "[]";

			var result = "[";
			for (var i = 0; i < Contracts.Count; i++) {
				if (i > 0)
					result += ", ";

				var contract = Contracts[i];
				result += contract == null ? "None" : contract.Biome.ToString();
			}

			return result + "]";
		}
	}
}
