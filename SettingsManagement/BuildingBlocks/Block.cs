using SettingsManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SettingsManagement.BuildingBlocks
{
    static class BlockHelper
    {
        public static Block Create(this ModuleBuilder builder, Type type)
        {
            return new CreateTypeBlock(type, builder);
        }

        public static Block WithConstructor(this Block block)
        {
            return new ConstructorBlock(block);
        }

        public static Block WithRefreshIfNeeded(this Block block)
        {
            if (typeof(ICanRefresh).IsAssignableFrom(block.Interface))
                return block.WithRefresh();

            return block;
        }

        public static Block WithRefresh(this Block block)
        {
            return new RefreshBlock(block);
        }

        public static Block WithResetIfNeeded(this Block block)
        {
            if (typeof(ICanReset).IsAssignableFrom(block.Interface))
                return block.WithReset();

            return block;
        }

        public static Block WithReset(this Block block)
        {
            return new ResetBlock(block);
        }

        public static Block WithPersistIfNeeded(this Block block)
        {
            if (typeof(ICanPersist).IsAssignableFrom(block.Interface))
                return block.WithPersist();

            return block;
        }

        public static Block WithPersist(this Block block)
        {
            return new PersistBlock(block);
        }

        public static Block WithReadableValuesIfNeeded(this Block block)
        {
            if (typeof(ICanShowMyValues).IsAssignableFrom(block.Interface))
                return block.WithReadableValues();

            return block;
        }

        public static Block WithReadableValues(this Block block)
        {
            return new ReadableValuesBlock(block);
        }

        public static Block WithDescriptionsIfNeeded(this Block block)
        {
            if (typeof(IAmDescriptive).IsAssignableFrom(block.Interface))
                return block.WithDescriptions();

            return block;
        }

        public static Block WithDescriptions(this Block block)
        {
            return new DescriptionsBlock(block);
        }

        public static Block WithSerializerIfNeeded(this Block block)
        {
            if (typeof(ICanSerialize).IsAssignableFrom(block.Interface))
                return block.WithSerializer();

            return block;
        }

        public static Block WithSerializer(this Block block)
        {
            return new SerializerBlock(block);
        }

        public static Block WithDisposeIfNeeded(this Block block)
        {
            if (typeof(IDisposable).IsAssignableFrom(block.Interface))
                return block.WithDispose();

            return block;
        }

        public static Block WithDispose(this Block block)
        {
            return new DisposeBlock(block);
        }
    }

    abstract class Block
    {
        public abstract TypeBuilder Builder { get; }
        public abstract Type Interface { get; }
        public abstract IReadOnlyList<PropertyDescriptor> Properties { get; }
        public abstract FieldBuilder ConfigurationManagerField { get; }
        public abstract Type Build();

    }

    abstract class DecoratorBlock : Block
    {
        readonly Block _block;

        public sealed override TypeBuilder Builder => _block.Builder;
        public sealed override Type Interface => _block.Interface;
        public sealed override IReadOnlyList<PropertyDescriptor> Properties => _block.Properties;
        public sealed override FieldBuilder ConfigurationManagerField => _block.ConfigurationManagerField;

        public DecoratorBlock(Block block)
        {
            _block = block ?? throw new ArgumentNullException(nameof(block));
        }

        public sealed override Type Build()
        {
            GenerateIl();
            return _block.Build();
        }

        protected abstract void GenerateIl();
    }
}
