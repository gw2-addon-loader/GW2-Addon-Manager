using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2AddonManager
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class DependentObservableObject : ObservableObject
    {
        protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);

            List<string> dependents;
            if (dependencyMap_.TryGetValue(e.PropertyName, out dependents))
            {
                foreach (var dependent in dependents)
                    OnPropertyChanged(dependent);
            }
        }

        protected override void OnPropertyChanging(PropertyChangingEventArgs e) {
            base.OnPropertyChanging(e);

            List<string> dependents;
            if (dependencyMap_.TryGetValue(e.PropertyName, out dependents))
            {
                foreach (var dependent in dependents)
                    OnPropertyChanging(dependent);
            }
        }

        protected DependentObservableObject()
        {
            if(dependencyMap_ == null)
            {
                dependencyMap_ = new Dictionary<string, List<string>>();
                foreach (var property in GetType().GetProperties())
                {
                    var dependsOn = property.GetCustomAttributes(true)
                                        .OfType<DependsOnAttribute>()
                                        .Select(attribute => attribute.Name);

                    foreach (var dependency in dependsOn)
                    {
                        if (property.Name == dependency)
                            throw new ApplicationException(String.Format("Circular dependency found: {0} of {1} cannot depends of itself", dependency, GetType().ToString()));

                        if (!dependencyMap_.TryGetValue(dependency, out List<string> list))
                        {
                            list = new List<string> { property.Name };
                            dependencyMap_.Add(dependency, list);
                        }
                        else
                            list.Add(property.Name);
                    }
                }
            }
        }

        private static Dictionary<string, List<string>> dependencyMap_ = null;
    }
}
