using System;
using Shared.Extensions;

namespace Shared.CompatibilityProvider
{
    /// <summary>
    /// Patches exam questions to have a unique id
    /// </summary>
    public class QuestionIdPatcher : ICompatibilityPatcher
    {
        public int VersionThreshold => 4;

        public bool Patch(Exam exam)
        {
            bool patched = false;

            foreach (Section examSection in exam.Sections)
            {
                foreach (Question question in examSection.Questions)
                {
                    if (question.Id.IsDefault())
                    {
                        question.Id = Guid.NewGuid();

                        if (!patched)
                            patched = true;
                    }
                }
            }

            return patched;
        }
    }
}