using System;
using System.Collections.Generic;

namespace FubuCsProjFile
{
    public class TargetFrameworkVersion : IEquatable<TargetFrameworkVersion>, IComparable,
        IComparable<TargetFrameworkVersion>
    {
        private Version version;

        /// <summary>
        /// Represents a target framework version
        /// </summary>
        /// <param name="version">In the form v2.0, v3.5, v4.0, v4.5, v4.5.1 etc...</param>
        public TargetFrameworkVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException("version");
            }

            this.version = new Version(version.TrimStart(new[] {'v'}));
        }

        public override string ToString()
        {
            return string.Format("v{0}", this.version);
        }

        public bool Equals(TargetFrameworkVersion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(version, other.version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TargetFrameworkVersion) obj);
        }


        public override int GetHashCode()
        {
            return (version != null ? version.GetHashCode() : 0);
        }

        public int CompareTo(TargetFrameworkVersion other)
        {

            if (other == null)
            {
                return 1;
            }

            return this.version.CompareTo(other.version);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            return this.CompareTo((TargetFrameworkVersion) obj);
        }

        public static bool operator ==(TargetFrameworkVersion left, TargetFrameworkVersion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TargetFrameworkVersion left, TargetFrameworkVersion right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(TargetFrameworkVersion x, TargetFrameworkVersion y)
        {
            if (x == null && y == null)
            {
                return false;
            }

            if (x == null)
            {
                return true;
            }

            return x.CompareTo(y) < 0;
        }

        public static bool operator >(TargetFrameworkVersion x, TargetFrameworkVersion y)
        {

            if (x == null && y == null)
            {
                return false;
            }

            if (x == null)
            {
                return false;
            }

            return x.CompareTo(y) > 0;
        }

        public static implicit operator string(TargetFrameworkVersion value)
        {
            return value == null ? null : value.ToString();
        }

        public static implicit operator TargetFrameworkVersion(string value)
        {
            return new TargetFrameworkVersion(value);
        }
    }
}