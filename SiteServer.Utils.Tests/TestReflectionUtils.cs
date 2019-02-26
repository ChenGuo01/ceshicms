﻿using Xunit;

namespace SiteServer.Utils.Tests
{
    public class TestReflectionUtils
    {
        public class MyClass
        {
            public string Foo { get; set; }
            private string Bar { get; set; }
            protected string Pro { get; set; }
            internal string Inter { get; set; }
        }

        [Fact]
        public void TestGetAllInstancePropertyInfosEmpty()
        {
            var properties = ReflectionUtils.GetAllInstancePropertyInfos(typeof(int));
            Assert.Empty(properties);
        }

        [Fact]
        public void TestGetAllInstancePropertyInfosObject()
        {
            var properties = ReflectionUtils.GetAllInstancePropertyInfos(typeof(MyClass));
            Assert.Equal(4, properties.Length);
        }

        [Fact]
        public void TestToKeyValueListEmpty()
        {
            var kvs = ReflectionUtils.ToKeyValueList(null);
            Assert.Empty(kvs);
        }

        [Fact]
        public void TestToKeyValueListObject()
        {
            var kvs = ReflectionUtils.ToKeyValueList(new
            {
                A = "a",
                B = "b",
                C = "c",
                D = "d"
            });
            Assert.Equal(4, kvs.Count);
        }
    }
}
