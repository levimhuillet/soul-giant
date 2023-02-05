using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;

namespace SoulGiant {
    static public class Layers  {
        static public int Layer_Default = LayerMask.NameToLayer("Default");
        static public int Layer_Player = LayerMask.NameToLayer("Player");
        static public int Layer_Projectile = LayerMask.NameToLayer("Projectile");
        static public int Layer_Solid = LayerMask.NameToLayer("Solid");
        static public int Layer_Scannable = LayerMask.NameToLayer("Scannable");
    }
}