namespace SettingsManagement;

sealed partial class SettingsContext
{
    /// <summary>
    /// TPL Helper
    /// </summary>
    public static class Tasks
    {
        /// <summary>
        /// Run an action on the TPL, setting the current settingsmanager as current.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <returns>Awaitable</returns>
        public static Task Run(Action action)
        {
            return Run(action, TaskScheduler.Current);
        }

        /// <summary>
        /// Run an action on the TPL, setting the current settingsmanager as current, given a scheduler.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <param name="taskScheduler">The scheduler used to schedule the task</param>
        /// <returns>Awaitable</returns>
        public static Task Run(Action action, TaskScheduler taskScheduler)
        {
            var task = Create(action);
            task.Start(taskScheduler ?? TaskScheduler.Current);

            return task;
        }

        /// <summary>
        /// Create an async action, setting the current settingsmanager as current.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <returns>Awaitable</returns>
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

        /// <summary>
        /// Runs an async action, setting the current settingsmanager as current.
        /// </summary>
        /// <param name="function">The function to run</param>
        /// <returns>Awaitable with result</returns>
        public static Task Run<T>(Func<T> function)
        {
            return Run(function, TaskScheduler.Current);
        }

        /// <summary>
        /// Runs an async action, setting the current settingsmanager as current.
        /// </summary>
        /// <param name="function">The function to run</param>
        /// <param name="taskScheduler">The scheduler used to schedule the task</param>
        /// <returns>Awaitable with result</returns>
        public static Task Run<T>(Func<T> function, TaskScheduler taskScheduler)
        {
            var task = Create(function);
            task.Start(taskScheduler);
            return task;
        }

        /// <summary>
        /// Create an async action, setting the current settingsmanager as current.
        /// </summary>
        /// <param name="function">The function to run</param>
        /// <returns>Awaitable with result</returns>
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
