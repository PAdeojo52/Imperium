using Imperium.Engine.Data.DataModels;
using Imperium.Engine.Data.EntityModels.Combat;
using Imperium.Engine.Data.Enums;
using Imperium.Engine.Data.Enums.Combat;
using Imperium.Engine.Data.Enums.Dice;

namespace Imperium.Engine.Services
{
    /// <summary>
    /// Core combat engine. Handles turn resolution for both Arena and Battle modes.
    /// 
    /// Arena flow:  Player selects move → resolve → update timer → update star rating → check time limit → swap turns
    /// Battle flow: Player selects move → resolve → check death → offer spare/kill → swap turns
    /// </summary>
    public class CombatService
    {
        private readonly DiceService _dice;

        public event Action<TurnAction>? OnTurnResolved;
        public event Action<CombatState>? OnCombatEnded;
        public event Action<WhisperResult, CombatMove?>? OnWhisperResolved;
        public event Action<CrowdReaction>? OnCrowdReactionChanged;

        public CombatService(DiceService dice)
        {
            _dice = dice;
        }

        // =================================================================
        //  Combat Initialization
        // =================================================================

        public CombatState StartArenaMatch(Combatant player, Combatant opponent,
            int timeLimitSeconds = 900, string? arenaName = null, string? promoterName = null)
        {
            return new CombatState
            {
                Mode = CombatMode.Arena,
                Player = player,
                Opponent = opponent,
                TimeLimitSeconds = timeLimitSeconds,
                ArenaName = arenaName,
                PromoterName = promoterName,
                ActiveFighterIndex = DetermineInitiative(player, opponent)
            };
        }

        public CombatState StartBattle(Combatant player, Combatant opponent)
        {
            return new CombatState
            {
                Mode = CombatMode.Battle,
                Player = player,
                Opponent = opponent,
                ActiveFighterIndex = DetermineInitiative(player, opponent)
            };
        }

        private int DetermineInitiative(Combatant a, Combatant b)
        {
            int aInit = _dice.RollTotal(DiceType.D10, 1, a.Agility / 10);
            int bInit = _dice.RollTotal(DiceType.D10, 1, b.Agility / 10);
            return aInit >= bInit ? 0 : 1;
        }

        // =================================================================
        //  Turn Execution
        // =================================================================

        public TurnAction ExecuteMove(CombatState state, CombatMove move)
        {
            var attacker = state.ActiveFighter;
            var defender = state.DefendingFighter;

            var action = new TurnAction
            {
                TurnNumber = state.CurrentTurn,
                ActorIndex = state.ActiveFighterIndex,
                Move = move
            };

            switch (move.Category)
            {
                case MoveCategory.Whisper:
                    ResolveWhisper(state, action, attacker, defender);
                    break;
                case MoveCategory.Taunt:
                    ResolveTaunt(state, action, attacker);
                    break;
                case MoveCategory.Defensive:
                    ResolveDefensiveMove(state, action, attacker);
                    break;
                case MoveCategory.Item:
                    ResolveItemUse(state, action, attacker);
                    break;
                default:
                    ResolveAttack(state, action, attacker, defender, move);
                    break;
            }

            // Apply costs
            attacker.CurrentStamina = Math.Max(0, attacker.CurrentStamina - move.StaminaCost);
            attacker.CurrentMP = Math.Max(0, attacker.CurrentMP - move.ManaCost);
            if (move.CooldownTurns > 0)
                attacker.Cooldowns[move.Id] = move.CooldownTurns;

            // Arena: timer and star rating
            if (state.Mode == CombatMode.Arena)
            {
                state.MatchTimeSeconds += move.TimeCostSeconds;
                action.MatchTimeSeconds = state.MatchTimeSeconds;
                UpdateStarRating(state, action, move);
                UpdateCrowdMood(state, action);

                if (state.IsOverTime)
                {
                    EndCombat(state, CombatOutcome.Draw);
                    action.Narration += " The promoter signals — time's up! The match is a draw.";
                }
            }

            // Check KO
            if (!defender.IsAlive)
            {
                var outcome = state.ActiveFighterIndex == 0 ? CombatOutcome.Victory : CombatOutcome.Defeat;
                EndCombat(state, outcome);
                action.Narration += $" {defender.Name} has fallen!";
            }

            state.CombatLog.Add(action);
            OnTurnResolved?.Invoke(action);

            if (state.IsActive)
                AdvanceTurn(state);

            return action;
        }

        // =================================================================
        //  Attack Resolution
        // =================================================================

        private void ResolveAttack(CombatState state, TurnAction action,
            Combatant attacker, Combatant defender, CombatMove move)
        {
            // Healing targets self
            if (move.Target == MoveTarget.Self)
            {
                int statBonus = attacker.GetStat(move.ScalingStat) / 10;
                var roll = _dice.Roll(move.DamageDie, move.DiceCount, move.DamageBonus + statBonus);
                action.Roll = roll;
                int healAmount = roll.Total;
                attacker.CurrentHP = Math.Min(attacker.MaxHP, attacker.CurrentHP + healAmount);
                action.DamageDealt = -healAmount;
                action.Narration = $"{attacker.Name} channels {move.Name} and recovers {healAmount} HP!";
                return;
            }

            int bonus = attacker.GetStat(move.ScalingStat) / 10;
            var damageRoll = _dice.Roll(move.DamageDie, move.DiceCount, move.DamageBonus + bonus);
            action.Roll = damageRoll;
            int rawDamage = damageRoll.Total;

            // Whispered move cooperation
            if (defender.WhisperedMove != null && defender.WhisperedMove.Id == move.Id && defender.WillCooperate)
            {
                action.WasWhispered = true;
                action.WasCountered = true;
                rawDamage = (int)(rawDamage * 0.7f);
                action.StarDelta += 0.15f;
                action.Narration = $"{attacker.Name} hits {move.Name} — {defender.Name} sells it beautifully!";
                defender.WhisperedMove = null;
                defender.WillCooperate = false;
            }
            else
            {
                action.Narration = $"{attacker.Name} uses {move.Name}!";
            }

            // Defense
            int defense = (defender.Endurance / 10) + _dice.RollValue(DiceType.D4);

            // Check for active defensive buff
            if (defender.ActiveBuffs.Contains("Block"))
            {
                defense += _dice.RollTotal(DiceType.D6, 1, defender.Endurance / 10);
                defender.ActiveBuffs.Remove("Block");
                action.Narration += $" {defender.Name} blocks!";
            }
            else if (defender.ActiveBuffs.Contains("Dodge"))
            {
                int dodgeRoll = _dice.RollTotal(DiceType.D10, 1, defender.Agility / 10);
                if (dodgeRoll > rawDamage / 2)
                {
                    rawDamage = 0;
                    defender.ActiveBuffs.Remove("Dodge");
                    action.Narration += $" {defender.Name} dodges completely!";
                    action.StarDelta += 0.15f;
                }
                else
                {
                    defender.ActiveBuffs.Remove("Dodge");
                }
            }

            int finalDamage = Math.Max(1, rawDamage - defense);

            // Exhaustion penalty
            if (attacker.IsExhausted)
            {
                finalDamage = (int)(finalDamage * 0.5f);
                action.Narration += " (exhausted)";
            }

            // Crit
            if (damageRoll.HasCrit)
            {
                finalDamage = (int)(finalDamage * 1.5f);
                action.Narration += " CRITICAL HIT!";
                action.StarDelta += 0.10f;
            }

            // If dodge nullified damage
            if (rawDamage == 0) finalDamage = 0;

            defender.CurrentHP = Math.Max(0, defender.CurrentHP - finalDamage);
            action.DamageDealt = finalDamage;
            action.Narration += $" ({finalDamage} damage)";
        }

        // =================================================================
        //  Whisper
        // =================================================================

        private void ResolveWhisper(CombatState state, TurnAction action,
            Combatant attacker, Combatant defender)
        {
            int discretionRoll = _dice.RollTotal(DiceType.D10, 1, attacker.Charisma / 10);
            WhisperResult result;

            if (discretionRoll <= 2)
            {
                result = WhisperResult.Exposed;
                state.StarRating = Math.Max(0, state.StarRating - 0.25f);
                action.StarDelta = -0.25f;
                action.Narration = $"{attacker.Name} tries to whisper but the crowd notices! Star rating drops.";
            }
            else
            {
                bool willCooperate = !defender.IsPlayer &&
                    (defender.CrowdFavor > 30 || defender.HPPercent > 0.4f);

                if (willCooperate)
                {
                    result = WhisperResult.Accepted;
                    defender.WillCooperate = true;
                    action.Narration = $"{attacker.Name} whispers to {defender.Name}... they nod subtly.";
                }
                else
                {
                    result = WhisperResult.Refused;
                    defender.WillCooperate = false;
                    action.Narration = $"{attacker.Name} whispers to {defender.Name}... but they refuse.";
                }
            }

            OnWhisperResolved?.Invoke(result, null);
        }

        public void SetWhisperedMove(CombatState state, CombatMove move)
        {
            state.DefendingFighter.WhisperedMove = move;
        }

        // =================================================================
        //  Taunt
        // =================================================================

        private void ResolveTaunt(CombatState state, TurnAction action, Combatant attacker)
        {
            int charismaRoll = _dice.RollTotal(DiceType.D10, 1, attacker.Charisma / 10);

            if (attacker.IsFanFavorite)
            {
                attacker.CrowdFavor = Math.Min(100, attacker.CrowdFavor + charismaRoll);
                action.StarDelta = 0.15f + (charismaRoll > 8 ? 0.10f : 0f);
                action.Narration = $"{attacker.Name} plays to the crowd — they love it!";
            }
            else
            {
                if (charismaRoll >= 6)
                {
                    attacker.CrowdFavor = Math.Min(100, attacker.CrowdFavor + charismaRoll / 2);
                    action.StarDelta = 0.10f;
                    action.Narration = $"{attacker.Name} taunts — the crowd reluctantly cheers.";
                }
                else
                {
                    attacker.CrowdFavor = Math.Max(0, attacker.CrowdFavor - 5);
                    action.StarDelta = -0.10f;
                    action.Narration = $"{attacker.Name} taunts — the crowd boos!";
                }
            }
        }

        // =================================================================
        //  Defensive & Item
        // =================================================================

        private void ResolveDefensiveMove(CombatState state, TurnAction action, Combatant attacker)
        {
            attacker.ActiveBuffs.Add(action.Move.Name);
            action.Narration = $"{attacker.Name} takes a defensive stance ({action.Move.Name}).";
            action.StarDelta = action.Move.CrowdAppeal;
        }

        private void ResolveItemUse(CombatState state, TurnAction action, Combatant attacker)
        {
            // TODO: Integrate with inventory system
            action.Narration = $"{attacker.Name} uses an item.";
        }

        // =================================================================
        //  Star Rating & Crowd
        // =================================================================

        private void UpdateStarRating(CombatState state, TurnAction action, CombatMove move)
        {
            float delta = move.CrowdAppeal + action.StarDelta;

            // Variety bonus
            if (state.LastMoveCategory.HasValue && move.Category != state.LastMoveCategory.Value)
            {
                delta += 0.05f;
                state.MomentumStreak++;
            }
            else if (move.Category != MoveCategory.Whisper)
            {
                state.MomentumStreak = 0;
            }

            if (state.MomentumStreak >= 3) delta += 0.10f;
            if (state.MomentumStreak >= 5) delta += 0.10f;

            // Drama bonuses
            if (state.Player.HPPercent < 0.5f && state.Opponent.HPPercent < 0.5f)
                delta += 0.05f;
            if (state.Player.HPPercent < 0.2f || state.Opponent.HPPercent < 0.2f)
                delta += 0.10f;

            state.StarRating = Math.Clamp(state.StarRating + delta, 0f, CombatState.MaxSecretStars);
            action.StarDelta = delta;
            state.LastMoveCategory = move.Category;
        }

        private void UpdateCrowdMood(CombatState state, TurnAction action)
        {
            CrowdReaction newMood = state.StarRating switch
            {
                >= 5.0f => CrowdReaction.Electric,
                >= 3.5f => CrowdReaction.Excited,
                >= 2.0f => CrowdReaction.Interested,
                >= 0.5f => CrowdReaction.Neutral,
                _ => CrowdReaction.Bored
            };

            if (newMood != state.CrowdMood)
            {
                state.CrowdMood = newMood;
                OnCrowdReactionChanged?.Invoke(newMood);
            }
            action.CrowdState = state.CrowdMood;
        }

        // =================================================================
        //  Turn Management
        // =================================================================

        private void AdvanceTurn(CombatState state)
        {
            state.ActiveFighterIndex = state.ActiveFighterIndex == 0 ? 1 : 0;
            if (state.ActiveFighterIndex == 0)
                state.CurrentTurn++;

            state.ActiveFighter.TickCooldowns();
            var active = state.ActiveFighter;
            active.CurrentStamina = Math.Min(active.MaxStamina, active.CurrentStamina + 3);
        }

        private void EndCombat(CombatState state, CombatOutcome outcome)
        {
            state.IsActive = false;
            state.Outcome = outcome;
            OnCombatEnded?.Invoke(state);
        }

        // =================================================================
        //  AI
        // =================================================================

        public CombatMove GetAIMove(CombatState state)
        {
            var fighter = state.ActiveFighter;
            var available = MoveLibrary.GetMovesForMode(state.Mode)
                .Where(m => !fighter.IsOnCooldown(m.Id))
                .Where(m => m.ManaCost <= fighter.CurrentMP)
                .Where(m => m.Category != MoveCategory.Whisper)
                .Where(m => m.RequiredWeaponType == null)
                .ToList();

            if (available.Count == 0) return MoveLibrary.Punch;

            if (state.Mode == CombatMode.Arena)
            {
                if (fighter.WhisperedMove != null && fighter.WillCooperate)
                    return MoveLibrary.Dodge;

                var different = available.Where(m => m.Category != state.LastMoveCategory).ToList();
                if (different.Count > 0)
                    return different[_dice.RollValue(DiceType.D6) % different.Count];

                if (_dice.CoinFlip() && fighter.CurrentStamina > 20)
                    return fighter.IsFanFavorite ? MoveLibrary.TauntCrowd : MoveLibrary.TauntOpponent;
            }

            if (fighter.HPPercent < 0.3f && fighter.CurrentMP >= MoveLibrary.HealingSurge.ManaCost)
                return MoveLibrary.HealingSurge;

            var offensive = available
                .Where(m => m.Category is MoveCategory.Unarmed or MoveCategory.Weapon or MoveCategory.Magic)
                .ToList();

            if (offensive.Count > 0)
                return offensive[_dice.RollValue(DiceType.D6) % offensive.Count];

            return available[_dice.RollValue(DiceType.D4) % available.Count];
        }

        public List<CombatMove> GetAvailableMoves(CombatState state)
        {
            var fighter = state.Player;
            return MoveLibrary.GetMovesForMode(state.Mode)
                .Where(m => !fighter.IsOnCooldown(m.Id))
                .Where(m => m.ManaCost <= fighter.CurrentMP)
                .Where(m => m.StaminaCost <= fighter.CurrentStamina || m.StaminaCost <= 3)
                .Where(m => m.RequiredWeaponType == null)
                .ToList();
        }
    }
}
