using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Statistics
{
    public interface IModifier
    {
        IEnumerable<float> GetAdditiveModifier(StatcExpand statcExpand);
        IEnumerable<float> GetPercentageModifier(StatcExpand statcExpand);
    }
}