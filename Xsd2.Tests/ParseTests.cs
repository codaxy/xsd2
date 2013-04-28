using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PetaTest;

namespace Xsd2.Tests
{
    [TestFixture]
    class ParseTests
    {
        [Test]
        public void NullableFieldsAreRead()
        {
            var form = ParseForm(@"schemas\form1.xml");
            Assert.AreEqual(2, form.Items.Items.Count);
            Assert.AreEqual(1, form.Items.Items[0].MinLength);
            Assert.IsNull(form.Items.Items[1].MinLength);
        }

        private Form ParseForm(string path)
        {
            var xmlSerializer = new XmlSerializer(typeof(Form));
            using (var x = File.OpenRead(path))
                return (Form)xmlSerializer.Deserialize(x);
        }
    }
}
