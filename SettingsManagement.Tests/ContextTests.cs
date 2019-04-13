using SettingsManagement.Tests.Models;
using Xunit;

namespace SettingsManagement.Tests
{
    public class ContextTests : IClassFixture<DefaultConfigurationManagerTestsFixture>
    {
        [Fact]
        public void ContextInstancesAreOnlyCreatedOnce()
        {
            var settings1 = SettingsContext.AppContext.Get<IMySettings>();
            Assert.NotNull(settings1);

            var settings2 = SettingsContext.AppContext.Get<IMySettings>();
            Assert.NotNull(settings2);

            Assert.True(ReferenceEquals(settings1, settings2));

            Assert.True(SettingsContext.AppContext.Remove<IMySettings>());
            Assert.False(SettingsContext.AppContext.HasManager<IMySettings>());
        }

        [Fact]
        public void InstancesAreInherited()
        {
            var settings = SettingsContext.AppContext.Get<IMySettings>();
            Assert.NotNull(settings);

            using (var scope = SettingsContext.BeginScope())
            {
                Assert.True(scope.HasManager<IMySettings>());

                Assert.True(SettingsContext.AppContext.Remove<IMySettings>());
                Assert.False(scope.HasManager<IMySettings>());
            }
        }

        [Fact]
        public void ChildScopesManageTheirOwnInstances()
        {
            using (var scope = SettingsContext.BeginScope())
            {
                var settings = scope.Get<IMySettings>();
                Assert.NotNull(settings);
                Assert.False(SettingsContext.AppContext.HasManager<IMySettings>(), "Existing child scopes should not affect Parent");
            }

            Assert.False(SettingsContext.AppContext.HasManager<IMySettings>(), "Disposed child scopes should not affect Parent");
        }

        [Fact]
        public void ChildScopesCanRemoveInstances()
        {
            using (var scope = SettingsContext.BeginScope())
            {
                var settings = scope.Get<IMySettings>();
                Assert.NotNull(settings);
                Assert.True(scope.HasManager<IMySettings>(), "Manager should exist");
                Assert.True(scope.Remove<IMySettings>(), "Manager should be found and removed");
                Assert.False(scope.HasManager<IMySettings>(), "Manager should not exist");
            }
        }

        [Fact]
        public void ChildScopesCantRemoveParentInstances()
        {
            using (var parentScope = SettingsContext.BeginScope())
            using (var scope = parentScope.BeginChildScope())
            {
                var settings = parentScope.Get<IMySettings>();
                Assert.NotNull(settings);
                Assert.True(scope.HasManager<IMySettings>(), "Manager should exist");
                Assert.False(scope.Remove<IMySettings>(), "Manager should not be removed");
                Assert.True(scope.HasManager<IMySettings>(), "Manager should still exist");
            }
        }

        [Fact]
        public void ChildScopesCanRemoveParentInstancesWhenRemovingRecursively()
        {
            using (var parentScope = SettingsContext.BeginScope())
            using (var scope = parentScope.BeginChildScope())
            {
                var settings = parentScope.Get<IMySettings>();
                Assert.NotNull(settings);
                Assert.True(scope.HasManager<IMySettings>(), "Manager should exist");
                Assert.True(scope.RemoveRecursive<IMySettings>(), "Manager should be removed");
                Assert.False(scope.HasManager<IMySettings>(), "Manager should not still exist");
            }
        }

        [Fact]
        public void ChildScopesManageTheirOwnInstancesAndShouldCleanUpTheirManagers()
        {
            SettingsContext scope;

            using (scope = SettingsContext.BeginScope())
            {
                var settings = scope.Get<IMySettings>();
                Assert.NotNull(settings);
                Assert.True(scope.HasManager<IMySettings>(), "Scopes should keep their manager until disposed");
            }

            //TODO: Should this crash since we disposed it?
            Assert.False(scope.HasManager<IMySettings>(), "Scopes should release their manager when disposed");
        }

        [Fact]
        public void TwoChildScopesManageTheirOwnInstances()
        {
            using (var scope1 = SettingsContext.BeginScope())
            {
                using (var scope2 = SettingsContext.BeginScope())
                {
                    var settings = scope2.Get<IMySettings>();
                    Assert.NotNull(settings);
                    Assert.False(scope1.HasManager<IMySettings>(), "Scopes should not have instances from other scopes");
                }
            }
        }

        [Fact]
        public void TwoChildScopesManageTheirOwnInstancesButSHouldntInfluenceTheOthers()
        {
            using (var scope1 = SettingsContext.BeginScope())
            {
                using (var scope2 = SettingsContext.BeginScope())
                {
                    var settings = scope1.Get<IMySettings>();
                    Assert.NotNull(settings);
                    Assert.False(scope2.HasManager<IMySettings>(), "Scopes should not have instances from other scopes");
                }

                Assert.True(scope1.HasManager<IMySettings>(), "Disposal of other scopes shouldn't influence the current scopes");
            }
        }

        [Fact]
        public void TwoChildScopesCanInherit()
        {
            using (var scope1 = SettingsContext.BeginScope())
            {
                using (var scope2 = scope1.BeginChildScope())
                {
                    var settings = scope1.Get<IMySettings>();
                    Assert.NotNull(settings);
                    Assert.True(scope2.HasManager<IMySettings>(), "Scopes should have instances from their parent scopes");
                }

                Assert.True(scope1.HasManager<IMySettings>(), "Disposal of other scopes shouldn't influence the current scopes");
            }
        }

        [Fact]
        public void ThreeChildScopesCanInherit()
        {
            using (var scope1 = SettingsContext.BeginScope())
            {
                using (var scope2 = scope1.BeginChildScope())
                {
                    using (var scope3 = scope2.BeginChildScope())
                    {
                        var settings = scope1.Get<IMySettings>();
                        Assert.NotNull(settings);
                        Assert.True(scope2.HasManager<IMySettings>(), "Scopes should have instances from their parent scopes");
                        Assert.True(scope3.HasManager<IMySettings>(), "Scopes should have instances from their parent scopes");
                    }

                    Assert.True(scope2.HasManager<IMySettings>(), "Child scopes should not influence instances from their parent scopes");
                }

                Assert.True(scope1.HasManager<IMySettings>(), "Disposal of other scopes shouldn't influence the current scopes");
            }
        }
    }
}
