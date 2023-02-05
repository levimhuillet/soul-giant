using UnityEngine;
using BeauUtil;
using BeauUtil.Extensions;

namespace SoulGiant {
    static public class Layers  {
        static public int Default = LayerMask.NameToLayer("Default");
        static public int Player = LayerMask.NameToLayer("Player");
        static public int Projectile = LayerMask.NameToLayer("Projectile");
        static public int Solid = LayerMask.NameToLayer("Solid");
        static public int Scannable = LayerMask.NameToLayer("Scannable");
    }
}