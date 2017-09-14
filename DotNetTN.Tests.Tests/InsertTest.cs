// <copyright file="InsertTest.cs">Copyright ©  2017</copyright>

using System;
using DotNetTN.Tests.Connector;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetTN.Tests.Connector.Tests
{
    [TestClass]
    [PexClass(typeof(Insert))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class InsertTest
    {

        [PexMethod]
        [PexAllowedException(typeof(TypeInitializationException))]
        public void add([PexAssumeUnderTest]Insert target)
        {
            target.add();
            // TODO: add assertions to method InsertTest.add(Insert)
        }
    }
}
