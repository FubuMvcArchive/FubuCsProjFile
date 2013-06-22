using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuCsProjFile.Testing
{
    [TestFixture]
    public class StringExtensionsTester
    {
        [Test]
        public void does_contain()
        {
            var list = new List<string> { "a", "b", "c", "d" };

            list.ContainsSequence(new string[] { "f" }).ShouldBeFalse();
            list.ContainsSequence(new string[] { "b", "d" }).ShouldBeFalse();
            list.ContainsSequence( new string[] { "c", "d", "e" }).ShouldBeFalse();
            list.ContainsSequence(new string[] { "b", "c", "e" }).ShouldBeFalse();
            list.ContainsSequence( new string[] { "b" }).ShouldBeTrue();
            list.ContainsSequence( new string[] { "b", "c" }).ShouldBeTrue();
            list.ContainsSequence( new string[] { "b", "c", "d" }).ShouldBeTrue();
        }
    }
}