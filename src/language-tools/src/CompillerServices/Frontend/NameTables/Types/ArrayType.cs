﻿using System.Text;

namespace CompillerServices.Frontend.NameTables.Types
{
    public class ArrayType : SlangType
    {
        public ArrayType(SlangType elementType, int dimentions)
        {
            ElementType = elementType;
            Dimentions = dimentions;
        }

        public SlangType ElementType { get; }
        public int Dimentions { get; }

        public override bool IsAssignable(SlangType other)
        {
            if (!(other is ArrayType otherArrayType))
            {
                return false;
            }

            return Dimentions == otherArrayType.Dimentions && ElementType.Equals(otherArrayType.ElementType);
        }

        public override bool Equals(SlangType other)
        {
            return IsAssignable(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ElementType != null ? ElementType.GetHashCode() : 0) * 397) ^ Dimentions;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("array ");

            for (int i = 0; i < Dimentions; i++)
            {
                builder.Append("[]");
            }

            builder.Append($" {ElementType}");

            return builder.ToString();
        }
    }
}