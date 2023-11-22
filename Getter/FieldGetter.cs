using System;

namespace Kalkatos.Firecard.Utility
{
    [Serializable]
    public class FieldGetter : Getter
    {
        public override object Get ()
        {
            throw new NotImplementedException();
        }

        public bool Evaluate ()
        {
            // TODO
            return false;
        }
    }
}
