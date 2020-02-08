using Microsoft.VisualStudio.TestTools.UnitTesting;
using RandomizerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static RandomizerApp.Enums;

namespace RandomizerTests
{
    [TestClass]
    public class AttributeTests
    {
        [TestMethod]
        public void TestStringValueAttribute()
        {
            var str = HintType.Both.GetStringValue();
            Assert.AreEqual("Spell + Helpful", str);
        }
    }
}
