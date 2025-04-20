using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF9Tweaks.Tools
{
    internal class DataDumper
    {
        public static string ToHumanString(PLAYER player)
        {
            StringBuilder sb = new StringBuilder();

            //public string name;
            sb.Append("name:");
            sb.Append(player.name);
            sb.Append('\t');
            //public byte category;
            sb.Append("category:");
            sb.Append(player.category);
            sb.Append('\t');
            //public byte level;
            sb.Append("level:");
            sb.Append(player.level);
            sb.Append('\t');
            //public uint exp;
            sb.Append("exp:");
            sb.Append(player.exp);
            sb.Append('\t');
            //public byte trance;
            sb.Append("trance:");
            sb.Append(player.trance);
            sb.Append('\t');
            //public byte wep_bone;
            sb.Append("wep_bone:");
            sb.Append(player.wep_bone);
            sb.Append('\t');
            //public byte status;
            sb.Append("status:");
            sb.Append(player.status);
            sb.Append('\t');
            //public byte[] equip;
            sb.Append("equip:");
            sb.Append(Tools.ByteArrayToString(player.equip));
            sb.Append('\t');
            //public byte[] pa;
            sb.Append("pa:");
            sb.Append(Tools.ByteArrayToString(player.pa));
            sb.Append('\t');
            //public uint[] sa;
            sb.Append("sa:");
            sb.Append(Tools.UintArrayToString(player.sa));
            sb.Append('\t');


            sb.Append("party:");
            sb.Append(player.info.party);
            sb.Append('\t');
            sb.Append("menu_type:");
            sb.Append(player.info.menu_type);
            sb.Append('\t');
            sb.Append("row:");
            sb.Append(player.info.row);
            sb.Append('\t');
            sb.Append("serial_no:");
            sb.Append(player.info.serial_no);
            sb.Append('\t');
            sb.Append("slot_no:");
            sb.Append(player.info.slot_no);
            sb.Append('\t');
            sb.Append("win_pose:");
            sb.Append(player.info.win_pose);
            sb.Append('\t');

            return sb.ToString();
        }
    }
}
