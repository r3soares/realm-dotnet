using System.Linq;
using NUnit.Framework;

namespace Realms.Tests.Database
{
    [TestFixture, Preserve(AllMembers = true)]
    public class GenericWriteTests : RealmTest
    {
        [Test]
        public void TestGenericWriteObject()
        {
            using (var realm = Realm.GetInstance())
            {
                var obj = realm.Write(() =>
                {
                    return realm.Add(new IntPrimaryKeyWithValueObject
                    {
                        Id = 123,
                        StringValue = "my string"
                    });
                });

                Assert.That(obj.Id, Is.EqualTo(123));
                Assert.That(obj.StringValue, Is.EqualTo("my string"));
            }
        }

        [Test]
        public void TestGenericWritePrimitive()
        {
            using (var realm = Realm.GetInstance())
            {
                var value = realm.Write(() =>
                {
                    realm.Add(new IntPrimaryKeyWithValueObject
                    {
                        Id = 123,
                        StringValue = "my string"
                    });

                    return realm.All<IntPrimaryKeyWithValueObject>().Count();
                });

                Assert.That(value, Is.EqualTo(1));
            }
        }

        [Test]
        public void TestGenericWriteAsyncObject()
        {
            TestHelpers.RunAsyncTest(async () =>
            {
                using (var realm = Realm.GetInstance())
                {
                    var obj = await realm.WriteAsync(r =>
                    {
                        return r.Add(new IntPrimaryKeyWithValueObject
                        {
                            Id = 456,
                            StringValue = "my async string"
                        });
                    });

                    Assert.That(obj.Id, Is.EqualTo(456));
                    Assert.That(obj.StringValue, Is.EqualTo("my async string"));
                }
            });
        }

        [Test]
        public void TestGenericWriteAsyncPrimitive()
        {
            TestHelpers.RunAsyncTest(async () =>
            {
                using (var realm = Realm.GetInstance())
                {
                    var value = await realm.WriteAsync(r =>
                    {
                        r.Add(new IntPrimaryKeyWithValueObject
                        {
                            Id = 456,
                            StringValue = "my async string"
                        });

                        return r.All<IntPrimaryKeyWithValueObject>().Count();
                    });

                    Assert.That(value, Is.EqualTo(1));
                }
            });
        }
    }
}
