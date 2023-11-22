using Kalkatos.Firecard.Core;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Utility
{
    /// <summary>
    /// Performs a selection in the Match zones according to parameters.
    /// </summary>
    [Serializable]
    public class ZoneGetter : Getter
    {
        [JsonProperty]
        internal List<Filter<Zone>> Filters = new();

        public ZoneGetter () { }

        public static ZoneGetter New => new ZoneGetter();

        public ZoneGetter Tag (string tag)
        {
            return Tag(tag, Operation.Equals);
        }

        public ZoneGetter Tag (string tag, Operation option)
        {
            Filters.Add(new ZoneFilter_Tag(tag));
            return this;
        }

        public List<Zone> GetZones ()
        {
            throw new NotImplementedException();
        }

        public override object Get ()
        {
            return GetZones();
        }

        private void Filter (List<Zone> zones)
        {
            if (Filters.Count == 0)
                return;
            zones = zones.Where((c) => Filters.TrueForAll((f) => f.IsMatch(c))).ToList();
        }
    }

    internal class ZoneFilter_Tag : Filter<Zone>
    {
        [JsonProperty]
        internal string plainTag;

        internal ZoneFilter_Tag () { }

        internal ZoneFilter_Tag (string tag)
        {
            Operation = Operation.Equals;
            plainTag = tag;
        }

        internal override bool IsMatch (Zone zone)
        {
            return zone.HasTag(plainTag);
        }
    }
}
