using System;

#pragma warning disable 659

namespace CompillerServices.Frontend.NameTables.Types
{
    public abstract class SlangType : IEquatable<SlangType>
    {
        public abstract bool Equals(SlangType other);
        public abstract bool IsAssignable(SlangType other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SlangType) obj);
        }
    }
}