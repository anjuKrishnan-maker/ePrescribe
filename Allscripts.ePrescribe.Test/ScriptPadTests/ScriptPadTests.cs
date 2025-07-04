using eRxWeb;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ScriptPadTests
{
    /*
     * No more classes should be added to this class.  If you are testing a method in the ScriptPad.cs file,
     * there should be 1 class per method.
     */
    [TestClass]
    public class ScriptPadTests
    {
        [TestMethod]
        public void should_get_pharmacy_id_if_it_is_set()
        {

            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetStringOrEmpty("PHARMACYID")).Return("one");

            string id = ScriptPad.GetPharmacyId(mock);
            Assert.AreEqual("one", id);
        }

        [TestMethod]
        public void should_get_lastpharmacyid_if_pharmacyid_is_null()
        {
            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetStringOrEmpty("LASTPHARMACYID")).Return("lf");
            mock.Stub(x => x.GetStringOrEmpty("PHARMACYID")).Return(string.Empty);

            var id = ScriptPad.GetPharmacyId(mock);
            Assert.AreEqual("lf", id);
        }


        [TestMethod]
        public void should_get_lastpharmacyid_if_pharmacyid_is_empty()
        {
            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetStringOrEmpty("LASTPHARMACYID")).Return("lf");
            mock.Stub(x => x.GetStringOrEmpty("PHARMACYID")).Return(string.Empty);

            var id = ScriptPad.GetPharmacyId(mock);
            Assert.AreEqual("lf", id);
        }

        [TestMethod]
        public void should_return_null_if_all_is_null()
        {
            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetStringOrEmpty("LASTPHARMACYID")).Return(string.Empty);
            mock.Stub(x => x.GetStringOrEmpty("PHARMACYID")).Return(string.Empty);

            var id = ScriptPad.GetPharmacyId(mock);
            Assert.AreEqual(null, id);
        }


        [TestMethod]
        public void should_get_physician_id_from_sessionUserId_if_delegateproviderid_is_null()
        {
            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetStringOrEmpty("DelegateProviderID")).Return(string.Empty);

            var id = ScriptPad.GetPhysicianId(mock, "id");
            Assert.AreEqual("id", id);
        }

        [TestMethod]
        public void should_get_physician_id_from_delegateproviderid_if_not_isPASupervised()
        {
            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetStringOrEmpty("DelegateProviderID")).Return("two");

            var id = ScriptPad.GetPhysicianId(mock, "id");
            Assert.AreEqual("two", id);
        }


        [TestMethod]
        public void should_get_physician_id_from_sessionUserId_if_isPASupervised()
        {
            var mock = MockRepository.GenerateStub<IStateContainer>();
            mock.Stub(x => x.GetBooleanOrFalse("IsPASupervised")).Return(true);
            mock.Stub(x => x.GetStringOrEmpty("DelegateProviderID")).Return("two");

            var id = ScriptPad.GetPhysicianId(mock, "id");
            Assert.AreEqual("id", id);
        }
    }


}
