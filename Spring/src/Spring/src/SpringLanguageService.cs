using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Host.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.UI.Icons;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Spring
{
  [ProjectFileTypeDefinition(Name)]
  public class SpringProjectFileType : KnownProjectFileType
  {
    public new const string Name = "Spring";

    public new static SpringProjectFileType Instance { get; private set; }

    private SpringProjectFileType() : base(Name, "Spring", new[] {Spring_EXTENSION})
    {
    }

    protected SpringProjectFileType(string name) : base(name)
    {
    }

    protected SpringProjectFileType(string name, string presentableName) : base(name, presentableName)
    {
    }

    protected SpringProjectFileType(string name, string presentableName, IEnumerable<string> extensions) : base(name,
      presentableName, extensions)
    {
    }

    public const string Spring_EXTENSION = ".Spring";
  }

  [ProjectFileType(typeof(SpringProjectFileType))]
  public class SpringProjectFileLanguageService : ProjectFileLanguageService
  {
    public SpringProjectFileLanguageService(ProjectFileType projectFileType) : base(projectFileType)
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
      return SpringLanguage.Instance;
    }

    public override ILexerFactory GetMixedLexerFactory(ISolution solution, IBuffer buffer,
      IPsiSourceFile sourceFile = null)
    {
      return new SpringLanguageService.SpringLexerFactory();
    }

    protected override PsiLanguageType PsiLanguageType => SpringLanguage.Instance;
    public override IconId Icon => null;
  }

  [LanguageDefinition(Name)]
  public class SpringLanguage : KnownLanguage
  {
    public new const string Name = "Spring";
    
    public new static SpringLanguage Instance { get; private set; }

    private SpringLanguage() : base(Name, Name)
    {
    }

    protected SpringLanguage([NotNull] string name) : base(name)
    {
    }

    protected SpringLanguage([NotNull] string name, [NotNull] string presentableName) : base(name, presentableName)
    {
    }
  }

  [Language(typeof(SpringLanguage))]
  class SpringLanguageService : LanguageService
  {
    public SpringLanguageService([NotNull] PsiLanguageType psiLanguageType,
      [NotNull] IConstantValueService constantValueService) : base(psiLanguageType, constantValueService)
    {
    }

    public override ILexerFactory GetPrimaryLexerFactory()
    {
      return new SpringLexerFactory();
    }

    public override ILexer CreateFilteringLexer(ILexer lexer)
    {
      return lexer;
    }

    public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile)
    {
      return new SpringParser(lexer);
    }

    internal class SpringParser : IParser
    {
      private readonly ILexer myLexer;

      public SpringParser(ILexer lexer)
      {
        myLexer = lexer;
      }

      public IFile ParseFile()
      {
        var file = new SpringFile();
        myLexer.Start();
        while (myLexer.TokenType != null)
        {
          file.AddChild(myLexer.TokenType.Create(myLexer.GetTokenText()));
          myLexer.Advance();
        }

        return file;
      }
    }

    public override IEnumerable<ITypeDeclaration> FindTypeDeclarations(IFile file)
    {
      return EmptyList<ITypeDeclaration>.Instance;
    }

    public override ILanguageCacheProvider CacheProvider => null;
    public override bool IsCaseSensitive => true;
    public override bool SupportTypeMemberCache => false;
    public override ITypePresenter TypePresenter => CLRTypePresenter.Instance;

    internal class SpringLexerFactory : ILexerFactory
    {
      public ILexer CreateLexer(IBuffer buffer)
      {
        return new CSharpLexer(buffer);
      }
    }
  }

  internal class SpringFile : FileElementBase
  {
    public override NodeType NodeType => SpringFileNodeType.Instance;

    public override PsiLanguageType Language => SpringLanguage.Instance;
  }

  internal class SpringFileNodeType : CompositeNodeType
  {
    public SpringFileNodeType(string s, int index) : base(s, index)
    {
    }

    public static readonly SpringFileNodeType Instance = new SpringFileNodeType("Spring_FILE", 0);

    public override CompositeElement Create()
    {
      return  new SpringFile();
    }
  }

  [Language(typeof(SpringLanguage))]
  internal class SpringSyntaxHighlightingManager : RiderSyntaxHighlightingManager
  {
    public override SyntaxHighlightingProcessor CreateProcessor()
    {
      return new SpringSyntaxHighlightingProcessor();
    }
  }

  class SpringSyntaxHighlightingProcessor : SyntaxHighlightingProcessor
  {
    protected override bool IsKeyword(TokenNodeType tokenType)
    {
      return base.IsKeyword(tokenType) || tokenType.IsConstantLiteral;
    }
  }

  [Language(typeof(SpringLanguage))]
  class SpringDaemonBehaviour : LanguageSpecificDaemonBehavior
  {
    public override ErrorStripeRequest InitialErrorStripe(IPsiSourceFile sourceFile)
    {
      return sourceFile.Properties.ShouldBuildPsi ? 
        ErrorStripeRequest.STRIPE_AND_ERRORS :
        ErrorStripeRequest.NONE;
    }
  }


}