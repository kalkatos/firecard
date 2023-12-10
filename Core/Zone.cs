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
        public event Action<List<Card>> OnCardGroupEntered;
        public event Action<Card> OnCardEntered;
        public event Action<List<Card>> OnCardGroupLeft;
        public event Action<Card> OnCardLeft;
        public event Action OnShuffled;

        internal string id;
        internal string name;
        internal List<string> tags;
        internal List<Card> cards;

        public string Name => name;
        public Card[] Cards => cards.ToArray();
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

        public Card GetCardAt (int index)
        {
            if (cards.Count == 0)
                return null;
            else if (index < 0 || index >= cards.Count)
                return null;
            return cards[index];
        }

        public bool PushCard (Card card, Action<Card, Zone, Zone> cardEnteredCallback = null, Action<Card, Zone> cardLeftCallback = null)
        {
            if (cards.Contains(card))
                return false;
            AddCard(card, cards.Count, cardEnteredCallback, cardLeftCallback);
            return true;
        }

        public void PushCards (List<Card> cardList, Action<Card, Zone, Zone> cardEnteredCallback = null, Action<Card, Zone> cardLeftCallback = null)
        {
            List<Card> added = new();
            foreach (Card card in cardList)
                if (PushCard(card, cardEnteredCallback, cardLeftCallback))
                    added.Add(card);
            OnCardGroupEntered?.Invoke(added);
        }

        public bool InsertCard (Card card, int index = 0, Action<Card, Zone, Zone> cardEnteredCallback = null, Action<Card, Zone> cardLeftCallback = null)
        {
            if (cards.Contains(card)
                || index < 0
                || index > cards.Count)
                return false;
            AddCard(card, index, cardEnteredCallback, cardLeftCallback);
            return true;
        }

        public void InsertCards (List<Card> cardList, int index = 0, Action<Card, Zone, Zone> cardEnteredCallback = null, Action<Card, Zone> cardLeftCallback = null)
        {
            List<Card> added = new();

            for (int i = cardList.Count - 1; i >= 0; i--)
            {
                Card card = cardList[i];
                if (InsertCard(card, 0, cardEnteredCallback, cardLeftCallback))
                    added.Add(card);
            }
        }

        public Card PopCard (int index = -1, Action<Card, Zone> cardRemovedCallback = null)
        {
            if (cards.Count == 0)
                return null;
            if (index == -1)
                index = cards.Count - 1;
            else if (index < 0 || index >= cards.Count)
                return null;
            Card card = cards[index];
            RemoveCard(card, cardRemovedCallback);
            return card;
        }

        public bool PopCard (Card card, Action<Card, Zone> cardRemovedCallback = null)
        {
            if (cards.Count == 0 || !cards.Contains(card))
                return false;
            RemoveCard(card, cardRemovedCallback);
            return true;
        }

        public void PopCards (List<Card> cardList, Action<Card, Zone> cardLeftCallback = null)
        {
            List<Card> removed = new();
            foreach (Card card in cardList)
                if (PopCard(card, cardLeftCallback))
                    removed.Add(card);
            OnCardGroupLeft?.Invoke(removed);
        }

        public void Shuffle ()
        {
            Random rand = Match.Random;
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                Card temp = cards[j];
                cards[j] = cards[i];
                cards[i] = temp;
            }
            OnShuffled?.Invoke();
        }

        public int IndexOf (Card card)
        {
            return cards.IndexOf(card);
        }

        public static ZoneGetter Tag (string tag)
        {
            return new ZoneGetter().Tag(tag);
        }

        public static ZoneGetter Id (Variable variable)
        {
            return new ZoneGetter().Id(variable.Value);
        }

        public static ZoneGetter Card (CardGetter cardGetter)
        {
            return new ZoneGetter().Card(cardGetter);
        }

        private void AddCard (Card card, int index, Action<Card, Zone, Zone> addCallback, Action<Card, Zone> removeCallback)
        {
            Zone oldZone = card.CurrentZone;
            if (oldZone != null)
                oldZone.RemoveCard(card, removeCallback);
            card.currentZone = this;
            cards.Insert(index, card);
            OnCardEntered?.Invoke(card);
            addCallback?.Invoke(card, this, oldZone);
        }

        private void RemoveCard (Card card, Action<Card, Zone> callback)
        {
            card.currentZone = null;
            cards.Remove(card);
            OnCardLeft?.Invoke(card);
            callback?.Invoke(card, this);
        }
    }
}