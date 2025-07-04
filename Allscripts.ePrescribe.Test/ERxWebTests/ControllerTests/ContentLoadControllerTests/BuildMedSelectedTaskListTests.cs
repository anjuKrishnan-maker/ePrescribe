using System;
using System.Collections.Generic;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.ContentLoadControllerTests
{
    [TestClass]
    public class BuildMedSelectedTaskListTests
    {
        [TestMethod]
        public void should_create_task_list()
        {
            var asyncUtil = MockRepository.GenerateMock<ILoadAsyncUtil>();

            var result = ContentLoadController.BuildMedSelectedTaskList(null, asyncUtil);

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<MedicationSelectedRequest, IStateContainer, List<SponsoredLinkModel>>>
                                                            .Matches(f => f.Method.Name == "GetRobustLinksData"),
                                                        Arg<MedicationSelectedRequest>.Is.Anything,
                                                        Arg<Action<List<SponsoredLinkModel>>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<MedicationSelectedRequest, IStateContainer, CopayCoverage>>
                                                            .Matches(f => f.Method.Name == "GetCopayCoverageList"),
                                                        Arg<MedicationSelectedRequest>.Is.Anything,
                                                        Arg<Action<CopayCoverage>>.Is.Anything));
            
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<MedicationSelectedRequest, IStateContainer, List<FormularyAlternative>>>
                                                            .Matches(f => f.Method.Name == "GetFormularyAlternatives"),
                                                        Arg<MedicationSelectedRequest>.Is.Anything,
                                                        Arg<Action<List<FormularyAlternative>>>.Is.Anything));
            
            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<MedicationSelectedRequest, IStateContainer, List<GenericAlternative>>>
                                                            .Matches(f => f.Method.Name == "getGenericAlternatives"),
                                                        Arg<MedicationSelectedRequest>.Is.Anything,
                                                        Arg<Action<List<GenericAlternative>>>.Is.Anything));

            Assert.AreEqual(4, result.Item1.Count);
        }
    }
}
