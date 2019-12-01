using System;
using System.Collections.Generic;

namespace UniInject
{
    public class BindingBuilder
    {
        private readonly List<BindingUnderConstruction> bindingsUnderConstruction = new List<BindingUnderConstruction>();

        public BindingBuilder()
        {
        }

        public void BindExistingInstance<T>(T instance)
        {
            BindingUnderConstruction b = new BindingUnderConstruction(typeof(T));
            bindingsUnderConstruction.Add(b);
            b.ToExistingInstance(instance);
        }

        public void BindTypeToNewInstancesOfIt(Type type)
        {
            BindingUnderConstruction b = new BindingUnderConstruction(type);
            bindingsUnderConstruction.Add(b);
            b.ToNewInstancesOfType(type);
        }

        public void BindTypeToSingleInstanceOfIt(Type type)
        {
            BindingUnderConstruction b = new BindingUnderConstruction(type);
            bindingsUnderConstruction.Add(b);
            b.ToSingleInstanceOfType(type);
        }

        public BindingUnderConstruction Bind(Type key)
        {
            BindingUnderConstruction b = new BindingUnderConstruction(key);
            bindingsUnderConstruction.Add(b);
            return b;
        }

        public BindingUnderConstruction Bind(object key)
        {
            BindingUnderConstruction b = new BindingUnderConstruction(key);
            bindingsUnderConstruction.Add(b);
            return b;
        }

        public List<IBinding> GetBindings()
        {
            List<IBinding> result = new List<IBinding>();
            foreach (BindingUnderConstruction bindingUnderConstruction in bindingsUnderConstruction)
            {
                IBinding binding = bindingUnderConstruction.GetBinding();
                if (binding == null)
                {
                    throw new InjectionException("Unfinished binding for key " + bindingUnderConstruction.GetKey());
                }
                result.Add(binding);
            }

            if (result.Count == 0)
            {
                throw new InjectionException("No bindings in BindingBuilder");
            }
            return result;
        }

        public class BindingUnderConstruction
        {
            private readonly object key;

            private IBinding binding;

            public BindingUnderConstruction(Type key)
            {
                this.key = key;
            }

            public BindingUnderConstruction(object key)
            {
                this.key = key;
            }

            public void ToExistingInstance<T>(T instance)
            {
                IProvider provider = new ExistingInstanceProvider<T>(instance);
                IBinding binding = new Binding(key, provider);
                this.binding = binding;
            }

            public void ToNewInstancesOfType(Type type)
            {
                IProvider provider = new NewInstancesProvider(type);
                IBinding binding = new Binding(key, provider);
                this.binding = binding;
            }

            public void ToSingleInstanceOfType(Type type)
            {
                IProvider provider = new SingleInstanceProvider(type);
                IBinding binding = new Binding(key, provider);
                this.binding = binding;
            }

            public void ToProvider(IProvider provider)
            {
                IBinding binding = new Binding(key, provider);
                this.binding = binding;
            }

            public object GetKey()
            {
                return key;
            }

            public IBinding GetBinding()
            {
                return binding;
            }
        }
    }
}