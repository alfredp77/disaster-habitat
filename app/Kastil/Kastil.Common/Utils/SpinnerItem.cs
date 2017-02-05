using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Common.Utils
{
    public class SpinnerItem
    {
        public SpinnerItem(Attribute attribute)
        {
            Attribute = attribute;
        }
        public Attribute Attribute { get; }
        public string Caption => Attribute.Key;

        public override string ToString()
        {
            return Caption;
        }

        public override bool Equals(object obj)
        {
            var item = obj as SpinnerItem;
            if (item == null)
                return false;
            return item.Caption == Caption;
        }

        public override int GetHashCode()
        {
            return Caption?.GetHashCode() ?? 0;
        }
    }
}
