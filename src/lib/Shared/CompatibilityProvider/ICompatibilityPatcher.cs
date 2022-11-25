namespace Shared.CompatibilityProvider
{
    public interface ICompatibilityPatcher
    {
        /// <summary>
        /// Patches the exam if required
        /// </summary>
        /// <param name="exam">The exam to patch</param>
        /// <returns>true if any patching took place</returns>
        bool Patch(Exam exam);

        /// <summary>
        /// This is the version of the application that this patcher is no longer required to be execute.
        /// For example;  if this was set to 4 for a specific patcher then it is assumed any exam below version 4
        /// would require this patcher to be run on it.  Exams with version 4 or higher should not need this patcher
        /// as the functionality should have been native during creation.
        /// </summary>
        int VersionThreshold { get; }
    }
}