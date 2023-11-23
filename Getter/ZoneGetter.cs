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

        public ZoneGetter Id (string id)
        {
            Filters.Add(new ZoneFilter_Id(id));
            return this;
        }

        public List<Zone> GetZones ()
        {
            List<Zone> zones = new List<Zone>(Match.GetState().Zones);
            Filter(zones);
            return zones;
        }

        public override object Get ()
        {
            return GetZones();
        }

        private void Filter (List<Zone> zones)
        {
            if (zones.Count == 0 || Filters.Count == 0)
                return;
            zones = zones.Where((z) => Filters.TrueForAll((f) => f.IsMatch(z))).ToList();
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

    internal class ZoneFilter_Id : Filter<Zone>
    {
        [JsonProperty]
        internal string id;

        internal ZoneFilter_Id () { }

        internal ZoneFilter_Id (string tag)
        {
            Operation = Operation.Equals;
            id = tag;
        }

        internal override bool IsMatch (Zone zone)
        {
            return Resolve(zone.id, Match.GetStringVariable(id)) || Resolve(zone.id, id);
        }
    }
}
