using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    [LanguageDefinition(Name)]
    public class PascalLanguage : KnownLanguage
    {
        public new const string Name = "Pascal";
    
        public new static PascalLanguage Instance { get; private set; }

        private PascalLanguage() : base(Name, Name)
        {
        }

        protected PascalLanguage([NotNull] string name) : base(name)
        {
        }

        protected PascalLanguage([NotNull] string name, [NotNull] string presentableName) : base(name, presentableName)
        {
        }
    }
}
