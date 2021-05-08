using JetBrains.ProjectModel;
using JetBrains.ReSharper.Plugins.Pascal;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.UI.Icons;

namespace JetBrains.ReSharper.Plugins.Spring.Pascal
{
    [ProjectFileType(typeof(PascalProjectFileType))]
    public class PascalProjectFileLanguageService : ProjectFileLanguageService
    {
        public PascalProjectFileLanguageService(ProjectFileType projectFileType) : base(projectFileType)
        {
        }

        public override IPsiSourceFileProperties GetPsiProperties(IProjectFile projectFile, IPsiSourceFile sourceFile,
            IsCompileService isCompileService)
        {
            this.AssertProjectFileType(projectFile.LanguageType);
            return new DefaultPsiProjectFileProperties(projectFile, sourceFile);
        }

        public override PsiLanguageType GetPsiLanguageType(IProjectFile projectFile)
        {
            return GetPsiLanguageType(projectFile.LanguageType);
        }

        public override PsiLanguageType GetPsiLanguageType(IPsiSourceFile sourceFile)
        {
            var projectFile = sourceFile.ToProjectFile();
            return projectFile != null ? GetPsiLanguageType(projectFile) : GetPsiLanguageType(sourceFile.LanguageType);
        }

        public override PsiLanguageType GetPsiLanguageType(ProjectFileType languageType)
        {
            return PascalLanguage.Instance;
        }

        public override ILexerFactory GetMixedLexerFactory(ISolution solution, IBuffer buffer,
            IPsiSourceFile sourceFile = null)
        {
            return new PascalLanguageService.PascalLexerFactory();
        }

        protected override PsiLanguageType PsiLanguageType => PascalLanguage.Instance;
        public override IconId Icon => null;
    }
}
