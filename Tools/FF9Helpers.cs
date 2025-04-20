using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FF9Tweaks.Tools
{
    public static class FF9Helpers
    {
        public static PlayerType GetPlayerType(PLAYER player)
        {
            PlayerType t = (PlayerType)player.info.menu_type;
            switch (t)
            {
                case PlayerType.Zidane2: return PlayerType.Zidane;
                case PlayerType.Cinna2: return PlayerType.Cinna;
                case PlayerType.Marcus2: return PlayerType.Marcus;
                case PlayerType.Blank2: return PlayerType.Blank;
                case PlayerType.Beatrix2: return PlayerType.Beatrix;
                case PlayerType.Cinna3: return PlayerType.Cinna;
                case PlayerType.Marcus3: return PlayerType.Marcus;
                case PlayerType.Blank3: return PlayerType.Blank;
                default: return t;
            }

        }

        public static PlayerType GetRealPlayerType(PLAYER player)
        {
            return (PlayerType)player.info.menu_type;
        }

        public static bool ForceAddCharacterToParty(int characterId, int slotIndex)
        {
            //Patches.ff9play_FF9Play_SetParty_Patch.CanRun = false;
            EventEngine eventEngine = PersistenSingleton<EventEngine>.Instance;

            if (slotIndex < 0 || slotIndex >= 4)
            {
                return false;
            }

            if(characterId >= 9)
            {
                ff9play.FF9Play_SetParty(slotIndex, characterId);
            }
            else
            {
                MethodInfo chr2slotMethod = typeof(EventEngine).GetMethod("chr2slot", BindingFlags.NonPublic | BindingFlags.Instance);
                int slot = (int)chr2slotMethod.Invoke(eventEngine, new object[] { characterId });

                if (slot < 0 || slot > 8)
                {
                    return false;
                }

                ff9play.FF9Play_SetParty(slotIndex, slot);
            }

            BattleAchievement.UpdateParty();

            MethodInfo setupPartyUIDMethod = typeof(EventEngine).GetMethod("SetupPartyUID", BindingFlags.NonPublic | BindingFlags.Instance);
            setupPartyUIDMethod.Invoke(eventEngine, null);

            //Patches.ff9play_FF9Play_SetParty_Patch.CanRun = true;
            return true; // Return true indicating the character was successfully added
        }
    }

    public enum PlayerType
    {
        Zidane,
        Vivi,
        Dagger,
        Steiner,
        Freya,
        Quina,
        Eiko,
        Amarant,
        Cinna,
        Cinna2,
        Marcus,
        Marcus2,
        Blank,
        Blank2,
        Beatrix,
        Beatrix2,
        Zidane2,
        Cinna3,
        Marcus3,
        Blank3
    }
}
