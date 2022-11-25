using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Shared.Util;

namespace Shared.CompatibilityProvider
{
    public class CompatibilityProvider
    {
        private readonly List<ICompatibilityPatcher> _patchers;
        private readonly int _fileVersion;

        public CompatibilityProvider()
        {
            _patchers = new List<ICompatibilityPatcher> { new QuestionIdPatcher() };
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            _fileVersion = int.Parse(fvi.FileVersion[0].ToString());
        }

        public void Patch(Exam exam, string filePath)
        {
            if (exam.Properties.Version >= _patchers.Max(p => p.VersionThreshold))
                return;

            bool patchingWasRequired = false;

            foreach (var compatibilityPatcher in _patchers.Where(p => p.VersionThreshold > exam.Properties.Version))
            {
                bool somethingPatched = compatibilityPatcher.Patch(exam);

                if (!patchingWasRequired && somethingPatched)
                    patchingWasRequired = true;
            }

            if (patchingWasRequired)
                SaveExam(exam, filePath);
        }

        private void SaveExam(Exam exam, string filePath)
        {
            exam.Properties.Version = _fileVersion;
            Writer.ToOef(exam, filePath);
        }
    }
}
