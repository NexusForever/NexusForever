using NexusForever.Shared.GameTable.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Shared.GameTable.Model
{
    public class SpellEffectDataUnitPropertyModifier : ICustomGameTableStructure
    {
        public uint Property;
        public uint Method;
        public float BaseValue;
        public float Modifier;
        public uint DataBits04;
        public uint DataBits05;
        public uint DataBits06;
        public uint DataBits07;
        public uint DataBits08;
        public uint DataBits09;
    }
}
