using System;
using System.Threading.Tasks;

namespace SettingsManagement
{
    sealed partial class SettingsContext
    {
        public static class Tasks
        {
            public static Task Run(Action action)
            {
                return Run(action, TaskScheduler.Current);
            }

            public static Task Run(Action action, TaskScheduler taskScheduler)
            {
                var task = Create(action);
                task.Start(taskScheduler ?? TaskScheduler.Current);

                return task;
            }

            public static Task Create(Action action)
            {
                var scope = Current;

                return new Task(() =>
                {
                    using (Set(scope))
                    {
                        action();
                    }
                });
            }

            public static Task Run<T>(Func<T> function)
            {
                return Run(function, TaskScheduler.Current);
            }

            public static Task Run<T>(Func<T> function, TaskScheduler taskScheduler)
            {
                var task = Create(function);
                task.Start(taskScheduler);
                return task;
            }

            public static Task<T> Create<T>(Func<T> function)
            {
                var scope = Current;

                return new Task<T>(() =>
                {
                    using (Set(scope))
                    {
                        return function();
                    }
                });
            }
        }
    }
}
