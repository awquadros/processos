using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Awfq.Processos.App.Utils
{
    /// <sumary>
    /// Essa classe é uma implementação da Microsoft que que dá novas possíbilidades 
    /// para a abstração de Enumerações.
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
    /// </sumary>
    public abstract class Enumeration : IComparable
    {
        protected string Name { get; private set; }

        protected int Id { get; private set; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);

        public override int GetHashCode()
        {
            int hashCode = 1460282102;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }
    }
}