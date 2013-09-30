using System;

namespace CarDepot
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class PropertyIdAttribute: Attribute
    {
        PropertyId _propertyId;
        bool _isSubObjectProperty;

        public PropertyIdAttribute(PropertyId id)
        {
            _propertyId = id;
        }

        public PropertyId PropertyId
        {
            get { return _propertyId; }
            set { _propertyId = value; }
        }

        public bool IsSubObjectProperty
        {
            get { return _isSubObjectProperty; }
            set { _isSubObjectProperty = value; }
        }
    }
}
