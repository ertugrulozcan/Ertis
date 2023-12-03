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
            Assert.That("ahmet-ertugrul-ozcan" == slug);
        }
        
        [Test]
        public void SlugifierTest1WithTrim()
        {
            var slug = Slugifier.Slugify("  Ahmet Ertuğrul Özcan   ");
            Assert.That("ahmet-ertugrul-ozcan" == slug);
        }

        [Test]
        public void SlugifierTest2()
        {
            var slug = Slugifier.Slugify("JúST å fëw wørds");
            Assert.That("just-a-few-words" == slug);
        }
        
        [Test]
        public void SlugifierTest3()
        {
            var slug = Slugifier.Slugify("Αυτή είναι μια δοκιμή");
            Assert.That("ayti-einai-mia-dokimi" == slug);
        }
        
        [Test]
        public void SlugifierTest4()
        {
            var slug = Slugifier.Slugify("Wikipedia");
            Assert.That("wikipedia" == slug);
        }
        
        [Test]
        public void SlugifierTest5()
        {
            var slug = Slugifier.Slugify("Haber Başlığı 1 - Update 1");
            Assert.That("haber-basligi-1-update-1" == slug);
        }
        
        [Test]
        public void SlugifierTest6()
        {
            var slug = Slugifier.Slugify("Haber Başlığı 1 (Update 1)");
            Assert.That("haber-basligi-1-update-1" == slug);
        }
        
        [Test]
        public void SlugifierTest7()
        {
            var slug = Slugifier.Slugify("base_user");
            Assert.That("base-user" == slug);
        }
        
        [Test]
        public void SlugifierTest8()
        {
            var slug = Slugifier.Slugify("base_user", Slugifier.Options.Ignore('_'));
            Assert.That("base_user" == slug);
        }

        #endregion
    }
}