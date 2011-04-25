using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Machine.Specifications.Model;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Bindings;

namespace Prism.NinjectExtension.Tests
{
    [Subject("Ninject Service Locator Adapter")]
    public class when_retrieving_services
    {
        private static IKernel _kernel;
        private static NinjectServiceLocatorAdapter _serviceLocator;
        private static string _serviceName = "serviceName";
        private static object _objOne;
        private static object _objTwo;
        private static IEnumerable<object> _returnedList;
        private static object _returnedObject;
        private static object _returnedNamedObject;
        
        private Establish context = () =>
                                        {
                                            _objOne = new object();
                                            _objTwo = new object();

                                            _kernel = new StandardKernel();
                                            _kernel.Bind<object>().ToConstant(_objOne).Named(_serviceName);
                                            _kernel.Bind<object>().ToConstant(_objTwo);
                                            

                                            _serviceLocator = new NinjectServiceLocatorAdapter(_kernel);
                                        };

        private Because of = () =>
                                 {
                                     _returnedList = _serviceLocator.GetAllInstances(typeof(object));
                                     _returnedObject = _serviceLocator.GetInstance(typeof(object));
                                     _returnedNamedObject = _serviceLocator.GetInstance(typeof (object), _serviceName);

                                 };

        private It should_forward_the_get_all_instances_request_to_its_the_inner_IKernel = () => _returnedList.ShouldContainOnly(new List<object> { _objOne, _objTwo });
        private It should_forward_the_get_instance_request_to_its_the_inner_IKernel = () => _returnedObject.ShouldEqual(_objTwo);
        private It should_forward_the_get_named_instance_request_to_its_the_inner_IKernel = () => _returnedNamedObject.ShouldEqual(_objOne);
    }
}
