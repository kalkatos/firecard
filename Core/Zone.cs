using Kalkatos.Firecard.Utility;
using System;
using System.Collections.Generic;

namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// The zone is a container for cards. Throughout the game, cards will move from zone to zone, and this class provides means to acces those cards and information about them, like index, count, etc.
    /// </summary>
    [Serializable]
    public class Zone
    {
        public event Action<Card> OnCardEntered;
        public event Action<Card> OnCardLeft;
        public event Action OnShuffled;

        internal string id;
        internal string name;
        internal List<string> tags;
        internal List<Card> cards;

        public string Name => name;
        public IReadOnlyList<string> Tags => tags.AsReadOnly();
        public IReadOnlyList<Card> Cards => cards.AsReadOnly();
        public int Count => cards.Count;

        public Zone () { }

        public Zone (ZoneData zoneData)
        {
            Setup(zoneData);
        }

        public void Setup (ZoneData zoneData)
        {
            name = zoneData.Name;
            tags = new List<string>(zoneData.Tags);
        }

        public bool HasTag (string tag)
        {
            return tags.Contains(tag);
        }

        public void PushCard (Card card)
        {
            if (cards.Contains(card))
                return;
            cards.Add(card);
            card.currentZone = this;
            OnCardEntered?.Invoke(card);
        }

        public void InsertCard (Card card, int index = 0)
        {
            if (cards.Contains(card)
                || index < 0
                || index > cards.Count)
                return;
            cards.Insert(index, card);
            card.currentZone = this;
            OnCardEntered?.Invoke(card);
        }

        public Card PopCard (int index = -1)
        {
            if (cards.Count == 0)
                return null;
            if (index == -1)
                index = cards.Count - 1;
            else if (index < 0 || index >= cards.Count)
                return null;
            Card card = cards[index];
            card.currentZone = null;
            cards.RemoveAt(index);
            OnCardLeft?.Invoke(card);
            return card;
        }

        public void Shuffle ()
        {
            Random rand = Match.Random;
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rand.Next(0, i);
                Card temp = cards[j];
                cards[j] = cards[i];
                cards[i] = temp;
            }
            OnShuffled?.Invoke();
        }

        public static ZoneGetter Tag (string tag)
        {
            return new ZoneGetter().Tag(tag);
        }
    }
}