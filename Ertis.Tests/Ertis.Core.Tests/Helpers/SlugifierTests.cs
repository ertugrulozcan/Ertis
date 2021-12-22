using Ertis.Core.Helpers;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Core.Tests.Helpers
{
    public class SlugifierTests
    {
        #region Methods

        [Test]
        public void SlugifierTest1()
        {
            var slug = Slugifier.Slugify("Ahmet Ertuğrul Özcan");
            Assert.AreEqual("ahmet-ertugrul-ozcan", slug);
        }
        
        [Test]
        public void SlugifierTest1WithTrim()
        {
            var slug = Slugifier.Slugify("  Ahmet Ertuğrul Özcan   ");
            Assert.AreEqual("ahmet-ertugrul-ozcan", slug);
        }

        [Test]
        public void SlugifierTest2()
        {
            var slug = Slugifier.Slugify("JúST å fëw wørds");
            Assert.AreEqual("just-a-few-words", slug);
        }
        
        [Test]
        public void SlugifierTest3()
        {
            var slug = Slugifier.Slugify("Αυτή είναι μια δοκιμή");
            Assert.AreEqual("ayti-einai-mia-dokimi", slug);
        }
        
        [Test]
        public void SlugifierTest4()
        {
            var slug = Slugifier.Slugify("Wikipedia");
            Assert.AreEqual("wikipedia", slug);
        }
        
        [Test]
        public void SlugifierTest5()
        {
            var slug = Slugifier.Slugify("Haber Başlığı 1 - Update 1");
            Assert.AreEqual("haber-basligi-1-update-1", slug);
        }
        
        [Test]
        public void SlugifierTest6()
        {
            var slug = Slugifier.Slugify("Haber Başlığı 1 (Update 1)");
            Assert.AreEqual("haber-basligi-1-update-1", slug);
        }

        #endregion
    }
}